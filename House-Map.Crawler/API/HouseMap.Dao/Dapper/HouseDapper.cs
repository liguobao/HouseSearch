using Dapper;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMap.Dao
{
    public class HouseDapper : BaseDapper
    {
        public HouseDapper(IOptions<AppSettings> options) : base(options)
        {
        }

        public int BulkInsertHouses(List<DBHouse> houses)
        {
            if (houses == null || houses.Count == 0)
            {
                return 0;
            }
            var tableName = SourceTool.GetHouseTableName(houses.FirstOrDefault().Source);
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute("INSERT INTO " + tableName + @"
                                     (`Title`, `Text`,
                                    `PicURLs`, `Location`,
                                    `City`,
                                    `Longitude`, `Latitude`,
                                    `RentType`,`Tags`,
                                    `PubTime`, `OnlineURL`,
                                     `Price`,`Labels`,
                                    `Source`,`Id`)
                                     VALUES (@Title, @Text,
                                            @PicURLs, @Location,
                                            @City,
                                            @Longitude,@Latitude,
                                            @RentType,@Tags,
                                            @PubTime,@OnlineURL,
                                            @Price,@Labels,
                                            @Source,@Id)  ON DUPLICATE KEY UPDATE UpdateTime=now();",
                                     houses, transaction: transaction);
                // dbConnection.Execute(@"INSERT INTO HouseData
                //         (`JsonData`,`Id`,`OnlineURL`)
                //         VALUES (@JsonData,@Id,@OnlineURL) ON DUPLICATE KEY UPDATE UpdateTime=now();",
                //         houses.Where(h => !string.IsNullOrEmpty(h.JsonData)), transaction: transaction);
                transaction.Commit();
                return result;
            }

        }


        public int BulkInsertPeoples(List<DBHouse> houses)
        {
            if (houses == null || houses.Count == 0)
            {
                return 0;
            }
            var tableName = SourceTool.GetHouseTableName(houses.FirstOrDefault().Source);
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute("INSERT INTO " + tableName + @"
                                     (`Title`, `Text`,
                                    `PicURLs`, `Location`,
                                    `City`,
                                    `Longitude`, `Latitude`,
                                    `RentType`,`Tags`,
                                    `PubTime`, `OnlineURL`,
                                     `Price`,`Labels`,
                                    `Source`,`Id`)
                                     VALUES (@Title, @Text,
                                            @PicURLs, @Location,
                                            @City,
                                            @Longitude,@Latitude,
                                            @RentType,@Tags,
                                            @PubTime,@OnlineURL,
                                            @Price,@Labels,
                                            @Source,@Id)  ON DUPLICATE KEY UPDATE UpdateTime=now();",
                                     houses, transaction: transaction);
                dbConnection.Execute(@"INSERT INTO HouseData
                        (`JsonData`,`Id`,`OnlineURL`)
                        VALUES (@JsonData,@Id,@OnlineURL) ON DUPLICATE KEY UPDATE UpdateTime=now();",
                        houses.Where(h => !string.IsNullOrEmpty(h.JsonData)), transaction: transaction);

                transaction.Commit();
                return result;
            }

        }




        public List<DBHouse> SearchHouses(DBHouseQuery condition)
        {
            try
            {
                var houses = new List<DBHouse>();
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    houses = dbConnection.Query<DBHouse>(condition.QueryText, condition).ToList();
                    return houses;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SearchHouses fail", ex, condition);
                return new List<DBHouse>();
            }
        }

        public DBHouse FindById(string houseId)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                foreach (var tableName in SourceTool.GetHouseTableNameDic().Values)
                {
                    var house = dbConnection.QueryFirstOrDefault<DBHouse>(@"SELECT Id,
                                            OnlineURL,
                                            Title,
                                            Location,
                                            Price,
                                            PubTime,
                                            City,
                                            Source,
                                            PicURLs,
                                            Labels,
                                            Tags,
                                            RentType,
                                            Latitude,
                                            Longitude,
                                            Text,
                                            Status"
                                            + $" from { tableName } where Id = @HouseId", new { HouseId = houseId });
                    if (house != null)
                    {
                        return house;
                    }
                }
                return null;
            }
        }


        public DBHouse FindByURL(string houseURL)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                foreach (var tableName in SourceTool.GetHouseTableNameDic().Values)
                {
                    var house = dbConnection.QueryFirstOrDefault<DBHouse>(@"SELECT Id,
                                            OnlineURL,
                                            Title,
                                            Location,
                                            Price,
                                            PubTime,
                                            City,
                                            Source,
                                            PicURLs,
                                            Labels,
                                            Tags,
                                            RentType,
                                            Latitude,
                                            Longitude,
                                            Text,
                                            Status"
                                            + $" from { tableName } where OnlineURL = @OnlineURL", new { OnlineURL = houseURL });
                    if (house != null)
                    {
                        return house;
                    }
                }
                return null;
            }
        }



        public int UpdateLngLat(HousesLatLng house)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                var tableName = SourceTool.GetHouseTableNameDic()[house.source];
                return dbConnection.Execute($"UPDATE {tableName} SET Longitude=@Longitude, Latitude=@Latitude,UpdateTime=now() WHERE Id=@Id;",
                new
                {
                    Longitude = house.longitude,
                    Latitude = house.latitude,
                    Id = house.id
                });
            }
        }


        public List<DBHouse> FindDoubanNotPriceData(int intervalDay = 14)
        {
            try
            {
                var houses = new List<DBHouse>();
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    houses = dbConnection.Query<DBHouse>(@"SELECT 
                        *
                    FROM
                        housemap.doubanhouse
                    WHERE
                        Price = - 1
                            AND UpdateTime > DATE_SUB(NOW(), INTERVAL @intervalDay DAY);", new { intervalDay = intervalDay }).ToList();
                    return houses;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("FindDoubanNotPriceData fail", ex);
                return new List<DBHouse>();
            }
        }

        public int UpdateDoubanPrices(List<DBHouse> houses)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    var sql = @"UPDATE `housemap`.`doubanhouse` SET `Price`=@Price WHERE `Id`=@Id;";
                    return dbConnection.Execute(sql, houses);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("FindDoubanNotPriceData fail", ex);
                return -1;
            }
        }
    }
}