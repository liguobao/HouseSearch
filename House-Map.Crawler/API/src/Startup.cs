using System.IO;
using HouseMap.Common;
using HouseMap.Dao;
using HouseMapAPI.Filters;
using HouseMapAPI.Service;
using HouseMapConsumer.Dao;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;

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
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(FieldsFilterAttribute)); // by type
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOptions().Configure<AppSettings>(Configuration);
            InitRedis(services);
            InitDI(services);
            InitDB(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "house-api", Version = "v1" });
                c.EnableAnnotations();
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "HouseMapAPI.xml");
                c.IncludeXmlComments(filePath);

            });
            //添加cors 服务
            services.AddCors(o => o.AddPolicy("APICors", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        }

        private void InitDB(IServiceCollection services)
        {
            services.AddDbContextPool<HouseMapContext>(options =>
            {
                options.UseMySql(Configuration["QCloudMySQL"].ToString());
            });
        }

        private void InitRedis(IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexer, ConnectionMultiplexer>(factory =>
            {
                ConfigurationOptions options = ConfigurationOptions.Parse(Configuration["RedisConnectionString"]);
                options.SyncTimeout = 10 * 1000;
                return ConnectionMultiplexer.Connect(options);
            });
        }



        public void InitDI(IServiceCollection services)
        {
            #region Dapper
            services.AddScoped<BaseDapper, BaseDapper>();
            services.AddScoped<ConfigDapper, ConfigDapper>();
            services.AddScoped<NoticeDapper, NoticeDapper>();
            services.AddScoped<HouseDapper, HouseDapper>();
            services.AddScoped<HouseMongoMapper, HouseMongoMapper>();
            services.AddScoped<MongoDBMapper, MongoDBMapper>();

            #endregion


            //services.AddSingleton<EncryptionTools, EncryptionTools>();

            #region Service
            services.AddScoped<EmailService, EmailService>();
            services.AddScoped<RedisTool, RedisTool>();

            services.AddScoped<CollectionService, CollectionService>();
            services.AddScoped<UserService, UserService>();
            services.AddScoped<UserHouseService, UserHouseService>();
            services.AddScoped<ConfigService, ConfigService>();
            services.AddScoped<HouseService, HouseService>();
            services.AddScoped<QQOAuthClient, QQOAuthClient>();
            services.AddScoped<WeChatAppDecrypt, WeChatAppDecrypt>();
            services.AddScoped<UserTokenFilter, UserTokenFilter>();
            services.AddScoped<FieldsFilterAttribute, FieldsFilterAttribute>();
            services.AddScoped<DBGroupService, DBGroupService>();
            services.AddScoped<ElasticService, ElasticService>();
            services.AddScoped<NoticeService, NoticeService>();


            #endregion
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint("/house-api/v1/swagger.json", "house-api");
                c.DefaultModelRendering(ModelRendering.Model);
            });
            app.UseErrorHandling();
            app.UseMvc();
        }
    }
}
