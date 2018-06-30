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
    public class UserDataDapper
    {
        private APPConfiguration configuration;

        private RedisService redisService;
        public UserDataDapper(IOptions<APPConfiguration> configuration, RedisService redisService)
        {
            this.configuration = configuration.Value;
            this.redisService = redisService;
        }

        private IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.MySQLConnectionString);
        }

        public void InsertUser(UserInfo insertUser)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                insertUser.Password = Tools.GetMD5(insertUser.Password);
                dbConnection.Query<UserInfo>(@"INSERT INTO `UserInfos` 
                (`UserName`,`Email`, `Password`, `ActivatedCode`,`ActivatedTime`)
                  VALUES (@UserName, @Email, @Password, @ActivatedCode, now());",
                insertUser);
            }
        }


        public void InsertUserForQQAuth(UserInfo insertUser)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                var user = dbConnection.Query<UserInfo>(@"INSERT INTO `UserInfos` 
                (`UserName`,`QQOpenUID`)
                  VALUES (@UserName, @QQOpenUID);",
                insertUser);
            }
        }


        public UserInfo FindUser(string userName)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"SELECT * FROM housecrawler.UserInfos 
                where (UserName = @UserName or Email=@UserName) ;", new { UserName = userName }).FirstOrDefault();
            }
        }


        public UserInfo FindUserByActivatedCode(string activatedCode)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"SELECT * FROM housecrawler.UserInfos 
                where (ActivatedCode = @ActivatedCode) ;", new { ActivatedCode = activatedCode }).FirstOrDefault();
            }
        }


        public UserInfo FindUserByQQOpenUID(string qqOpenUID)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"SELECT * FROM housecrawler.UserInfos 
                where (QQOpenUID = @QQOpenUID) ;", new { QQOpenUID = qqOpenUID }).FirstOrDefault();
            }
        }

        public UserInfo SaveUserStatus(long userID, int status)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"UPDATE `housecrawler`.`UserInfos` SET `Status`=@Status WHERE `ID`=@ID ;",
                new { ID = userID, Status = status }).FirstOrDefault();
            }
        }


        public UserInfo SaveRetrievePasswordToken(long userID, string token)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"UPDATE `UserInfos` SET `RetrievePasswordToken`=@RetrievePasswordToken, TokenTime=now()
                  WHERE `ID`=@ID ;",
                new { ID = userID, RetrievePasswordToken = token }).FirstOrDefault();
            }
        }

        public UserInfo FindUserByToken(string token)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"SELECT * FROM UserInfos 
                where (RetrievePasswordToken = @RetrievePasswordToken) ;", new { RetrievePasswordToken = token }).FirstOrDefault();
            }
        }


        public UserInfo SavePassword(long userID, string password)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<UserInfo>(@"UPDATE `UserInfos` SET Password=@Password  
                WHERE `ID`=@ID ;",
                new { ID = userID, Password = password }).FirstOrDefault();
            }
        }

    }
}