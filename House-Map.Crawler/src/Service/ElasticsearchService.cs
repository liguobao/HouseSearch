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

namespace HouseMap.Crawler.Service
{

    public class ElasticsearchService
    {
        AppSettings configuration;

        public ElasticsearchService(IOptions<AppSettings> configuration)
        {
            this.configuration = configuration.Value;
        }
        /// <summary>
        /// 添加房源到ES（新增或修改）
        /// </summary>
        /// <param name="listData">要添加的对象</param>
        /// <returns></returns>
        public void SaveHousesToES(List<HouseInfo> houses)
        {
            var connSettings = new ConnectionSettings(new Uri(configuration.ESURL))
            .BasicAuthentication(configuration.ESUserName, configuration.ESPassword);
            var elasticClient = new ElasticClient(connSettings);
            if (houses == null)
            {
                return;
            }

            var houseIndex = $"house-data";
            var index = elasticClient.IndexExists(houseIndex);
            if (!index.Exists && index.IsValid)//判断索引是否存在和有效
            {
                //创建索引
                elasticClient.CreateIndex(houseIndex, i => i
                   .Settings(s => s.NumberOfShards(2).NumberOfReplicas(0))// 2是常量，阿里云只买了两个片
                   .Mappings(m => m.Map<HouseInfo>(mm => mm.AutoMap()))
                   .Mappings(map => map.Map<HouseInfo>(mm => mm)));
            }
            //批量创建索引和文档
            IBulkResponse bulkRs = elasticClient.IndexMany(houses, houseIndex);
            if (bulkRs.Errors)//如果异常
            {
                LogHelper.Info("SaveHouses finish,index:" + houseIndex + ",DebugInformation:" + bulkRs.DebugInformation);
                //TODO
            }

        }
    }



}