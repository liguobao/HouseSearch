using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using HouseMap.Crawler.Common;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace HouseMap.Crawler.Dapper
{
    public class ConfigDapper
    {
        private APPConfiguration configuration;

        public ConfigDapper(IOptions<APPConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }


        public List<CrawlerConfig> GetList(string configurationName)
        {


            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();


                return dbConnection.Query<CrawlerConfig>(@"SELECT * FROM housecrawler.CrawlerConfigurations 
                where ConfigurationName=@ConfigurationName;", new
                {
                    ConfigurationName = configurationName
                }).ToList();
            }
        }

        public void BulkInsertConfig(List<CrawlerConfig> configs)
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