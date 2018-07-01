using Dapper;
using HouseCrawler.Web.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseCrawler.Web
{
    public class HouseDapper
    {

        private APPConfiguration configuration;

        private RedisService redisService;
        public HouseDapper(IOptions<APPConfiguration> configuration, RedisService redisService)
        {
            this.configuration = configuration.Value;
            this.redisService = redisService;
        }

        private IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.MySQLConnectionString);
        }

        public IEnumerable<HouseInfo> SearchHouses(string cityName, string source = "",
          int houseCount = 300, int intervalDay = 7, string keyword = "",
           bool refresh = false, int page = 0)
        {
            if (string.IsNullOrEmpty(source))
            {
                var houseList = new List<HouseInfo>();
                // 因为会走几个表,默认每个表取N条
                var houseSources = GetCityHouseSources(cityName);
                var limitCount = houseCount / houseSources.Count;
                foreach (var houseSource in houseSources)
                {
                    //建荣家园数据质量比较差,默认不出
                    if (houseSource == ConstConfigurationName.CCBHouse)
                    {
                        continue;
                    }
                    houseList.AddRange(Search(cityName, houseSource, limitCount, intervalDay, keyword, refresh, page));
                }
                return houseList.OrderByDescending(h => h.PubTime);
            }
            else
            {
                return Search(cityName, source, houseCount, intervalDay, keyword, refresh, page);
            }

        }

        public IEnumerable<HouseInfo> Search(string cityName, string source = "",
            int houseCount = 300, int intervalDay = 7, string keyword = "",
            bool refresh = false, int page = 0)
        {
            string redisKey = $"{cityName}-{source}-{intervalDay}-{houseCount}-{keyword}-{page}";
            var houses = new List<HouseInfo>();
            if (!refresh)
            {
                houses = redisService.ReadSearchCache(redisKey);
                if (houses != null && houses.Count > 0)
                {
                    return houses;
                }
            }
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                string search_SQL = $"SELECT * from { ConstConfigurationName.GetTableName(source)} where 1=1 " +
                    $"and LocationCityName = @LocationCityName and  PubTime >= @PubTime";
                if (!string.IsNullOrEmpty(keyword))
                {
                    search_SQL = search_SQL + " and (HouseText like @KeyWord or HouseLocation like @KeyWord) ";
                }
                search_SQL = search_SQL + $" order by PubTime desc limit {houseCount * page}, {houseCount} ";
                houses = dbConnection.Query<HouseInfo>(search_SQL,
                    new
                    {
                        LocationCityName = cityName,
                        KeyWord = $"%{keyword}%",
                        PubTime = DateTime.Now.Date.AddDays(-intervalDay)
                    }).ToList();
                if (houses != null && houses.Count > 0)
                {
                    redisService.WriteSearchCache(redisKey, houses);
                }
                return houses;
            }
        }



        public HouseInfo GetHouseID(long houseID, string source)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();

                return dbConnection.Query<HouseInfo>($"SELECT * FROM {ConstConfigurationName.GetTableName(source)} where ID = @ID",
                  new
                  {
                      ID = houseID
                  }).FirstOrDefault();
            }
        }





        public List<Models.HouseDashboard> GetHouseDashboard()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                var list = new List<Models.HouseDashboard>();
                foreach (var key in ConstConfigurationName.HouseTableNameDic.Keys)
                {
                    var tableName = ConstConfigurationName.GetTableName(key);
                    var dashboards = dbConnection.Query<HouseDashboard>(@"SELECT 
                                LocationCityName AS CityName,
                                Source, COUNT(id) AS HouseSum, 
                                MAX(PubTime) AS LastRecordPubTime
                            FROM 
                                " + tableName + $" GROUP BY LocationCityName, Source ORDER BY HouseSum desc;");
                    list.AddRange(dashboards);
                }
                return list.Where(dash => dash.LastRecordPubTime.CompareTo(DateTime.Now.AddDays(-30)) > 0)
                .ToList();
            }
        }


        public List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = redisService.ReadCache("HouseDashboard");
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = GetHouseDashboard();
                redisService.WriteCache("HouseDashboard", Newtonsoft.Json.JsonConvert.SerializeObject(dashboards));
                return dashboards;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(houseDashboardJson);
            }
        }

        public List<string> GetCityHouseSources(string cityName)
        {
            string redisKey = "CitySource-" + cityName;
            string citySources = redisService.ReadCache(redisKey);
            if (string.IsNullOrEmpty(citySources))
            {
                var dicCityNameToSources = LoadDashboard().GroupBy(d => d.CityName)
                          .ToDictionary(item => item.Key, item => item.Select(db => db.Source).ToList());
                var soures = new List<String>();
                if (dicCityNameToSources != null && dicCityNameToSources.ContainsKey(cityName))
                {
                    soures = dicCityNameToSources[cityName];
                }
                redisService.WriteCache("redisKey", Newtonsoft.Json.JsonConvert.SerializeObject(soures));
                return soures;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(citySources);
            }


        }
    }
}
