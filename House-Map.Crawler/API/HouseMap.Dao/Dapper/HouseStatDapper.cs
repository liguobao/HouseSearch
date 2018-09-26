using Dapper;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMap.Dao
{
    public class HouseStatDapper : BaseDapper
    {
        public HouseStatDapper(IOptions<AppSettings> configuration)
        : base(configuration)
        {
        }

       


        public List<HouseStat> GetHouseStatList(int intervalDay = 1)
        {
            List<HouseStat> houseStatList = new List<HouseStat>();
            DateTime today = DateTime.Now;

            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                foreach (var tableName in ConstConfigName.HouseTableNameDic.Values)
                {
                    houseStatList.AddRange(dbConnection.Query<HouseStat>(@"SELECT COUNT(*) AS HouseSum,
                    MAX(PubTime) AS LastPubTime,
                    MAX(DataCreateTime) AS LastCreateTime,
                    Source 
                    FROM " + tableName + " WHERE DataCreateTime BETWEEN @FromTime AND @ToTime;", new
                    {
                        FromTime = today.AddDays(-intervalDay).Date,
                        ToTime = today
                    }).ToList());
                }
                return houseStatList;

            }
        }
    }

    public class HouseStat
    {

        public int HouseSum { get; set; }

        public string Source { get; set; }

        public DateTime LastPubTime { get; set; }

        public DateTime LastCreateTime { get; set; }
    }
}