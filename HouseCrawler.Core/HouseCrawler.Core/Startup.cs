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
using HouseCrawler.Core.Service;

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
            services.AddOptions().Configure<APPConfiguration>(Configuration);
            InitDI(services);
            services.AddTimedJob();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void InitDI(IServiceCollection services)
        {
            services.AddSingleton<HouseDapper, HouseDapper>();

            #region Service
            services.AddSingleton<EmailService, EmailService>();
            services.AddSingleton<RedisService, RedisService>();
            services.AddSingleton<HouseDashboardService, HouseDashboardService>();
            #endregion

            #region Jobs
            services.AddSingleton<CrawlerJobs, CrawlerJobs>();
            services.AddSingleton<HKSpaciousCrawlerJob, HKSpaciousCrawlerJob>();
            services.AddSingleton<HouseDashboardJob, HouseDashboardJob>();
            services.AddSingleton<RefreshDashboardJob, RefreshDashboardJob>();
            #endregion

            #region Crawler
            services.AddSingleton<CCBHouesCrawler, CCBHouesCrawler>();
            services.AddSingleton<DoubanHouseCrawler, DoubanHouseCrawler>();
            services.AddSingleton<DoubanHouseCrawler, DoubanHouseCrawler>();
            services.AddSingleton<HKSpaciousCrawler, HKSpaciousCrawler>();
            services.AddSingleton<MoGuHouseCrawler, MoGuHouseCrawler>();
            services.AddSingleton<PeopleRentingCrawler, PeopleRentingCrawler>();
            services.AddSingleton<PinPaiGongYuHouseCrawler, PinPaiGongYuHouseCrawler>();
            services.AddSingleton<ZuberHouseCrawler, ZuberHouseCrawler>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //add NLog to .NET Core
            loggerFactory.AddNLog();
            //needed for non-NETSTANDARD platforms: configure nlog.config in your project root. NB: you need NLog.Web.AspNetCore package for this. 
            env.ConfigureNLog("Resources/nlog.config");

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

            ConnectionStrings.MySQLConnectionString = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build()["ConnectionStrings:MySQLConnectionString"];

            ConnectionStrings.RedisConnectionString = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json").Build()["ConnectionStrings:RedisConnectionString"];

            ConnectionStrings.CCBHomeAPIKey = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build()["CCBHomeAPIKey"];
        }
    }
}
