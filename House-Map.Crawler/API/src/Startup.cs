using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseMap.Common;
using HouseMap.Dao;
using HouseMapAPI.Filters;
using HouseMapAPI.Service;
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

namespace HouseMapAPI
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
            //添加cors 服务
            services.AddCors(o => o.AddPolicy("APICors", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
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
            #region Dapper
            services.AddScoped<HouseDapper, HouseDapper>();
            services.AddScoped<UserCollectionDapper, UserCollectionDapper>();
            services.AddScoped<UserDapper, UserDapper>();
            services.AddScoped<BaseDapper, BaseDapper>();
            services.AddScoped<ConfigDapper, ConfigDapper>();
            services.AddScoped<NoticeDapper, NoticeDapper>();
            services.AddScoped<NewHouseDapper, NewHouseDapper>();

            #endregion


            //services.AddSingleton<EncryptionTools, EncryptionTools>();

            #region Service
            services.AddScoped<EmailService, EmailService>();
            services.AddScoped<RedisTool, RedisTool>();
            services.AddScoped<UserService, UserService>();
            services.AddScoped<UserHouseService, UserHouseService>();
            services.AddScoped<ConfigService, ConfigService>();
            services.AddScoped<HouseService, HouseService>();
            services.AddScoped<UserCollectionService, UserCollectionService>();
            services.AddScoped<QQOAuthClient, QQOAuthClient>();
            services.AddScoped<WeChatAppDecrypt, WeChatAppDecrypt>();
            services.AddScoped<UserTokenFilter, UserTokenFilter>();
            services.AddScoped<CrawlerConfigService, CrawlerConfigService>();

            #endregion
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            env.ConfigureNLog("nlog.config");
            app.UseErrorHandling();
            app.UseMvc();
        }
    }
}
