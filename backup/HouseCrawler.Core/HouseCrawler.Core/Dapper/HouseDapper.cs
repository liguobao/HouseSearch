using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Service;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace HouseCrawler.Core
{
    public class HouseDapper
    {
        private APPConfiguration configuration;

        private RedisService redis;

        private ElasticsearchService elasticsearchService;

        private ConfigDapper _configMap;

        public HouseDapper(IOptions<APPConfiguration> configuration, RedisService redis,
        ElasticsearchService elasticsearchService,ConfigDapper configMap)
        {
            this.configuration = configuration.Value;
            this.redis = redis;
            this.elasticsearchService = elasticsearchService;
            this._configMap = configMap;
        }

        private IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.MySQLConnectionString);
        }


        public void BulkInsertHouses(List<BaseHouseInfo> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            var tableName = ConstConfigName.GetTableName(list.FirstOrDefault().Source);
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute("INSERT INTO " + tableName + @" (`HouseTitle`, `HouseOnlineURL`, 
                                    `HouseLocation`, `DisPlayPrice`, 
                                    `PubTime`, `HousePrice`, 
                                    `LocationCityName`,
                                    `Source`,
                                    `HouseText`, 
                                    `IsAnalyzed`, 
                                    `Status`,`PicURLs`) 
                                     VALUES (@HouseTitle, @HouseOnlineURL,
                                            @HouseLocation, @DisPlayPrice,
                                            @PubTime, @HousePrice,
                                            @LocationCityName,
                                            @Source,
                                            @HouseText,
                                            @IsAnalyzed,
                                            @Status,@PicURLs)  ON DUPLICATE KEY UPDATE DataChange_LastTime=now();",
                                     list, transaction: transaction);
                transaction.Commit();
            }
            LogHelper.RunActionTaskNotThrowEx(() =>
            {
                elasticsearchService.SaveHousesToES(list);
            }, "SaveHousesToES");

        }

        public List<BaseHouseInfo> QueryByTimeSpan(DateTime fromTime, DateTime toTime)
        {
            var houseList = new List<BaseHouseInfo>();
            foreach (var key in ConstConfigName.HouseTableNameDic.Keys)
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    string search_SQL = $"SELECT * from { ConstConfigName.GetTableName(key)} where " +
                        $" PubTime BETWEEN @FromTime AND @ToTime ";
                    search_SQL = search_SQL + $" order by PubTime desc";
                    var houses = dbConnection.Query<BaseHouseInfo>(search_SQL,
                        new
                        {
                            FromTime = fromTime,
                            ToTime = toTime
                        });
                    houseList.AddRange(houses);
                }
            }
            return houseList;
        }


        public IEnumerable<BaseHouseInfo> SearchHouses(HouseSearchCondition condition)
        {
            if (string.IsNullOrEmpty(condition.Source))
            {
                var houseList = new List<BaseHouseInfo>();
                // 因为会走几个表,默认每个表取N条
                var houseSources = GetCityHouseSources(condition.CityName);
                var limitCount = condition.HouseCount / houseSources.Count;
                foreach (var houseSource in houseSources)
                {
                    //建荣家园数据质量比较差,默认不出
                    if (houseSource == ConstConfigName.CCBHouse)
                    {
                        continue;
                    }
                    condition.Source = houseSource;
                    condition.HouseCount = limitCount;
                    houseList.AddRange(Search(condition));
                }
                return houseList.OrderByDescending(h => h.PubTime);
            }
            else
            {
                return Search(condition);
            }

        }
        public IEnumerable<BaseHouseInfo> Search(HouseSearchCondition condition)
        {
            string redisKey = condition.RedisKey;
            var houses = new List<BaseHouseInfo>();
            if (!condition.Refresh)
            {
                houses = redis.ReadSearchCache(redisKey);
                if (houses != null && houses.Count > 0)
                {
                    return houses;
                }
            }
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                houses = dbConnection.Query<BaseHouseInfo>(condition.QueryText, condition).ToList();
                if (houses != null && houses.Count > 0)
                {
                    redis.WriteSearchCache(redisKey, houses);
                }
                return houses;
            }
        }


        public List<HouseStat> GetHouseStatList(int intervalDay = 1)
        {
            List<HouseStat> houseStatList = new List<HouseStat>();
            DateTime today = DateTime.Now;

            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                foreach (var tableName in ConstConfigName.HouseTableNameDic.Values)
                {
                    houseStatList.AddRange(dbConnection.Query<HouseStat>(@"SELECT COUNT(*) AS HouseSum,
                    MAX(PubTime) AS LastPubTime,
                    MAX(DataCreateTime) AS LastCreateTime,
                    Source 
                    FROM " + tableName + " WHERE DataCreateTime BETWEEN @FromTime AND @ToTime;", new
                    {
                        FromTime = today.AddDays(-intervalDay).Date,
                        ToTime = today
                    }).ToList());
                }
                return houseStatList;

            }
        }


        public List<string> GetCityHouseSources(string cityName)
        {
            string redisKey = "CitySource-" + cityName;
            string citySources = redis.ReadCache(redisKey);
            if (string.IsNullOrEmpty(citySources))
            {
                var dicCityNameToSources = _configMap.GetDashboards().GroupBy(d => d.CityName)
                          .ToDictionary(item => item.Key, item => item.Select(db => db.Source).ToList());
                var soures = new List<String>();
                if (dicCityNameToSources != null && dicCityNameToSources.ContainsKey(cityName))
                {
                    soures = dicCityNameToSources[cityName];
                }
                redis.WriteCache(redisKey, Newtonsoft.Json.JsonConvert.SerializeObject(soures));
                return soures;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(citySources);
            }


        }




    }
}
