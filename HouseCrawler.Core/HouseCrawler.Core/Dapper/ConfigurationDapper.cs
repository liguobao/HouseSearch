using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Service;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace HouseCrawler.Core
{
    public class ConfigurationDapper
    {
        private APPConfiguration configuration;

        public ConfigurationDapper(IOptions<APPConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }


        public List<BizCrawlerConfiguration> GetConfigurationList(string configurationName)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<BizCrawlerConfiguration>(@"SELECT * FROM housecrawler.CrawlerConfigurations 
                where ConfigurationName=@ConfigurationName;", new
                {
                    ConfigurationName = configurationName
                }).ToList();
            }
        }

        public void BulkInsertConfig(List<BizCrawlerConfiguration> configs)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute(@"INSERT INTO `housecrawler`.`CrawlerConfigurations` (`ConfigurationName`, `ConfigurationValue`) 
                VALUES (@ConfigurationName, @ConfigurationValue) ON DUPLICATE KEY UPDATE DataChange_LastTime=now();",
                 configs, transaction: transaction);
                transaction.Commit();
            }
        }

        private IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.MySQLConnectionString);
        }

    }
}