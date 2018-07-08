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

        private HouseDapper houseDapper;


        public RefreshHouseCacheJob(HouseDapper houseDapper, RedisService redis)
        {
            //this.configuration = configuration.Value;
            this.houseDapper = houseDapper;
            this.redis = redis;
        }

        [Invoke(Begin = "2018-07-01 00:30", Interval = 1000 * 3500, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始RefreshHouseCacheJob...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var cityDashboards = houseDapper.GetHouseDashboard().GroupBy(d => d.CityName);
            foreach (var item in cityDashboards)
            {
                var search = new HouseSearchCondition() { CityName = item.Key, HouseCount = 600, IntervalDay = 14, Refresh = true };
                houseDapper.SearchHouses(search);
                foreach (var dashbord in item)
                {
                    search.HouseCount = 600;
                    search.Source = dashbord.Source;
                    houseDapper.SearchHouses(search);
                }
                for (var page = 0; page <= 5; page++)
                {
                    search.Source = "";
                    search.HouseCount = 180;
                    search.Page = page;
                    houseDapper.SearchHouses(search);
                }
            }

            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("RefreshHouseCacheJob结束，花费时间：" + copyTime);
        }
    }
}
