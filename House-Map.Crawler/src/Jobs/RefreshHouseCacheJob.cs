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
using HouseMap.Common;

namespace HouseMap.Crawler.Jobs
{
    public class RefreshHouseCacheJob : Job
    {

        private HouseService _houseService;

        private ConfigService _configService;




        public RefreshHouseCacheJob(HouseService houseService, ConfigService configService)
        {
            _houseService = houseService;
            _configService = configService;
        }

        [Invoke(Begin = "2018-07-01 00:30", Interval = 1000 * 3500, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始RefreshHouseCacheJob...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var cityDashboards = _configService.LoadCitySources();
            foreach (var item in cityDashboards)
            {
                //聚合房源的缓存,前600条数据
                var search = new HouseCondition() { CityName = item.Key, HouseCount = 600, IntervalDay = 14, Refresh = true };
                _houseService.Search(search);
                foreach (var dashboard in item.Value)
                {
                    //每类房源的默认缓存,前600条数据
                    search.HouseCount = 600;
                    search.Source = dashboard.Source;
                    _houseService.Search(search);

                    // 为小程序做的缓存,每次拉10条,一共20页
                    for (var page = 0; page <= 30; page++)
                    {
                        search.HouseCount = 20;
                        search.Source = dashboard.Source;
                        search.Page = page;
                        _houseService.Search(search);
                    }
                }
                //为移动端做的缓存,每次拉180条,一共10页
                for (var page = 0; page <= 10; page++)
                {
                    search.Source = "";
                    search.HouseCount = 180;
                    search.Page = page;
                    _houseService.Search(search);
                }

                //为小程序做的缓存,每次拉20条,一共30页
                for (var page = 0; page <= 30; page++)
                {
                    search.Source = "";
                    search.HouseCount = 20;
                    search.Page = page;
                    _houseService.Search(search);
                }
            }

            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("RefreshHouseCacheJob结束，花费时间：" + copyTime);
        }
    }
}
