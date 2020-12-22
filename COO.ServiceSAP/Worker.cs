using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using COO.Application.Config.Config;
using COO.Application.Config.CountryShip;
using COO.Application.Config.Plant;
using COO.Application.MainFuction.BoomEcus;
using COO.Application.MainFuction.DeliverySale;
using COO.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace COO.ServiceSAP
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        //private StdSchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        private readonly ISchedulerFactory _schedulerFactory;

        private ServiceCollection services;
        public Worker(ILogger<Worker> logger, ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;

            // Prepare the DI container
            // 0. Common
            services = new ServiceCollection();
            services.AddDbContext<COOContext>(options => options.UseSqlServer("data source=hvlappsdb02-dev;initial catalog=COO;user id=imes;password=jan2015;MultipleActiveResultSets=True;"));
            //services.AddDbContext<COOContext>(options => options.UseSqlServer("data source=HVNN0606\\SQLEXPRESS;initial catalog=COO;user id=sa;password=123;MultipleActiveResultSets=True;"));
            services.AddTransient<CollectListDS_Boom>();
            services.AddTransient<IPlantService, PlantService>();
            services.AddTransient<IConfigService, ConfigService>();
            services.AddTransient<ICountryShipService, CountryShipService>();
            services.AddTransient<IEcusService, EcusService>();
            services.AddTransient<IBoomService, BoomService>();
            services.AddTransient<IDeliverySaleService, DeliverySaleService>();
            services.AddTransient<IBoomEcusService, BoomEcusService>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been stopped...");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_schedulerFactory = new StdSchedulerFactory();
            _scheduler = await _schedulerFactory.GetScheduler();
            await _scheduler.Start();

            // // Trigger now =>>>>
            ITrigger triggerNow = TriggerBuilder.Create()
                  .WithIdentity("trigger_Now", "group")
                  .StartNow()
                    .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever())
                 .Build();

            // Trigger For
            // 1. DS
            // At second :00, at minute :00, at 06am, 07am, 08am, 09am, 10am, 11am, 12pm, 13pm, 14pm, 15pm, 16pm and 17pm, on every Monday, Tuesday, Wednesday, Thursday and Friday, every month
            ITrigger triggerDS = TriggerBuilder.Create()
                    .WithIdentity("triggerDS", "group2")
                    .WithCronSchedule("0 0 8,10,11,12,14,15,16,17,18,20 ? * MON,TUE,WED,THU,FRI *")
                    .Build();

            // Job Int
            IJobDetail collectListDS_Boom = JobBuilder.Create<CollectListDS_Boom>()
                .WithIdentity("collectListDS_Boom", "gtoup1")
                .Build();

            // RUN =====================
            _scheduler.JobFactory = new DS_BoomJobFactory(services.BuildServiceProvider());
            await _scheduler.ScheduleJob(collectListDS_Boom, triggerDS, stoppingToken);
        }
    }
}
