using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.Data.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Serilog;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;

namespace COO.ServiceSAP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .CreateLogger();
            try
            {
                Log.Information("Starting up the service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the serivce");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    // 1.Local db
                    services.AddDbContext<COOContext>(options =>
                        options.UseSqlServer(configuration["ConnectionStrings:COO"]));

                    // 2. EcusTS db
                    //services.Configure<ConnectionStringSettings>("Hoang", configuration["ConnectionStrings:COO"]);
                    // Add DI
                    //services.AddTransient<IEcusService, EcusService>();
                    //services.AddTransient<CollectListEcusTS>();

                    // Add Quartz services
                    services.AddTransient<ISchedulerFactory, StdSchedulerFactory>();

                    services.AddTransient<IServiceCollection, ServiceCollection>();

                    services.AddHostedService<Worker>();
                });
    }
}