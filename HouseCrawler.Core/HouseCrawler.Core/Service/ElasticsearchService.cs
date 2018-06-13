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
            var connSettings = new ConnectionSettings(new Uri("http://127.0.0.1:9200/"));
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
    }


}