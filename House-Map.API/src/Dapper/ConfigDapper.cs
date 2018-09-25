using Dapper;
using HouseMapAPI.Common;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Models;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
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
