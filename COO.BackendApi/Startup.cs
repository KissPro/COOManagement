using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.Application.Config.Config;
using COO.Application.Config.CountryShip;
using COO.Application.Config.Plant;
using COO.Application.MainFuction.BoomEcus;
using COO.Application.MainFuction.DeliverySale;
using COO.Data.EF;
using COO.Utilities.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace COO.BackendApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add connection string
            services.AddDbContext<COOContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(SystemConstants.MainConnectionString)));

            // Declare DI
            services.AddTransient<ICountryShipService, CountryShipService>();
            services.AddTransient<IBoomEcusService, BoomEcusService>();
            services.AddTransient<IDeliverySaleService, DeliverySaleService>();
            services.AddTransient<IDSManual, DSManual>();
            services.AddTransient<ICountryShipService, CountryShipService>();
            services.AddTransient<IConfigService, ConfigService>();
            services.AddTransient<IPlantService, PlantService>();

            // Add session config
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);//You can set Time   
            });

            // Swagger Config
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger COO Management", Version = "v1" });
            });

            // Related Class Prevent
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            // Cors
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                     builder =>
                                     {
                                         builder.WithOrigins("http://localhost:44400",
                                                             "http://coo-dev.fushan.fihnbb.com")
                                         .AllowAnyHeader()
                                         .AllowAnyOrigin()
                                         .AllowAnyMethod();
                                     });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSession();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger COO Management V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
