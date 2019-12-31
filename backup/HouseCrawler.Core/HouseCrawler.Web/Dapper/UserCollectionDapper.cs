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
    public class UserCollectionDapper
    {
        private APPConfiguration configuration;

        private RedisService redisService;
        public UserCollectionDapper(IOptions<APPConfiguration> configuration, RedisService redisService)
        {
            this.configuration = configuration.Value;
            this.redisService = redisService;
        }

        private IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.MySQLConnectionString);
        }
    

        public UserCollection InsertUser(UserCollection insertCollection)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                var collection = dbConnection.Query<UserCollection>(@"INSERT INTO `UserCollections` 
                (`UserID`,`HouseID`, `Source`, `HouseCity`)
                  VALUES (@UserID, @HouseID, @Source, @HouseCity) 
                  ON DUPLICATE KEY UPDATE DataChange_LastTime=now();",
                insertCollection).FirstOrDefault();
                return collection;
            }
        }


        public  UserCollection FindByIDAndUserID(long id, long userID)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                var collection = dbConnection.Query<UserCollection>(@"
                SELECT 
                    *
                FROM
                    housecrawler.UserCollections
                WHERE
                    ID = @ID AND UserID = @UserID;",
                new { ID = id, UserID = userID }).FirstOrDefault();
                return collection;
            }
        }

        public void RemoveByIDAndUserID(long id, long userID)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Query<UserCollection>(@"
                DELETE FROM `housecrawler`.`UserCollections` 
                WHERE 
                    ID = @ID AND UserID = @UserID;",
                new { ID = id, UserID = userID }).FirstOrDefault();
            }
        }

        public List<HouseInfo> FindUserCollections(long userID, string city = "", string source = "")
        {

            if (string.IsNullOrEmpty(source))
            {
                var houses = new List<HouseInfo>();
                foreach (var key in ConstConfigName.HouseTableNameDic.Keys)
                {
                    houses.AddRange(SearchUserCollections(userID, city, key));
                }

                return houses;
            }
            else
            {
                return SearchUserCollections(userID, city, source);
            }
        }

        private List<HouseInfo> SearchUserCollections(long userID, string city, string source)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                var tableName = ConstConfigName.GetTableName(source);
                string sqlText = GetSQLText(city, tableName);
                var list = dbConnection.Query<HouseInfo>(sqlText,
                    new
                    {
                        UserID = userID,
                        HouseCity = city,
                        Source = source
                    }).ToList();
                return list;
            }


        }

        private static string GetSQLText(string city, string tableName)
        {

            string sqlText = @"SELECT 
                                house.HouseTitle,
                                house.HouseOnlineURL,
                                house.HouseLocation,
                                house.DisPlayPrice,
                                house.PubTime,
                                house.HousePrice,
                                house.LocationCityName,
                                house.Source,
                                house.DataCreateTime,
                                uc.ID
                                FROM
                                    UserCollections uc
                                        JOIN
                               " + tableName + @" house ON uc.HouseID = house.ID
                                        AND uc.Source = house.Source
                                WHERE
                                    uc.UserID = @UserID ";
            if (!string.IsNullOrEmpty(city))
            {
                sqlText = sqlText + " AND uc.HouseCity =@HouseCity ";
            }
            return sqlText;
        }

        public List<HouseDashboard> LoadUserHouseDashboard(long userID)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<HouseDashboard>(@"
                        SELECT 
                            HouseCity AS CityName,
                            Source,
                            count(*) as HouseSum
                        FROM
                            UserCollections where UserID = @UserID
                        GROUP BY HouseCity , source;", new { UserID = userID }).ToList();
            }
        }

    }
}