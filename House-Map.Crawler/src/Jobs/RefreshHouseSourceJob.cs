using HouseMap.Crawler.Common;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Service;
using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HouseMap.Crawler.Jobs
{
    public class RefreshHouseSourceJob : Job
    {

        private RedisService redis;

        private readonly HouseDapper _houseDapper;

        private readonly HouseStatDapper _statDapper;


        public RefreshHouseSourceJob(HouseDapper houseDapper, RedisService redis, HouseStatDapper statDapper)
        {
            //this.configuration = configuration.Value;
            this._houseDapper = houseDapper;
            this.redis = redis;
            _statDapper = statDapper;
        }

        [Invoke(Begin = "2018-07-01 00:30", Interval = 1000 * 3500, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始RefreshHouseSourceJob...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var cityDashboards = _statDapper.GetHouseDashboard().GroupBy(d => d.CityName);
            foreach (var item in cityDashboards)
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    //聚合房源的缓存,前600条数据
                    var search = new HouseCondition() { CityName = item.Key, HouseCount = 600, IntervalDay = 14, Refresh = true };
                    _houseDapper.SearchHouses(search);
                    foreach (var dashbord in item)
                    {
                        //每类房源的默认缓存,前600条数据
                        search.HouseCount = 600;
                        search.Source = dashbord.Source;
                        _houseDapper.SearchHouses(search);
                    }
                }, "RefreshHouse");
            }

            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("RefreshHouseSourceJob结束，花费时间：" + copyTime);
        }
    }
}
