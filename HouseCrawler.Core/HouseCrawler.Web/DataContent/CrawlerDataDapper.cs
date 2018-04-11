using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseCrawler.Web.DataContent
{
    public class CrawlerDataDapper
    {

        private static Dictionary<String, String> dicHouseTableName = new Dictionary<string, string>() {
            { ConstConfigurationName.Douban, "DoubanHouseInfos"},
            { ConstConfigurationName.HuZhuZuFang, "MutualHouseInfos"},
            { ConstConfigurationName.PinPaiGongYu, "ApartmentHouseInfos"},
            { ConstConfigurationName.CCBHouse, "CCBHouseInfos"}
        };

        protected static internal IDbConnection Connection => new MySqlConnection(ConnectionStrings.MySQLConnectionString);


        public static IEnumerable<DBHouseInfo> SearchHouseInfo(string cityName, string source = "",
          int houseCount = 100, int intervalDay = 7, string keyword = "", bool refresh = false)
        {
            if (string.IsNullOrEmpty(source))
            {
                var houseList = new List<DBHouseInfo>();
                foreach (var key in dicHouseTableName.Keys)
                {
                    houseList.AddRange(Search(cityName, key, houseCount, intervalDay, keyword, refresh));
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




        public static DBHouseInfo GetHouseByOnlineURL(string onlineURL)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                foreach (var key in dicHouseTableName.Keys)
                {
                    var house = dbConnection.Query<DBHouseInfo>($"SELECT * FROM {GetTableName(key)} where HouseOnlineURL = @HouseOnlineURL",
                     new
                     {
                         HouseOnlineURL = onlineURL
                     }).FirstOrDefault();
                    if (house != null)
                    {
                        return house;
                    }
                }
                return null;
            }
        }


        private static string GetTableName(string source)
        {
            if (dicHouseTableName.ContainsKey(source))
            {
                return dicHouseTableName[source];
            }

            return "HouseInfos";
        }



        public static List<DBHouseDashboard> GetHouseDashboard()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var apartment = dbConnection.Query<DBHouseDashboard>(@"SELECT 
                                LocationCityName AS CityName, Source, COUNT(id) AS HouseSum
                            FROM
                                housecrawler.ApartmentHouseInfos
                            GROUP BY LocationCityName, Source;");


                var douban = dbConnection.Query<DBHouseDashboard>(@"SELECT 
                                LocationCityName AS CityName, Source, COUNT(id) AS HouseSum
                            FROM
                                housecrawler.DoubanHouseInfos
                            GROUP BY LocationCityName, Source;");

                var mutual = dbConnection.Query<DBHouseDashboard>(@"SELECT 
                                LocationCityName AS CityName, Source, COUNT(id) AS HouseSum
                            FROM
                                housecrawler.MutualHouseInfos
                            GROUP BY LocationCityName, Source;");
                var list = apartment.ToList();
                list.AddRange(douban.ToList());
                list.AddRange(mutual.ToList());
                return list;

            }
        }

    }
}
