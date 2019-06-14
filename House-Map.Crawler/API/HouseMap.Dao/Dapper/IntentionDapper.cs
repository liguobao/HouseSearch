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
    public class IntentionDapper : BaseDapper
    {
        public IntentionDapper(IOptions<AppSettings> options) : base(options)
        {
        }


        public int BulkInsertPeoples(List<DBPeopleIntention> intentions)
        {
            if (intentions == null || intentions.Count == 0)
            {
                return 0;
            }
            var tableName = SourceTool.GetHouseTableName(intentions.FirstOrDefault().Source);
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
                                     intentions, transaction: transaction);
                dbConnection.Execute(@"INSERT INTO HouseData
                        (`JsonData`,`Id`,`OnlineURL`)
                        VALUES (@JsonData,@Id,@OnlineURL) ON DUPLICATE KEY UPDATE UpdateTime=now();",
                        intentions.Where(h => !string.IsNullOrEmpty(h.JsonData)), transaction: transaction);

                transaction.Commit();
                return result;
            }

        }




        public List<DBHouse> SearchHouses(HouseCondition condition)
        {
            var houses = new List<DBHouse>();
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                houses = dbConnection.Query<DBHouse>(condition.QueryText, condition).ToList();
                return houses;
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


        public int UpdateLngLat(DBHouse house)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                var tableName = SourceTool.GetHouseTableNameDic()[house.Source];
                return dbConnection.Execute($"UPDATE {tableName} SET Longitude=@Longitude, Latitude=@Latitude,UpdateTime=now() WHERE Id=@Id;",
                new
                {
                    Longitude = house.Longitude,
                    Latitude = house.Latitude,
                    Id = house.Id
                });
            }
        }



        public DBHouse FindByOnlineURL(string onlineURL)
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
                                            + $" from { tableName } where OnlineURL = @OnlineURL", new { OnlineURL = onlineURL });
                    if (house != null)
                    {
                        return house;
                    }
                }
                return null;
            }
        }
    }
}