using Dapper;
using HouseMapAPI.Common;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMapAPI.Dapper
{
    public class ConfigDapper : BaseDapper
    {
        public ConfigDapper(IOptions<AppSettings> configuration)
        : base(configuration)
        {
        }



        public void Insert(CrawlerConfig conf)
        {
            string sqlText = @"INSERT INTO `housecrawler`.`CrawlerConfigurations`
             (`ConfigurationName`, `ConfigurationValue`, `ConfigurationKey`, `IsEnabled`) 
             VALUES (@ConfigurationName,@ConfigurationValue, @ConfigurationKey,1);";

            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute(sqlText,
                                     conf, transaction: transaction);
                transaction.Commit();
            }

        }


        public List<CrawlerConfig> FindAll()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<CrawlerConfig>("SELECT * FROM housecrawler.CrawlerConfigurations").ToList();
            }
        }

    }
}
