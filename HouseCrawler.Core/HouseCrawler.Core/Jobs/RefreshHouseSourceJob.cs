using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Jobs
{
    public class RefreshHouseSourceJob : Job
    {

        private RedisService redis;

        private HouseDapper houseDapper;

        private ConfigDapper configDapper;


        public RefreshHouseSourceJob(HouseDapper houseDapper, RedisService redis,ConfigDapper configDapper)
        {
            //this.configuration = configuration.Value;
            this.houseDapper = houseDapper;
            this.redis = redis;
            this.configDapper = configDapper;
        }

        [Invoke(Begin = "2018-07-01 00:30", Interval = 1000 * 3500, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始RefreshHouseSourceJob...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var cityDashboards = configDapper.GetDashboards().GroupBy(d => d.CityName);
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
                    }
                }, "RefreshHouse");
            }

            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("RefreshHouseSourceJob结束，花费时间：" + copyTime);
        }
    }
}
