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

        public static List<DBHouseInfo> FindUserCollections(long userID)
        {
            using (IDbConnection dbConnection = Connection)
            {

                var  houses = new List<DBHouseInfo>();
                dbConnection.Open();
                foreach (var tableName in ConstConfigurationName.HouseTableNameDic.Values)
                {
                    var list = dbConnection.Query<DBHouseInfo>(@"SELECT 
                                    house.*
                                FROM
                                    UserCollections uc
                                        JOIN
                                    "+ tableName+ @" house ON uc.HouseID = house.ID
                                        AND uc.Source = house.Source
                                WHERE
                                    uc.UserID = @UserID;", new { UserID = userID});
                   houses.AddRange(list);
                }

                return houses;
            }
        }

    }
}