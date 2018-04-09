using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace HouseCrawler.Core.DataContent
{
    public class CrawlerDataDapper
    {
        static internal IDbConnection Connection => new MySqlConnection(ConnectionStrings.MySQLConnectionString);


        public static List<DBHouseDashboard> GetHouseSourceInfoList()
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
    }
}
