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

        [Invoke(Begin = "2018-01-10 00:30", Interval = 1000 * 4800, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始RefreshHouseCacheJob...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var cityDashboards = houseDapper.GetHouseDashboard().GroupBy(d => d.CityName);
            foreach (var item in cityDashboards)
            {
                houseDapper.SearchHouses(item.Key, "", 500, 14, "", true);
                foreach (var dashbord in item)
                {
                    houseDapper.SearchHouses(item.Key, dashbord.Source, 500, 14, "", true);
                }
                for (var page = 0; page <= 5; page++)
                {
                    houseDapper.SearchHouses(item.Key, "", 140, 14, "", true, page);
                }
            }

            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("RefreshHouseCacheJob结束，花费时间：" + copyTime);
        }
    }
}
