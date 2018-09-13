using Dapper;
using HouseMapAPI.Common;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMapAPI.Dapper
{
    public class BaseDapper
    {

        protected AppSettings _appSettings;

        protected RedisService _redisService;

        public BaseDapper(IOptions<AppSettings> configuration, RedisService redisService)
        {
            this._appSettings = configuration.Value;
            this._redisService = redisService;
        }

         public BaseDapper(IOptions<AppSettings> configuration)
        {
            this._appSettings = configuration.Value;
        }

        protected IDbConnection GetConnection()
        {
            return new MySqlConnection(_appSettings.MySQLConnectionString);
        }
    }
}