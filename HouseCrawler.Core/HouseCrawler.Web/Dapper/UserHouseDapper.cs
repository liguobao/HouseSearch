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

        public UserCollection InsertUser(UserHouse userHouse)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                var collection = dbConnection.Query<UserCollection>(@"INSERT INTO `housecrawler`.`UserHouses`
                                    (
                                    `UserId`,
                                    `HouseTitle`,
                                    `HouseOnlineURL`,
                                    `HouseLocation`,
                                    `DisPlayPrice`,
                                    `PubTime`,
                                    `HousePrice`,
                                    `LocationCityName`,
                                    `DataCreateTime`,
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
                                    @DataChange_LastTime,
                                    @Status,
                                    @PicURLs,
                                    @RentTyp);",
                userHouse).FirstOrDefault();
                return collection;
            }
        }

    }
}