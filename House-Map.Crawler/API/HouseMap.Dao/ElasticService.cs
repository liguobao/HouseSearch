using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System.Linq;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using System.Reflection;
using RestSharp;

namespace HouseMap.Dao
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
                // Elasticsearch 跨index 无法插入自动去重更新,所以此处做按照发布时间分组
                foreach (var group in houses.GroupBy(h => h.PubTime.ToString("yyyy-MM-dd")))
                {
                    var houseIndex = $"house-data-{group.Key}";
                    var index = elasticClient.IndexExists(houseIndex);
                    if (!index.Exists && index.IsValid)//判断索引是否存在和有效
                    {
                        CreateIndex(houseIndex);
                        CreateMapping(houseIndex);
                    }
                    //批量创建索引和文档
                    IBulkResponse bulkRs = elasticClient.IndexMany(group.ToList(), houseIndex);
                    if (bulkRs.Errors)//如果异常
                    {
                        LogHelper.Info("SaveHouses error,index:" + houseIndex + ",DebugInformation:" + bulkRs.DebugInformation);
                    }
                }
            }, "SaveHouses");

        }

        private void CreateMapping(string houseIndex)
        {
            var client = new RestClient($"{configuration.ESURL}/{houseIndex}/dbhoused/_mapping");
            var request = new RestRequest(Method.PUT);
            request.AddParameter("application/json", "{\n        \"properties\": {\n            \"city\": {\n                \"type\": \"text\"\n            },\n            \"createTime\": {\n                \"type\": \"date\"\n            },\n            \"id\": {\n                \"type\": \"text\"\n            },\n            \"jsonData\": {\n                \"type\": \"text\",\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"labels\": {\n                \"type\": \"text\",\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"latitude\": {\n                \"type\": \"text\"\n            },\n            \"location\": {\n                \"type\": \"text\"\n            },\n            \"longitude\": {\n                \"type\": \"text\"\n            },\n            \"onlineURL\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"picURLs\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"pictures\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"price\": {\n                \"type\": \"long\"\n            },\n            \"pubTime\": {\n                \"type\": \"date\"\n            },\n            \"rentType\": {\n                \"type\": \"long\"\n            },\n            \"source\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"status\": {\n                \"type\": \"long\"\n            },\n            \"tags\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                },\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"text\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                },\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"title\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                },\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"updateTime\": {\n                \"type\": \"date\"\n            }\n        }\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        private void CreateIndex(string houseIndex)
        {
            var client = new RestClient($"{configuration.ESURL}/{houseIndex}");
            var request = new RestRequest(Method.PUT);
            IRestResponse response = client.Execute(request);
        }
    }



}