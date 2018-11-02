using Dapper;
using HouseMap.Common;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMap.Dao
{
    public class BaseDapper
    {

        protected AppSettings _appSettings;

        public BaseDapper(IOptions<AppSettings> configuration)
        {
            this._appSettings = configuration.Value;
        }

        protected IDbConnection GetConnection()
        {
            return new MySqlConnection(_appSettings.QCloudMySQL);
        }
    }
}