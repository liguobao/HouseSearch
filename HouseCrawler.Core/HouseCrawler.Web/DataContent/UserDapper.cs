using Dapper;
using HouseCrawler.Web.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseCrawler.Web.DataContent
{
    public class UserDataDapper
    {
        protected static internal IDbConnection Connection => new MySqlConnection(ConnectionStrings.MySQLConnectionString);

        public static UserInfo InsertUser(UserInfo insertUser)
        {
            using (IDbConnection dbConnection = Connection)
            {
                insertUser.Password = Tools.GetMD5(insertUser.Password);
                var user = dbConnection.Query<UserInfo>(@"INSERT INTO `UserInfos` 
                (`UserName`,`Email`, `Password`, `ActivatedCode`,`ActivatedTime`)
                  VALUES (@UserName, @Email, @Password, @ActivatedCode, now());",
                insertUser).FirstOrDefault();
                return user;
            }
        }

        
        public static UserInfo FindUser(string userName)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"SELECT * FROM housecrawler.UserInfos 
                where (UserName = @UserName or Email=@UserName) ;",new { UserName = userName }).FirstOrDefault();
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


        public static UserInfo SaveUserStatus(long userID,int  status)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"UPDATE `housecrawler`.`UserInfos` SET `Status`=@Status WHERE `ID`=@ID ;",
                new { ID = userID, Status=status }).FirstOrDefault();
            }
        }

    }
}