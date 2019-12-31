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
    public class NoticeDapper : BaseDapper
    {
        public NoticeDapper(IOptions<APPConfiguration> configuration, RedisService redisService)
        : base(configuration, redisService)
        {
        }

        public Notice FindLastNotice()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Notice>(@"SELECT 
                        *
                    FROM
                        Notice
                    WHERE EndShowDate > @Today order by EndShowDate desc;",
                new
                {
                    Today = DateTime.Now.Date
                }).FirstOrDefault();
            }
        }

        public List<Notice> FindAllNotice()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Notice>(@"SELECT 
                        *
                    FROM
                        Notice
                    order by DataCreateTime desc limit 10;").ToList();
            }
        }
    }
}