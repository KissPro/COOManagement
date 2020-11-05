using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using COO.Application.Common;
using COO.Application.Config.Config;
using COO.Application.Config.Plant;
using COO.Application.MainFuction.Boom;
using COO.Application.MainFuction.DeliverySale;
using COO.Application.MainFuction.EcusTS;
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
            services.AddDbContext<COOContext>(options => options.UseSqlServer("data source=HVNN0606\\SQLEXPRESS;initial catalog=COO;user id=sa;password=123;MultipleActiveResultSets=True;"));
            services.AddTransient<IPlantService, PlantService>();
            services.AddTransient<IConfigService, ConfigService>();
            services.AddTransient<ISAPService, SAPService>();
            // 1. For EcusTs
            services.AddTransient<CollectListEcusTS>();
            services.AddTransient<IEcusService, EcusService>();
            // 2. DS
            services.AddTransient<CollectListDS>();
            services.AddTransient<IDeliverySaleService, DeliverySaleService>();
            // 3. Boom
            services.AddTransient<CollectListBoom>();
            services.AddTransient<IBoomService, BoomService>();
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

            // Trigger
            // Running 0:00 -> 23h00  everyday
            ITrigger triggerDaily = TriggerBuilder.Create()
                .WithIdentity("triggerDaily", "group1")
                .WithCronSchedule("0 0 0,1,2,3,4,5,6,7,8,9,10,11,13,14,15,16,17,18,19,20,21,22,23 ? * * *")
                .Build();


            // Start at special timE
            var startTime = new DateTimeOffset(2020, 11, 4, 13, 17, 10,
                                 new TimeSpan(0, 0, 0));
            ITrigger triggerAtTime = TriggerBuilder.Create()
                  .WithIdentity("triggerAtTime", "group")
                  .StartAt(startTime)

              .Build();

            // Trigger by cron
            ITrigger triggerCron = TriggerBuilder.Create()
                    .WithIdentity("triggerDaily", "group1")
                    .WithCronSchedule("0 45 13 1/1 * ? *")
                    .Build();

            // Trigger now =>>>>
            ITrigger triggerNow = TriggerBuilder.Create()
                  .WithIdentity("trigger_Now", "group")
                  .StartNow()
              .Build();

            // Job Int
            // 1. EcusTS
            IJobDetail collectListEcusTS = JobBuilder.Create<CollectListEcusTS>()
                .WithIdentity("collectEcusJob", "gtoup")
                .Build();

            // 2. DS
            IJobDetail collectListDN = JobBuilder.Create<CollectListDS>()
                .WithIdentity("collectDSJob", "gtoup")
                .Build();

            // 3. Boom
            //IJobDetail collectListBoom = JobBuilder.Create<CollectListBoom>()
            //    .WithIdentity("job1", "gtoup")
            //    .Build();



            // RUN =====================
            // 1. Ecus
            //_scheduler.JobFactory = new EcusJobFactory(services.BuildServiceProvider());
            //await _scheduler.ScheduleJob(collectListEcusTS, triggerNow, stoppingToken);

            // 2. DS
            _scheduler.JobFactory = new DSJobFactory(services.BuildServiceProvider());
            await _scheduler.ScheduleJob(collectListEcusTS, triggerNow, stoppingToken);

        }
    }

}
