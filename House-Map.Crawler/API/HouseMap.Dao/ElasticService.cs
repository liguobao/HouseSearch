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
        private readonly AppSettings _config;
        private readonly ElasticClient _elasticClient;

        public ElasticService(IOptions<AppSettings> configuration)
        {
            _config = configuration.Value;
            _elasticClient = InitElasticClient(configuration.Value);
        }


        private ElasticClient InitElasticClient(AppSettings config)
        {
            var connSettings = new ConnectionSettings(new Uri(config.ESURL));
            if (!string.IsNullOrEmpty(config.ESUserName)
            && !string.IsNullOrEmpty(config.ESPassword))
            {
                connSettings.BasicAuthentication(config.ESUserName, config.ESPassword);
            }
            connSettings.DisableDirectStreaming();
            return new ElasticClient(connSettings);
        }



        public List<DBHouse> Query(HouseCondition condition)
        {
            var searchRsp = _elasticClient.Search<DBHouse>(s => s
            .Index("house-data-*")
            .Explain()
            .From(condition.Page * condition.Page)
            .Size(condition.Size)
            .Sort(sort => sort.Descending(h => h.PubTime))
            .Query(q => ConvertToQuery(condition, q))
            );
            if (searchRsp.IsValid)
            {
                return searchRsp.Documents
                .GroupBy(h => h.OnlineURL).Select(items => items.FirstOrDefault()).ToList();
            }
            else
            {
                LogHelper.Info(searchRsp.DebugInformation);
            }
            return new List<DBHouse>();
        }

        public DBHouse QueryById(string id)
        {
            var searchRsp = _elasticClient.Search<DBHouse>(s => s
            .Index("house-data-*")
            .Explain()
            .Sort(sort => sort.Descending(h => h.PubTime))
            .Query(q => q.Match(m => m.Field("id.keyword").Query(id))));
            if (searchRsp.IsValid)
            {
                return searchRsp.Documents.FirstOrDefault();
            }
            Console.WriteLine(searchRsp.DebugInformation);
            return null;
        }

        private static QueryContainer ConvertToQuery(HouseCondition condition, QueryContainerDescriptor<DBHouse> q)
        {
            List<string> keywords = GetKeywords(condition);
            var qcList = keywords.Select(k => ConvertToQueryContainer(k)).ToArray();
            var query = q.Bool(b => b.Should(qcList).MinimumShouldMatch(1))
            && q.Match(m => m.Field(f => f.City).Query(condition.City));
            if (!string.IsNullOrEmpty(condition.Source))
            {
                query = query && q.Match(m => m.Field(f => f.Source).Query(condition.Source));
            }
            if (condition.FromPrice > 0 && condition.ToPrice > 0 && condition.ToPrice >= condition.ToPrice)
            {
                query = query && q.Range(r => r.Field(f => f.Price)
                .GreaterThanOrEquals(condition.FromPrice)
                .LessThanOrEquals(condition.ToPrice));
            }
            return query;
        }

        private static List<string> GetKeywords(HouseCondition condition)
        {
            var keywords = new List<string>();
            if (condition.Keyword.Contains(","))
            {
                keywords = condition.Keyword.Split(',').ToList();
            }
            else if (condition.Keyword.Contains("|"))
            {
                keywords = condition.Keyword.Split('|').ToList();
            }
            else
            {
                keywords.Add(condition.Keyword);
            }
            return keywords;
        }

        private static QueryContainer ConvertToQueryContainer(string word)
        {
            return new QueryContainerDescriptor<DBHouse>().MatchPhrase(m => m.Field(p => p.JsonData).Query(word));
        }

        public void SaveHouses(List<DBHouse> houses)
        {
            LogHelper.RunActionTaskNotThrowEx(() =>
            {
                if (houses == null || !houses.Any())
                {
                    return;
                }
                // Elasticsearch 跨index 无法插入自动去重更新,所以此处做按照发布时间分组
                foreach (var group in houses.GroupBy(h => h.PubTime.ToString("yyyy-MM")))
                {
                    var houseIndex = $"house-data-{group.Key}";
                    var index = _elasticClient.IndexExists(houseIndex);
                    if (!index.Exists && index.IsValid)//判断索引是否存在和有效
                    {
                        CreateIndex(houseIndex);
                        CreateMapping(houseIndex);
                    }
                    //批量创建索引和文档
                    IBulkResponse bulkRs = _elasticClient.IndexMany(group.ToList(), houseIndex);
                    if (bulkRs.Errors)//如果异常
                    {
                        LogHelper.Info("SaveHouses error,index:" + houseIndex + ",DebugInformation:" + bulkRs.DebugInformation);
                    }
                }
            }, "SaveHouses");

        }

        private void CreateMapping(string houseIndex)
        {
            var client = new RestClient($"{_config.ESURL}/{houseIndex}/dbhoused/_mapping");
            var request = new RestRequest(Method.PUT);
            request.AddParameter("application/json", "{\n        \"properties\": {\n            \"city\": {\n                \"type\": \"text\"\n            },\n            \"createTime\": {\n                \"type\": \"date\"\n            },\n            \"id\": {\n                \"type\": \"text\"\n            },\n            \"jsonData\": {\n                \"type\": \"text\",\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"labels\": {\n                \"type\": \"text\",\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"latitude\": {\n                \"type\": \"text\"\n            },\n            \"location\": {\n                \"type\": \"text\"\n            },\n            \"longitude\": {\n                \"type\": \"text\"\n            },\n            \"onlineURL\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"picURLs\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"pictures\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"price\": {\n                \"type\": \"long\"\n            },\n            \"pubTime\": {\n                \"type\": \"date\"\n            },\n            \"rentType\": {\n                \"type\": \"long\"\n            },\n            \"source\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                }\n            },\n            \"status\": {\n                \"type\": \"long\"\n            },\n            \"tags\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                },\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"text\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                },\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"title\": {\n                \"type\": \"text\",\n                \"fields\": {\n                    \"keyword\": {\n                        \"type\": \"keyword\",\n                        \"ignore_above\": 256\n                    }\n                },\n                \"analyzer\": \"ik_max_word\",\n                \"search_analyzer\": \"ik_max_word\"\n            },\n            \"updateTime\": {\n                \"type\": \"date\"\n            }\n        }\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        private void CreateIndex(string houseIndex)
        {
            var client = new RestClient($"{_config.ESURL}/{houseIndex}");
            var request = new RestRequest(Method.PUT);
            IRestResponse response = client.Execute(request);
        }
    }



}