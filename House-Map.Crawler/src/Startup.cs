using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseMap.Common;
using HouseMap.Crawler.Common;
using HouseMap.Crawler.Jobs;
using HouseMap.Crawler.Service;
using HouseMap.Dao;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Web;

namespace HouseMap.Crawler
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOptions().Configure<AppSettings>(Configuration);
            InitDI(services);
            InitDB(services);
        }

         private void InitDB(IServiceCollection services)
        {
            services.AddDbContext<HouseDataContext>(options =>
            {
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                options.UseLoggerFactory(loggerFactory);
                options.UseMySql(Configuration["MySQLConnectionString"].ToString());
            });
        }

        public void InitDI(IServiceCollection services)
        {
            #region Mapper
            services.AddScoped<HouseDapper, HouseDapper>();
            services.AddScoped<ConfigDapper, ConfigDapper>();
            services.AddScoped<HouseStatDapper, HouseStatDapper>();
            #endregion Service

            #region Service
            services.AddScoped<EmailService, EmailService>();
            services.AddScoped<RedisTool, RedisTool>();
            services.AddScoped<HouseService, HouseService>();
            services.AddScoped<ElasticsearchService, ElasticsearchService>();


            #endregion

            #region Jobs
            services.AddScoped<BaiXingJob, BaiXingJob>();
            services.AddScoped<DoubanCCBJob, DoubanCCBJob>();
            services.AddScoped<GCJob, GCJob>();
            services.AddScoped<HKSpaciousJob, HKSpaciousJob>();
            services.AddScoped<PingPaiPeopleJob, PingPaiPeopleJob>();
            services.AddScoped<TodayHouseDashboardJob, TodayHouseDashboardJob>();
            services.AddScoped<ZuberMoguJob, ZuberMoguJob>();
            services.AddScoped<RefreshHouseCacheJob, RefreshHouseCacheJob>();
            services.AddScoped<RefreshHouseSourceJob, RefreshHouseSourceJob>();


            #endregion

            #region Crawler
            services.AddScoped<CCBHouesCrawler, CCBHouesCrawler>();
            services.AddScoped<DoubanHouseCrawler, DoubanHouseCrawler>();
            services.AddScoped<DoubanHouseCrawler, DoubanHouseCrawler>();
            services.AddScoped<HKSpaciousCrawler, HKSpaciousCrawler>();
            services.AddScoped<MoGuHouseCrawler, MoGuHouseCrawler>();
            services.AddScoped<PeopleRentingCrawler, PeopleRentingCrawler>();
            services.AddScoped<PinPaiGongYuHouseCrawler, PinPaiGongYuHouseCrawler>();
            services.AddScoped<ZuberHouseCrawler, ZuberHouseCrawler>();
            services.AddScoped<BaiXingHouseCrawler, BaiXingHouseCrawler>();
            services.AddScoped<BeikeHouseCrawler, BeikeHouseCrawler>();
            services.AddScoped<ChengduZufangCrawler, ChengduZufangCrawler>();


            services.AddScoped<BaseCrawler, BaseCrawler>();
            services.AddScoped<DoubanCrawler, DoubanCrawler>();
            services.AddScoped<DoubanCrawler, DoubanCrawler>();
            services.AddScoped<SpaciousCrawler, SpaciousCrawler>();
            services.AddScoped<MoGuCrawler, MoGuCrawler>();
            services.AddScoped<HuzhuCrawler, HuzhuCrawler>();
            services.AddScoped<PinPaiGongYuCrawler, PinPaiGongYuCrawler>();
            services.AddScoped<ZuberCrawler, ZuberCrawler>();
            services.AddScoped<BaixingCrawler, BaixingCrawler>();
            services.AddScoped<BeikeCrawler, BeikeCrawler>();
            services.AddScoped<ChengdufgjCrawler, ChengdufgjCrawler>();

            #endregion
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            env.ConfigureNLog("nlog.config");
            app.UseMvc(routes =>
           {
               routes.MapRoute(
                   name: "default",
                   template: "{controller=Test}/{action=Index}/{id?}");
           });
        }
    }
}
