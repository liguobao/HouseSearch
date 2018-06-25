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
    public class BaseDapper
    {

        protected APPConfiguration configuration;

        protected RedisService redisService;

        public BaseDapper(IOptions<APPConfiguration> configuration, RedisService redisService)
        {
            this.configuration = configuration.Value;
            this.redisService = redisService;
        }

        protected IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.MySQLConnectionString);
        }
    }
}