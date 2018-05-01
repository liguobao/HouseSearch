using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 添加 Cook 服务
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/LogIn";
                options.LogoutPath = "/Account/LogOff";
            });
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

           var config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
            ConnectionStrings.MySQLConnectionString = config["ConnectionStrings:MySQLConnectionString"];
            ConnectionStrings.RedisConnectionString = config["ConnectionStrings:RedisConnectionString"];
            EncryptionConfig.CIV = config["EncryptionConfig:CIV"];
            EncryptionConfig.CKEY = config["EncryptionConfig:CKEY"];
            EmailConfig.Account = config["EmailConfig:Account"];
            EmailConfig.Password = config["EmailConfig:Password"];

        }
    }
}
