using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Jobs
{
    public class RefreshHouseCacheJob : Job
    {

        private RedisService redis;

        private ConfigDapper config;

        private HouseDapper houseDapper;


        public RefreshHouseCacheJob(ConfigDapper configDapper, RedisService redis,HouseDapper houseDapper)
        {
            //this.configuration = configuration.Value;
            this.config = configDapper;
            this.redis = redis;
            this.houseDapper = houseDapper;
        }

        [Invoke(Begin = "2018-07-01 00:30", Interval = 1000 * 3500, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始RefreshHouseCacheJob...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var cityDashboards = config.GetDashboards().GroupBy(d => d.CityName);
            foreach (var item in cityDashboards)
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    //聚合房源的缓存,前600条数据
                    var search = new HouseSearchCondition() { CityName = item.Key, HouseCount = 600, IntervalDay = 14, Refresh = true };
                    houseDapper.SearchHouses(search);
                    foreach (var dashbord in item)
                    {
                        //每类房源的默认缓存,前600条数据
                        search.HouseCount = 600;
                        search.Source = dashbord.Source;
                        houseDapper.SearchHouses(search);

                        // 为小程序做的缓存,每次拉10条,一共20页
                        for (var page = 0; page <= 30; page++)
                        {
                            search.HouseCount = 20;
                            search.Source = dashbord.Source;
                            search.Page = page;
                            houseDapper.SearchHouses(search);
                        }
                    }
                    //为移动端做的缓存,每次拉180条,一共5页
                    for (var page = 0; page <= 5; page++)
                    {
                        search.Source = "";
                        search.HouseCount = 180;
                        search.Page = page;
                        houseDapper.SearchHouses(search);
                    }

                    //为小程序做的缓存,每次拉20条,一共30页
                    for (var page = 0; page <= 30; page++)
                    {
                        search.Source = "";
                        search.HouseCount = 20;
                        search.Page = page;
                        houseDapper.SearchHouses(search);
                    }

                }, "RefreshHouse");
            }

            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("RefreshHouseCacheJob结束，花费时间：" + copyTime);
        }
    }
}
