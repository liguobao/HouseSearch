using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace HouseCrawler.Core.DataContent
{
    public class CrawlerDataDapper
    {
        private static Dictionary<String, String> dicHouseTableName = new Dictionary<string, string>() {
            { ConstConfigurationName.Douban, "DoubanHouseInfos"},
            { ConstConfigurationName.HuZhuZuFang, "MutualHouseInfos"},
            { ConstConfigurationName.PinPaiGongYu, "ApartmentHouseInfos"},
            { ConstConfigurationName.CCBHouse, "CCBHouseInfos"},
            { ConstConfigurationName.Zuber, "ZuberHouseInfos"},
            { ConstConfigurationName.MoguHouse, "MoguHouseInfos"}
        };

        static internal IDbConnection Connection => new MySqlConnection(ConnectionStrings.MySQLConnectionString);

        public static void BulkInsertHouses(List<BaseHouseInfo> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            var tableName = GetTableName(list.FirstOrDefault().Source);
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

        public static List<DBHouseDashboard> GetHouseDashboardList()
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

        public static List<BaseHouseInfo> GetHouse(int intervalDay)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var apartment = dbConnection.Query<BaseHouseInfo>(@"SELECT HouseOnlineURL,HouseLocation 
                                FROM housecrawler.ApartmentHouseInfos 
                                where PubTime >=@PubTime order by PubTime desc;",
                                new { PubTime = DateTime.Now.Date.AddDays(-intervalDay) });


                var douban = dbConnection.Query<BaseHouseInfo>(@"SELECT HouseOnlineURL,HouseLocation 
                                FROM housecrawler.DoubanHouseInfos 
                                where PubTime >=@PubTime order by PubTime desc;",
                                new { PubTime = DateTime.Now.Date.AddDays(-intervalDay) });

                var mutual = dbConnection.Query<BaseHouseInfo>(@"SELECT HouseOnlineURL,HouseLocation 
                                FROM housecrawler.MutualHouseInfos 
                                where PubTime >=@PubTime order by PubTime desc;",
                                new { PubTime = DateTime.Now.Date.AddDays(-intervalDay) });

                var ccb = dbConnection.Query<BaseHouseInfo>(@"SELECT HouseOnlineURL,HouseLocation 
                                FROM housecrawler.CCBHouseInfos 
                                where PubTime >=@PubTime order by PubTime desc;",
                                new { PubTime = DateTime.Now.Date.AddDays(-intervalDay) });
                var list = apartment.ToList();
                list.AddRange(douban.ToList());
                list.AddRange(mutual.ToList());
                list.AddRange(ccb);
                return list;

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
    }
}
