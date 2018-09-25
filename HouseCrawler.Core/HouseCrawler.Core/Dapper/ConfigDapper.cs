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
using Newtonsoft.Json.Linq;

namespace HouseCrawler.Core
{
    public class ConfigDapper
    {
        private APPConfiguration configuration;

        public ConfigDapper(IOptions<APPConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }


        public List<CrawlerConfiguration> GetList(string configurationName)
        {


            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();


                return dbConnection.Query<CrawlerConfiguration>(@"SELECT * FROM housecrawler.CrawlerConfigurations 
                where ConfigurationName=@ConfigurationName;", new
                {
                    ConfigurationName = configurationName
                }).ToList();
            }
        }

        public void BulkInsertConfig(List<CrawlerConfiguration> configs)
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

        public List<CrawlerConfiguration> FindAll()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<CrawlerConfiguration>("SELECT * FROM housecrawler.CrawlerConfigurations").ToList();
            }
        }

        public List<HouseDashboard> GetDashboards()
        {
            var configs = FindAll();
            var dashboards = new List<HouseDashboard>();
            foreach (var config in configs)
            {
                var configJson = JToken.Parse(config.ConfigurationValue);
                var dash = new HouseDashboard()
                {
                    Source = config.ConfigurationName,
                    CityName = configJson["cityname"] != null
                    ? configJson["cityname"].ToString()
                    : configJson["cityName"].ToString(),
                    HouseSum = 9999,
                    LastRecordPubTime = DateTime.Now
                };
                dashboards.Add(dash);
            }
            return dashboards;
        }

    }
}