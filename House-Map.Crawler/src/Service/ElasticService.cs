using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System.Linq;
using HouseMap.Crawler.Common;
using HouseMap.Models;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;

namespace HouseMap.Crawler.Service
{

    public class ElasticService
    {
        AppSettings configuration;

        public ElasticService(IOptions<AppSettings> configuration)
        {
            this.configuration = configuration.Value;
        }


        public void SaveHouses(List<DBHouse> houses)
        {
            LogHelper.RunActionTaskNotThrowEx(() =>
            {
                var connSettings = new ConnectionSettings(new Uri(configuration.ESURL));
                var elasticClient = new ElasticClient(connSettings);
                if (houses == null || !houses.Any())
                {
                    return;
                }
                var houseIndex = $"house-data-{DateTime.Now.ToString("yyyy-MM-dd")}";
                var index = elasticClient.IndexExists(houseIndex);
                if (!index.Exists && index.IsValid)//判断索引是否存在和有效
                {
                    //创建索引
                    elasticClient.CreateIndex(houseIndex, i => i
                       .Settings(s => s.NumberOfShards(1).NumberOfReplicas(0))// 2是常量，阿里云只买了两个片
                       .Mappings(m => m.Map<DBHouse>(mm => mm.AutoMap()))
                       .Mappings(map => map.Map<DBHouse>(mm => mm)));
                }
                //批量创建索引和文档
                IBulkResponse bulkRs = elasticClient.IndexMany(houses, houseIndex);
                if (bulkRs.Errors)//如果异常
                {
                    LogHelper.Info("SaveHouses error,index:" + houseIndex + ",DebugInformation:" + bulkRs.DebugInformation);
                }
            }, "SaveHouses");

        }
    }



}