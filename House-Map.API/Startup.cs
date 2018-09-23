using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseMapAPI.Common;
using HouseMapAPI.Dapper;
using HouseMapAPI.DBEntity;
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
            services.AddSingleton<HouseDapper, HouseDapper>();
            services.AddSingleton<ConfigDapper, ConfigDapper>();
            services.AddSingleton<UserCollectionDapper, UserCollectionDapper>();
            services.AddSingleton<UserDapper, UserDapper>();
            services.AddSingleton<BaseDapper, BaseDapper>();
            services.AddSingleton<NoticeDapper, NoticeDapper>();


            #endregion


            //services.AddSingleton<EncryptionTools, EncryptionTools>();

            #region Service
            services.AddSingleton<EmailService, EmailService>();
            services.AddSingleton<RedisService, RedisService>();
            services.AddSingleton<DashboardService, DashboardService>();
            services.AddSingleton<UserService, UserService>();
            services.AddSingleton<ConfigService, ConfigService>();
            services.AddSingleton<HouseService, HouseService>();
            services.AddSingleton<UserCollectionService, UserCollectionService>();
            services.AddSingleton<QQOAuthClient, QQOAuthClient>();
            services.AddSingleton<WeChatAppDecrypt, WeChatAppDecrypt>();
            services.AddScoped<UserTokenFilter, UserTokenFilter>();

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
