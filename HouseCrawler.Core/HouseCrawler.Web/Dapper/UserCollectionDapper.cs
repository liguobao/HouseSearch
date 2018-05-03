using Dapper;
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
                (`UserID`,`HouseID`, `HouseSource`, `City`)
                  VALUES (@UserID, @HouseID, @HouseSource, @City);",
                insertCollection).FirstOrDefault();
                return collection;
            }
        }

        public static UserInfo FindUserByActivatedCode(string activatedCode)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"SELECT * FROM housecrawler.UserInfos 
                where (ActivatedCode = @ActivatedCode) ;",new { ActivatedCode = activatedCode }).FirstOrDefault();
            }
        }

    }
}