using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Elasticsearch.Net;
using HouseCrawler.Core;
using Microsoft.Extensions.Options;
using Nest;
using System.Linq;

namespace HouseCrawler.Core.Service
{

    public class ElasticsearchService
    {
        APPConfiguration configuration;

        public ElasticsearchService(IOptions<APPConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }
        /// <summary>
        /// 添加房源到ES（新增或修改）
        /// </summary>
        /// <param name="listData">要添加的对象</param>
        /// <returns></returns>
        public void SaveHousesToES(List<BaseHouseInfo> houses)
        {
            var connSettings = new ConnectionSettings(new Uri(configuration.ESURL))
            .BasicAuthentication(configuration.ESUserName, configuration.ESPassword);
            var elasticClient = new ElasticClient(connSettings);
            if (houses == null)
            {
                return;
            }
            foreach (var groupitem in houses.GroupBy(h => h.PubDate))
            {
                var houseIndex = $"house-{groupitem.Key.ToString("yyyy-MM-dd")}";
                var index = elasticClient.IndexExists(houseIndex);
                if (!index.Exists && index.IsValid)//判断索引是否存在和有效
                {
                    //创建索引
                    elasticClient.CreateIndex(houseIndex, i => i
                       .Settings(s => s.NumberOfShards(2).NumberOfReplicas(0))// 2是常量，阿里云只买了两个片
                       .Mappings(m => m.Map<BaseHouseInfo>(mm => mm.AutoMap()))
                       .Mappings(map => map.Map<BaseHouseInfo>(mm => mm)));
                }
                //批量创建索引和文档
                IBulkResponse bulkRs = elasticClient.IndexMany(groupitem, houseIndex);
                if (bulkRs.Errors)//如果异常
                {
                    LogHelper.Info("SaveHouses finish,index:" + houseIndex + ",DebugInformation:" + bulkRs.DebugInformation);
                    //TODO
                }
            };
        }
    }



}