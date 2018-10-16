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
using SkyWalking.AspNetCore;
using SkyWalking.Diagnostics.EntityFrameworkCore;
using SkyWalking.Diagnostics.HttpClient;
using SkyWalking.Diagnostics.SqlClient;
using StackExchange.Redis;

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
            InitRedis(services);
            InitDI(services);
            InitDB(services);
            InitSkyWalking(services);
        }

        private void InitRedis(IServiceCollection services)
        {
            services.AddScoped<ConnectionMultiplexer, ConnectionMultiplexer>(factory =>
            {
                ConfigurationOptions options = ConfigurationOptions.Parse(Configuration["RedisConnectionString"]);
                options.SyncTimeout = 10 * 1000;
                return ConnectionMultiplexer.Connect(options);
            });
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

        private void InitSkyWalking(IServiceCollection services)
        {
            services.AddSkyWalking(option =>
            {
                option.ApplicationCode = Configuration["AppName"]?.ToString();
                option.DirectServers = Configuration["SkyWalkingURL"]?.ToString();
                // 每三秒采样的Trace数量,-1 为全部采集
                option.SamplePer3Secs = -1;
            }).AddEntityFrameworkCore(c => { c.AddPomeloMysql().AddNpgsql(); })
            .AddSqlClient()
            .AddHttpClient();
        }

        public void InitDI(IServiceCollection services)
        {
            #region Mapper
            services.AddScoped<HouseDapper, HouseDapper>();
            services.AddScoped<HouseStatDapper, HouseStatDapper>();
            services.AddScoped<ConfigDapper, ConfigDapper>();
            services.AddScoped<NewBaseDapper, NewBaseDapper>();
            services.AddScoped<BaseDapper, BaseDapper>();
            services.AddScoped<NewHouseDapper, NewHouseDapper>();

            #endregion Service

            #region Service
            services.AddScoped<EmailService, EmailService>();
            services.AddScoped<RedisTool, RedisTool>();
            services.AddScoped<HouseService, HouseService>();
            services.AddScoped<ElasticsearchService, ElasticsearchService>();
            services.AddScoped<ConfigService, ConfigService>();

            #endregion

            #region Crawler

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

            services.AddScoped<ICrawler, DoubanCrawler>();
            services.AddScoped<ICrawler, DoubanCrawler>();
            services.AddScoped<ICrawler, SpaciousCrawler>();
            services.AddScoped<ICrawler, MoGuCrawler>();
            services.AddScoped<ICrawler, HuzhuCrawler>();
            services.AddScoped<ICrawler, PinPaiGongYuCrawler>();
            services.AddScoped<ICrawler, ZuberCrawler>();
            services.AddScoped<ICrawler, BaixingCrawler>();
            services.AddScoped<ICrawler, BeikeCrawler>();
            services.AddScoped<ICrawler, ChengdufgjCrawler>();


            services.AddScoped<NewBaseCrawler, NewBaseCrawler>();
            services.AddScoped<INewCrawler, Zuber>();
            services.AddScoped<INewCrawler, Beike>();
            services.AddScoped<INewCrawler, DoubanWechat>();
            services.AddScoped<INewCrawler, Huzhu>();
            services.AddScoped<INewCrawler, Mogu>();
            services.AddScoped<INewCrawler, BaixingWechat>();
            services.AddScoped<INewCrawler, Douban>();
            services.AddScoped<INewCrawler, CCBHouse>();
            #endregion

            services.AddScoped<TodayHouseDashboardJob, TodayHouseDashboardJob>();

            services.AddScoped<RefreshHouseCacheJob, RefreshHouseCacheJob>();
            services.AddScoped<RefreshHouseSourceJob, RefreshHouseSourceJob>();


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
