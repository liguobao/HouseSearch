using System;
using System.Linq;
using HouseMap.Crawler.Common;
using HouseMap.Crawler.Dapper;
using HouseMap.Crawler.Service;
using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;

namespace HouseMap.Crawler.Jobs
{
    public class SyncHousesToESJob : Job
    {
        ElasticsearchService elasticsearchService;

        HouseDapper houseDapper;

        public SyncHousesToESJob(ElasticsearchService elasticsearchService,
         HouseDapper houseDapper)
        {
            this.elasticsearchService = elasticsearchService;
            this.houseDapper = houseDapper;
        }

        [Invoke(Begin = "2018-07-01 03:50", Interval = 1000 * 3600 * 1, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionTaskNotThrowEx(() =>
            {
                //var houses = houseDapper.QueryByTimeSpan(DateTime.Now.Date.AddDays(-1), DateTime.Now);
                //elasticsearchService.SaveHousesToES(houses);
            }, "SyncHousesToESJob");

        }
    }
}
