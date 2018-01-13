using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Text;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using HouseCrawler.Core.Common;

namespace HouseCrawler.Core
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<ConnectionStrings>(Configuration);
            services.AddTimedJob();
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

           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole();
            }

            //使用TimedJob
            app.UseTimedJob();

            app.UseStaticFiles();
           

            app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=House}/{action=Index}/{id?}"); 
            });

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            AppSettings.CityJsonFilePath = Path.Combine(env.WebRootPath, "DomainJS//pv.json");

            ConnectionStrings.MySQLConnectionString = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build()["ConnectionStrings:MySQLConnectionString"];

            AppSettings.DoubanAccount = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build()["DoubanAccount"];
            AppSettings.DoubanPassword = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build()["DoubanPassword"];

            DoubanHTTPHelper.InitCookieCollection();

            DomainProxyInfo.InitDomainProxyInfo(Path.Combine(env.WebRootPath, "availableProxy.json"));

          
        }
    }
}
