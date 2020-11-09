using COO.ServiceSAP;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace COO.ServiceSAP
{
    public class DS_BoomJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public DS_BoomJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService<CollectListDS_Boom>();
        }
        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
