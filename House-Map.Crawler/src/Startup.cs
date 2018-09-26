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
        }

        public void InitDI(IServiceCollection services)
        {
            #region Mapper
            services.AddSingleton<HouseDapper, HouseDapper>();
            services.AddSingleton<ConfigDapper, ConfigDapper>();
            services.AddSingleton<HouseStatDapper, HouseStatDapper>();
            #endregion Service

            #region Service
            services.AddSingleton<EmailService, EmailService>();
            services.AddSingleton<RedisTool, RedisTool>();
            services.AddSingleton<HouseService, HouseService>();
            services.AddSingleton<HouseDashboardService, HouseDashboardService>();
            services.AddSingleton<ElasticsearchService, ElasticsearchService>();


            #endregion

            #region Jobs
            services.AddSingleton<BaiXingJob, BaiXingJob>();
            services.AddSingleton<DoubanCCBJob, DoubanCCBJob>();
            services.AddSingleton<GCJob, GCJob>();
            services.AddSingleton<HKSpaciousJob, HKSpaciousJob>();
            services.AddSingleton<PingPaiPeopleJob, PingPaiPeopleJob>();
            services.AddSingleton<RefreshDashboardJob, RefreshDashboardJob>();
            services.AddSingleton<TodayHouseDashboardJob, TodayHouseDashboardJob>();
            services.AddSingleton<ZuberMoguJob, ZuberMoguJob>();
            services.AddSingleton<RefreshHouseCacheJob, RefreshHouseCacheJob>();
            services.AddSingleton<RefreshHouseSourceJob, RefreshHouseSourceJob>();


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
            services.AddSingleton<BaiXingHouseCrawler, BaiXingHouseCrawler>();
            services.AddSingleton<BeikeHouseCrawler, BeikeHouseCrawler>();
            services.AddSingleton<ChengduZufangCrawler, ChengduZufangCrawler>();


            services.AddSingleton<BaseCrawler, BaseCrawler>();
            services.AddSingleton<DoubanCrawler, DoubanCrawler>();
            services.AddSingleton<DoubanCrawler, DoubanCrawler>();
            services.AddSingleton<SpaciousCrawler, SpaciousCrawler>();
            services.AddSingleton<MoGuCrawler, MoGuCrawler>();
            services.AddSingleton<HuzhuCrawler, HuzhuCrawler>();
            services.AddSingleton<PinPaiGongYuCrawler, PinPaiGongYuCrawler>();
            services.AddSingleton<ZuberCrawler, ZuberCrawler>();
            services.AddSingleton<BaixingCrawler, BaixingCrawler>();
            services.AddSingleton<BeikeCrawler, BeikeCrawler>();
            services.AddSingleton<ChengdufgjCrawler, ChengdufgjCrawler>();

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
