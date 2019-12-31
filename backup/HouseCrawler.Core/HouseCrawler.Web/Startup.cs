using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HouseCrawler.Web.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using StackExchange.Redis;

namespace HouseCrawler.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddXmlSerializerFormatters();
            services.AddOptions().Configure<APPConfiguration>(Configuration);
            InitDI(services);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Configuration["RedisConnectionString"]);
            services.AddDataProtection();
            services.Configure<KeyManagementOptions>(o =>
            {
                o.XmlRepository = new RedisXmlRepository(() => redis.GetDatabase(), "ShareCookie");
            });
            // 添加 Cook 服务
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/LogOff";
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            //添加cors 服务
            services.AddCors(o => o.AddPolicy("APICors", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        }



        public void InitDI(IServiceCollection services)
        {
            #region Dapper
            services.AddSingleton<HouseDapper, HouseDapper>();
            services.AddSingleton<ConfigDapper, ConfigDapper>();
            services.AddSingleton<UserCollectionDapper, UserCollectionDapper>();
            services.AddSingleton<UserDataDapper, UserDataDapper>();
            services.AddSingleton<BaseDapper, BaseDapper>();
            services.AddSingleton<NoticeDapper, NoticeDapper>();


            #endregion


            services.AddSingleton<EncryptionTools, EncryptionTools>();

            #region Service
            services.AddSingleton<EmailService, EmailService>();
            services.AddSingleton<RedisService, RedisService>();
            services.AddSingleton<HouseDashboardService, HouseDashboardService>();
            services.AddSingleton<UserService, UserService>();
            services.AddSingleton<QQOAuthClient, QQOAuthClient>();
            #endregion

            #region Jobs

            #endregion

            #region Crawler

            services.AddSingleton<DoubanHouseCrawler, DoubanHouseCrawler>();
            services.AddSingleton<DoubanHouseCrawler, DoubanHouseCrawler>();
            services.AddSingleton<PeopleRentingCrawler, PeopleRentingCrawler>();
            services.AddSingleton<PinPaiGongYuHouseCrawler, PinPaiGongYuHouseCrawler>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //add NLog to .NET Core
            loggerFactory.AddNLog();

            //needed for non-NETSTANDARD platforms: configure nlog.config in your project root. NB: you need NLog.Web.AspNetCore package for this. 
            env.ConfigureNLog("./wwwroot/nlog.config");


            loggerFactory.AddConsole();

            app.UseAuthentication();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors("APICors");

        }

    }
}
