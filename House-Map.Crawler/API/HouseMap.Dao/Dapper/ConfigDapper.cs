using Dapper;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
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

        public List<DBConfig> LoadAll(string city = "")
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                var queryText = @"SELECT 
                        *
                    FROM
                        Config where 1=1 and score >=0 ";
                if (!string.IsNullOrEmpty(city))
                {
                    queryText = queryText + " and city =@city ";
                }
                queryText = queryText + " order by score desc;";
                return dbConnection.Query<DBConfig>(queryText, new { city = city }).ToList();
            }
        }


        public List<DBConfig> LoadBySource(string source)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                var queryText = @"SELECT 
                        *
                    FROM
                        Config where 1=1 ";
                if (!string.IsNullOrEmpty(source))
                {
                    queryText = queryText + " and source =@source ";
                }
                queryText = queryText + " order by score desc;";
                return dbConnection.Query<DBConfig>(queryText, new { source = source }).ToList();
            }
        }


        public void BulkInsert(List<DBConfig> configs)
        {
            if (configs == null || configs.Count == 0)
            {
                return;
            }
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute(@"INSERT INTO Config
                                     (`Id`, `City`,  
                                    `Source`, `PageCount`, 
                                    `Json`,
                                     `Score`) 
                                     VALUES (@Id, @City,
                                            @Source, @PageCount,
                                            @Json, 
                                            @Score)  ON DUPLICATE KEY UPDATE UpdateTime=now();",
                                     configs, transaction: transaction);
                transaction.Commit();
            }
        }


    }
}