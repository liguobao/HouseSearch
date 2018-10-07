using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Collections.Generic;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler;
using HouseMap.Crawler.Service;
using HouseMap.Crawler.Common;
using HouseMap.Crawler.Jobs;
using HouseMap.Common;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace HouseMap.Crawler.Controllers
{
    public class CrawlerController : Controller
    {
        private readonly IServiceProvider _serviceProvider;

        public CrawlerController(
                             IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IActionResult Index(string source)
        {

            try
            {
                var crawler = _serviceProvider.GetServices<INewCrawler>().FirstOrDefault(c =>
                c.GetSource().GetSourceName() == source);
                if (crawler == null)
                {
                    return Json(new { success = true, error = $"{source} not found" });
                }
                LogHelper.RunActionTaskNotThrowEx(() =>
                {
                    crawler.Run();
                }, source);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = true, error = ex.ToString() });
            }

        }

        public IActionResult InitCCBConfig()
        {
            var configDapper = _serviceProvider.GetServices<ConfigDapper>().FirstOrDefault();

            using (StreamReader sr = new StreamReader(Path.Combine(AppContext.BaseDirectory, "Resources/CCBHouse.json")))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    //构建Json.net的读取流  
                    JsonReader reader = new JsonTextReader(sr);
                    //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                    var cities = serializer.Deserialize<JToken>(reader);
                    var configs = new List<DBConfig>();
                    foreach (var city in cities)
                    {
                        if (city["url"].Contains("java"))
                        {
                            continue;
                        }
                        var config = new DBConfig();
                        config.City = city["name"].ToString();
                        config.Id = Tools.GetUUId();
                        config.Json = "{'cityname': '" + city["name"].ToString() + "','shortcutname': '"
                        + city["url"].ToString().Replace("http://", "").Replace(".jiayuan.home.ccb.com", "")
                        + "','pagecount': 10,'APIKey':'cef8222092f74b95a8b24bc4a9e694a0'}";
                        config.PageCount = 30;
                        config.Source = "ccbhouse";
                        configs.Add(config);
                    }
                    configDapper.BulkInsert(configs);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return Json(new { success = true });

        }


    }
}
