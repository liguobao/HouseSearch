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
    public class UserHouseDapper : BaseDapper
    {
        public UserHouseDapper(IOptions<APPConfiguration> configuration, RedisService redisService)
        : base(configuration, redisService)
        {
        }

        public UserHouse InsertUser(UserHouse userHouse)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                var collection = dbConnection.Query<UserHouse>(@"INSERT INTO `housecrawler`.`UserHouses`
                                    (
                                    `UserId`,
                                    `HouseTitle`,
                                    `HouseOnlineURL`,
                                    `HouseLocation`,
                                    `DisPlayPrice`,
                                    `PubTime`,
                                    `HousePrice`,
                                    `LocationCityName`,
                                    `Source`,
                                    `HouseText`,
                                    `Status`,
                                    `PicURLs`,
                                    `RentTyp`)
                                    VALUES
                                    (
                                    @UserId,
                                    @HouseTitle,
                                    @HouseOnlineURL,
                                    @HouseLocation,
                                    @DisPlayPrice,
                                    @PubTime,
                                    @HousePrice,
                                    @LocationCityName,
                                    @Source,
                                    @HouseText,
                                    @Status,
                                    @PicURLs,
                                    @RentTyp);",
                userHouse).FirstOrDefault();
                return collection;
            }
        }

        /// <summary>
        ///  获取用户N天内发布的房源数量
        /// </summary>
        public int GetUserHouseCount(long userId, int intervalDay)
        {
            DateTime today = DateTime.Now;
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<int>(@"SELECT 
                        count(*) as Count
                    FROM
                        UserHouses
                    WHERE
                        UserId = @UserId
                            AND PubTime BETWEEN @FromDate AND @ToDate;",
                new
                {
                    UserId = userId,
                    FromDate = today.Date.AddDays(-intervalDay),
                    ToDate = today.Date.AddDays(1)
                }).FirstOrDefault();
            }
        }

    }
}