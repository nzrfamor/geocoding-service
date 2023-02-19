using Business.Services;
using Enyim.Caching.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeocodingApi
{
    public class Startup
    {
        private readonly ILogger logger;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var loggerFactory = LoggerFactory.Create(options => options.AddConsole());
            logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Initialization");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            logger.LogInformation("ConfigureServices called");

            services.AddControllers();
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
            services.AddHttpClient("GeocodingApi", c => c.BaseAddress = new Uri("https://maps.googleapis.com"));
            services.AddEnyimMemcached(setup => {
                setup.Servers.Add(new Server
                {
                    Address = "127.0.0.1",
                    Port = 11211
                });
            });
            services.AddScoped<IGeocodingApiService, GeocodingApiService>();
            services.AddSwaggerGen(c =>
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "Geocoding Service", Version = "v1" })
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            logger.LogInformation("Configure called");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "Geocoding Api V1")
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
