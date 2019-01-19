using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseMap.Common;
using HouseMap.Crawler.Common;
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
using RestSharp;
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

        }

        private void InitRedis(IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexer, ConnectionMultiplexer>(factory =>
            {
                ConfigurationOptions options = ConfigurationOptions.Parse(Configuration["RedisConnectionString"]);
                options.SyncTimeout = 10 * 10000;
                return ConnectionMultiplexer.Connect(options);
            });
        }


        private void InitDB(IServiceCollection services)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            services.AddDbContextPool<HouseMapContext>(options =>
           {
               options.UseLoggerFactory(loggerFactory);
               options.UseMySql(Configuration["QCloudMySQL"].ToString());
           });
        }



        public void InitDI(IServiceCollection services)
        {
            #region Mapper
            services.AddScoped<ConfigDapper, ConfigDapper>();
            services.AddScoped<BaseDapper, BaseDapper>();
            services.AddScoped<BaseDapper, BaseDapper>();
            services.AddScoped<HouseDapper, HouseDapper>();

            #endregion Service

            #region Service
            services.AddScoped<EmailService, EmailService>();
            services.AddScoped<RedisTool, RedisTool>();
            services.AddScoped<HouseService, HouseService>();
            services.AddScoped<ElasticService, ElasticService>();
            services.AddScoped<ConfigService, ConfigService>();

            #endregion

            #region Crawler


            services.AddScoped<NewBaseCrawler, NewBaseCrawler>();
            services.AddScoped<INewCrawler, Zuber>();
            services.AddScoped<INewCrawler, Beike>();
            services.AddScoped<INewCrawler, DoubanWechat>();
            services.AddScoped<INewCrawler, Huzhu>();
            services.AddScoped<INewCrawler, Mogu>();
            services.AddScoped<INewCrawler, BaixingWechat>();
            services.AddScoped<INewCrawler, Douban>();
            services.AddScoped<INewCrawler, CCBHouse>();
            services.AddScoped<INewCrawler, PinPaiGongYu>();
            //services.AddScoped<INewCrawler, Xianyu>();
            services.AddScoped<INewCrawler, Fangduoduo>();
            services.AddScoped<INewCrawler, Fangtianxia>();
            services.AddScoped<INewCrawler, Hizhu>();
            services.AddScoped<INewCrawler, V2ex>();
            services.AddScoped<INewCrawler, Pinshiyou>();
            services.AddScoped<INewCrawler, Hezuzhaoshiyou>();
            services.AddScoped<INewCrawler, Baletu>();
            services.AddScoped<INewCrawler, Anjuke>();
            services.AddScoped<INewCrawler, ZiRoom>();

            #endregion

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            env.ConfigureNLog("nlog.config");
            app.UseStaticFiles();
            app.UseMvc(routes =>
           {
               routes.MapRoute(
                   name: "default",
                   template: "{controller=Test}/{action=Index}/{id?}");
           });
        }
    }
}
