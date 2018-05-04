using Dapper;
using HouseCrawler.Web.DataContent;
using HouseCrawler.Web.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseCrawler.Web
{
    public class HouseDapper
    {

        protected static internal IDbConnection Connection => new MySqlConnection(ConnectionStrings.MySQLConnectionString);


        public static IEnumerable<DBHouseInfo> SearchHouseInfo(string cityName, string source = "",
          int houseCount = 100, int intervalDay = 7, string keyword = "", bool refresh = false)
        {
            if (string.IsNullOrEmpty(source))
            {
                var houseList = new List<DBHouseInfo>();
                foreach (var key in ConstConfigurationName.HouseTableNameDic.Keys)
                {
                    // 因为会走几个表,默认每个表取100条
                    houseList.AddRange(Search(cityName, key, 100, intervalDay, keyword, refresh));
                }
                return houseList.OrderByDescending(h => h.PubTime).Take(houseCount);
            }
            else
            {
                return Search(cityName, source, houseCount, intervalDay, keyword, refresh);
            }

        }

        public static IEnumerable<DBHouseInfo> Search(string cityName, string source = "",
            int houseCount = 100, int intervalDay = 7, string keyword = "", bool refresh = false)
        {
            string redisKey = $"{cityName}-{source}-{intervalDay}-{houseCount}-{keyword}";
            var houses = new List<DBHouseInfo>();
            if (!refresh)
            {
                houses = RedisService.ReadSearchCache(redisKey);
                if (houses != null)
                {
                    return houses;
                }
            }
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                string search_SQL = $"SELECT * from { GetTableName(source)} where 1=1 " +
                    $"and LocationCityName = @LocationCityName and  PubTime >= @PubTime";
                if (!string.IsNullOrEmpty(keyword))
                {
                    search_SQL = search_SQL + " and (HouseText like @KeyWord or HouseLocation like @KeyWord or HouseTitle like @KeyWord) ";
                }
                search_SQL = search_SQL + $" order by PubTime desc limit {houseCount} ";
                houses = dbConnection.Query<DBHouseInfo>(search_SQL,
                    new
                    {
                        LocationCityName = cityName,
                        KeyWord = $"%{keyword}%",
                        PubTime = DateTime.Now.Date.AddDays(-intervalDay)
                    }).ToList();
                RedisService.WriteSearchCache(redisKey, houses);
                return houses;
            }
        }



        public static DBHouseInfo GetHouseID(long houseID, string source)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                return dbConnection.Query<DBHouseInfo>($"SELECT * FROM {GetTableName(source)} where ID = @ID",
                  new
                  {
                      ID = houseID
                  }).FirstOrDefault();
            }
        }

        private static string GetTableName(string source)
        {
            if (ConstConfigurationName.HouseTableNameDic.ContainsKey(source))
            {
                return ConstConfigurationName.HouseTableNameDic[source];
            }

            return "HouseInfos";
        }



        public static List<Models.HouseDashboard> GetHouseDashboard()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var list = new List<Models.HouseDashboard>();
                foreach (var key in ConstConfigurationName.HouseTableNameDic.Keys)
                {
                    var tableName = GetTableName(key);
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

    }
}
