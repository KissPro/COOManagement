using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using COO.Application.MainFuction.EcusTS;
using COO.Data.EF;
using COO.Utilities.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using Serilog.Events;

namespace COO.Service
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
                //.UseWindowsService()
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

                    services.AddSingleton<IServiceCollection, ServiceCollection>();


                    services.AddHostedService<Worker>();
                });
    }
}
