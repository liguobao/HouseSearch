using Dapper;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMap.Dao
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
            return dashboards.GroupBy(d => d.CityName + d.Source).Select(item => item.First()).ToList();
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

    }
}
