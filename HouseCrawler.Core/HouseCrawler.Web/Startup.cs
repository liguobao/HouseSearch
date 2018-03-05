using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace HouseCrawler.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<ConnectionStrings>(Configuration);
            //services.AddTimedJob();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        { 
            //add NLog to .NET Core
            loggerFactory.AddNLog();

            //Enable ASP.NET Core features (NLog.web) - only needed for ASP.NET Core users
            app.AddNLogWeb();

            //needed for non-NETSTANDARD platforms: configure nlog.config in your project root. NB: you need NLog.Web.AspNetCore package for this. 

            env.ConfigureNLog("./wwwroot/nlog.config");

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            //使用TimedJob
            // app.UseTimedJob();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            AppSettings.CityJsonFilePath = Path.Combine(env.WebRootPath, "DomainJS//pv.json");

            ConnectionStrings.MySQLConnectionString = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build()["ConnectionStrings:MySQLConnectionString"];
        }
    }
}
