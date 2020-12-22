using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using COO.Application.Config.Config;
using COO.Application.Config.CountryShip;
using COO.Application.Config.Plant;
using COO.Application.MainFuction.BoomEcus;
using COO.Application.MainFuction.DeliverySale;
using COO.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;

namespace COO.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        //private StdSchedulerFactory _schedulerFactory;
        private IScheduler _schedulerTS;
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
            services.AddTransient<IPlantService, PlantService>();
            services.AddTransient<IConfigService, ConfigService>();
            services.AddTransient<ICountryShipService, CountryShipService>();
            // 1. For EcusTs
            services.AddTransient<CollectListEcusTS>();
            services.AddTransient<IEcusService, EcusService>();
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
            _schedulerTS = await _schedulerFactory.GetScheduler();
            await _schedulerTS.Start();


            // // Trigger now =>>>>
            ITrigger triggerNow = TriggerBuilder.Create()
                  .WithIdentity("trigger_Now", "group")
                  .StartNow()
                    .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever())
                 .Build();

            // Trigger For
            // 1. Ecus TS
            // At 01:00:00am, on every Monday, Tuesday, Wednesday, Thursday and Friday, every month
            ITrigger triggerTS = TriggerBuilder.Create()
                    .WithIdentity("triggerTS", "group1")
                    .WithCronSchedule("0 30 1 ? * MON,TUE,WED,THU,FRI *")
                    .Build();

            // Job Int
            // 1. EcusTS
            IJobDetail collectListEcusTS = JobBuilder.Create<CollectListEcusTS>()
                .WithIdentity("collectEcusJob", "gtoup1")
                .Build();

            // RUN =====================
            // 1. Ecus
            _schedulerTS.JobFactory = new EcusJobFactory(services.BuildServiceProvider());
            await _schedulerTS.ScheduleJob(collectListEcusTS, triggerTS, stoppingToken);
        }
    }

}
