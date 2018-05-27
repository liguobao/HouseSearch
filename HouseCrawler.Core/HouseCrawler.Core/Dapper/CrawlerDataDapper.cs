using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using HouseCrawler.Core.Models;
using MySql.Data.MySqlClient;

namespace HouseCrawler.Core
{
    public class HouseDataDapper
    {


        static internal IDbConnection Connection => new MySqlConnection(ConnectionStrings.MySQLConnectionString);

        public static void BulkInsertHouses(List<BaseHouseInfo> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            var tableName = ConstConfigurationName.GetTableName(list.FirstOrDefault().Source);
            using (IDbConnection dbConnection = Connection)
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


        }

        public static List<HouseDashboard> GetHouseDashboard()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var list = new List<HouseDashboard>();
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

        public static IEnumerable<BaseHouseInfo> SearchHouseInfo(string cityName, string source = "",
         int houseCount = 100, int intervalDay = 7, string keyword = "", bool refresh = false)
        {
            if (string.IsNullOrEmpty(source))
            {
                var houseList = new List<BaseHouseInfo>();
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

        public static IEnumerable<BaseHouseInfo> Search(string cityName, string source = "",
            int houseCount = 100, int intervalDay = 7, string keyword = "", bool refresh = false)
        {
            string redisKey = $"{cityName}-{source}-{intervalDay}-{houseCount}-{keyword}";
            var houses = new List<BaseHouseInfo>();
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
                string search_SQL = $"SELECT * from { ConstConfigurationName.GetTableName(source)} where 1=1 " +
                    $"and LocationCityName = @LocationCityName and  PubTime >= @PubTime";
                if (!string.IsNullOrEmpty(keyword))
                {
                    search_SQL = search_SQL + " and (HouseText like @KeyWord or HouseLocation like @KeyWord or HouseTitle like @KeyWord) ";
                }
                search_SQL = search_SQL + $" order by PubTime desc limit {houseCount} ";
                houses = dbConnection.Query<BaseHouseInfo>(search_SQL,
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


        public static List<BizCrawlerConfiguration> GetConfigurationList(string configurationName)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<BizCrawlerConfiguration>(@"SELECT * FROM housecrawler.CrawlerConfigurations 
                where ConfigurationName=@ConfigurationName;", new
                {
                    ConfigurationName = configurationName
                }).ToList();
            }
        }


        public static List<HouseStat> GetHouseStatList(int intervalDay = 1)
        {
            List<HouseStat> houseStatList = new List<HouseStat>();
            DateTime today = DateTime.Now;

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                foreach (var tableName in ConstConfigurationName.HouseTableNameDic.Values)
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
    }
}
