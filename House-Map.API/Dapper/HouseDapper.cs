using Dapper;
using HouseMapAPI.Common;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Models;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMapAPI.Dapper
{
    public class HouseDapper : BaseDapper
    {
        public HouseDapper(IOptions<AppSettings> configuration, RedisService redisService)
        : base(configuration, redisService)
        {
        }


        public IEnumerable<HouseInfo> SearchHouses(HouseSearchCondition condition)
        {
            if (string.IsNullOrEmpty(condition.Source))
            {
                var houseList = new List<HouseInfo>();
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
        public IEnumerable<HouseInfo> Search(HouseSearchCondition condition)
        {
            string redisKey = condition.RedisKey;
            var houses = new List<HouseInfo>();
            if (!condition.Refresh)
            {
                houses = _redisService.ReadCache<List<HouseInfo>>(redisKey, RedisKey.Houses.DBName);
                if (houses != null && houses.Count > 0)
                {
                    return houses;
                }
            }
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                houses = dbConnection.Query<HouseInfo>(condition.QueryText, condition).ToList();
                if (houses != null && houses.Count > 0)
                {
                    _redisService.WriteObject(redisKey, houses);
                }
                return houses;
            }
        }



        public HouseInfo GetHouseID(long houseID, string source)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();

                return dbConnection.Query<HouseInfo>($"SELECT * FROM {ConstConfigName.GetTableName(source)} where ID = @ID",
                  new
                  {
                      ID = houseID
                  }).FirstOrDefault();
            }
        }





        public List<HouseDashboard> GetHouseDashboard()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                var list = new List<HouseDashboard>();
                foreach (var key in ConstConfigName.HouseTableNameDic.Keys)
                {
                    var tableName = ConstConfigName.GetTableName(key);
                    var dashboards = dbConnection.Query<HouseDashboard>(@"SELECT 
                                LocationCityName AS CityName,
                                Source, COUNT(id) AS HouseSum, 
                                MAX(PubTime) AS LastRecordPubTime
                            FROM 
                                " + tableName + $" GROUP BY LocationCityName, Source ORDER BY HouseSum desc;");
                    list.AddRange(dashboards);
                }
                return list.Where(dash => dash.LastRecordPubTime.CompareTo(DateTime.Now.AddDays(-30)) > 0 && dash.HouseSum > 100)
                .ToList();
            }
        }


        public List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = _redisService.ReadCache(RedisKey.HouseDashboard.Key,
             RedisKey.HouseDashboard.DBName);
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = GetHouseDashboard();
                _redisService.WriteObject(RedisKey.HouseDashboard.Key, dashboards);
                return dashboards;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(houseDashboardJson);
            }
        }

        public List<string> GetCityHouseSources(string cityName)
        {
            string redisKey = RedisKey.CityHouseSource.Key + cityName;
            var citySources = _redisService.ReadCache<List<string>>(redisKey, RedisKey.CityHouseSource.DBName);
            if (citySources != null)
            {
                return citySources;
            }

            var dicCityNameToSources = LoadDashboard().GroupBy(d => d.CityName)
                      .ToDictionary(item => item.Key, item => item.Select(db => db.Source).ToList());
            var soures = new List<String>();
            if (dicCityNameToSources != null && dicCityNameToSources.ContainsKey(cityName))
            {
                soures = dicCityNameToSources[cityName];
            }
            _redisService.WriteObject(redisKey, soures);
            return soures;
        }
    }
}
