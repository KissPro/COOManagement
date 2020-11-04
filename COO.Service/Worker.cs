using System;
using System.Threading;
using System.Threading.Tasks;
using COO.Application.MainFuction.EcusTS;
using COO.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;

namespace COO.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private StdSchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
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
            _schedulerFactory = new StdSchedulerFactory();
            _scheduler = await _schedulerFactory.GetScheduler();
            await _scheduler.Start();

            // Trigger
            // Running 0:00 -> 23h00  everyday
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("triggerDaily", "group1")
                .WithCronSchedule("0 0 0,1,2,3,4,5,6,7,8,9,10,11,13,14,15,16,17,18,19,20,21,22,23 ? * * *")
                .ForJob("myJob", "group1")
                .Build();

            ITrigger trigger1 = TriggerBuilder.Create()
                  .WithIdentity("trigger_30_sec", "group")
                  .StartNow()
                  .WithSimpleSchedule(x => x
                      .WithIntervalInSeconds(30)
                      .RepeatForever())
              .Build();

            var startTime = new DateTimeOffset(2020, 11, 4, 13, 17, 10,
                                 new TimeSpan(0, 0, 0));
            ITrigger triggerAt = TriggerBuilder.Create()
                  .WithIdentity("triggerAt", "group")
                  .StartAt(startTime)

              .Build();

            ITrigger triggerCron = TriggerBuilder.Create()
                    .WithCronSchedule("0 45 13 1/1 * ? *")
                    .Build();

            ITrigger triggerNow = TriggerBuilder.Create()
                  .WithIdentity("trigger_Now", "group")
                  .StartNow()
              .Build();

            // List Job
            // EcusTS
            IJobDetail collectListEcusTS = JobBuilder.Create<CollectListEcusTS>()
                .WithIdentity("job1", "gtoup")
                .Build();

            //// DN
            //IJobDetail collectListDN = JobBuilder.Create<CollectListDN>()
            //    .WithIdentity("job1", "gtoup")
            //    .Build();

            //// Boom
            //IJobDetail collectListBoom = JobBuilder.Create<CollectListBoom>()
            //    .WithIdentity("job1", "gtoup")
            //    .Build();


            // Prepare the DI container

            var services = new ServiceCollection();
            // Register job
            services.AddTransient<CollectListEcusTS>();
            // Register job dependencies
            services.AddTransient<IEcusService, EcusService>();
            services.AddDbContext<COOContext>(options => options.UseSqlServer("data source=HVNN0606\\SQLEXPRESS;initial catalog=COO;user id=sa;password=123;MultipleActiveResultSets=True;"));

            var container = services.BuildServiceProvider();
            // Create an instance of the job factory
            var jobFactory = new DemoJobFactory(container);

            _scheduler.JobFactory = jobFactory;

            await _scheduler.ScheduleJob(collectListEcusTS, triggerNow, stoppingToken);

            //await _scheduler.ScheduleJob(collectListDN, trigger1, stoppingToken);
            //await _scheduler.ScheduleJob(collectListBoom, trigger1, stoppingToken);
        }
    }

}
