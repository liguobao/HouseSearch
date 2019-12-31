using Dapper;
using HouseCrawler.Web;
using HouseCrawler.Web.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseCrawler.Web
{
    public class ConfigDapper
    {
        private APPConfiguration configuration;
        public ConfigDapper(IOptions<APPConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }

        private IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.MySQLConnectionString);
        }

        public void Insert(CrawlerConfiguration conf)
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

    }
}
