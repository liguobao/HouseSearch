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
    public class UserCollectionDapper
    {
        protected static internal IDbConnection Connection => new MySqlConnection(ConnectionStrings.MySQLConnectionString);

        public static UserCollection InsertUser(UserCollection insertCollection)
        {
            using (IDbConnection dbConnection = Connection)
            {
                var collection = dbConnection.Query<UserCollection>(@"INSERT INTO `UserCollections` 
                (`UserID`,`HouseID`, `Source`, `HouseCity`)
                  VALUES (@UserID, @HouseID, @Source, @HouseCity);",
                insertCollection).FirstOrDefault();
                return collection;
            }
        }

        public static List<DBHouseInfo> FindUserCollections(long userID, string city = "", string source="")
        {

            if (string.IsNullOrEmpty(source))
            {
                var houses = new List<DBHouseInfo>();
                foreach (var key in ConstConfigurationName.HouseTableNameDic.Keys)
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

        private static List<DBHouseInfo> SearchUserCollections(long userID, string city, string source)
        {
            using (IDbConnection dbConnection = Connection)
            {
                var tableName = ConstConfigurationName.GetTableName(source);
                string sqlText = GetSQLText(city, tableName);
                var list = dbConnection.Query<DBHouseInfo>(sqlText,
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
                                    house.*
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

        public static List<HouseDashboard> LoadUserHouseDashboard(long userID)
        {
            using (IDbConnection dbConnection = Connection)
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