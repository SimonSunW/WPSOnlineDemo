using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using WPSApi.Core;
using WPSApi.Helper;

namespace WPSApi
{
    public class Startup
    {
        public ILoggerRepository LoggerRepository;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // 配置日志
            LoggerRepository = LogManager.CreateRepository("wps");
            XmlConfigurator.Configure(LoggerRepository, new FileInfo("log4net.config"));

            // 配置WPS的 AppId 和 AppSecretKey
            WPSSignatureHelper.Config(configuration["WPSConfig:AppId"], configuration["WPSConfig:AppSecretKey"]);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors(p =>
            {
                p.AddPolicy("wps", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("wps");
            app.UseMiddleware<LogMiddlware>();
            app.UseMvc();
        }
    }
}
