using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Elasticsearch.Net;
using HouseCrawler.Core;
using Microsoft.Extensions.Options;
using Nest;

namespace HouseCrawler.Core.Service
{

    public class ElasticsearchService
    {
        public static void AddHouse(List<BaseHouseInfo> houses)
        {
            var connSettings = new ConnectionSettings(new Uri("http://118.25.130.101:9200/"));
            var elasticClient = new ElasticClient(connSettings);
            elasticClient.CreateIndex("default", i => i.Mappings(m => m.Map<BaseHouseInfo>(ms => ms.AutoMap())));
            if (elasticClient != null)
            {
                foreach (var house in houses)
                {
                    elasticClient.Index(house, i => i.Index("default").Type("house").Id(house.HouseOnlineURL).Refresh(Refresh.True));
                }

            }
        }



        /// <summary>
        /// 添加房源ES索引（新增或修改）
        /// </summary>
        /// <param name="listData">要添加的对象</param>
        /// <returns></returns>
        public static bool SaveHouses(List<BaseHouseInfo> houses)
        {
            var connSettings = new ConnectionSettings(new Uri("http://118.25.130.101:9200/"));
            var elasticClient = new ElasticClient(connSettings);
            if (houses == null)
                throw new Exception("参数对象集合必须有值");
            var houseIndex = $"house-{DateTime.Now.Date.ToString("yyyy-MM-dd")}";
            var index = elasticClient.IndexExists(houseIndex);
            if (!index.Exists && index.IsValid)//判断索引是否存在和有效
            {
                //创建索引
                elasticClient.CreateIndex(houseIndex, i => i
                   .Settings(s => s.NumberOfShards(2).NumberOfReplicas(0))// 2是常量，阿里云只买了两个片
                   .Mappings(m => m.Map<BaseHouseInfo>(mm => mm.AutoMap())));
            }
            //批量创建索引和文档
            IBulkResponse bulkRs = elasticClient.IndexMany(houses, houseIndex);
            if (bulkRs.Errors)//如果异常
            {
                LogHelper.Info("SaveHouses error,ex:" + bulkRs.ToString());
                //TODO
            }
            return bulkRs.Errors;
        }
    }



}