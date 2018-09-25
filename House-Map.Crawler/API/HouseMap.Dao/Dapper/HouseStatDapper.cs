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

        public List<HouseDashboard> GetHouseDashboard()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                var list = new List<HouseDashboard>();
                foreach (var key in ConstConfigName.HouseTableNameDic.Keys)
                {
                    var tableName = ConstConfigName.GetTableName(key);
                    var dashboards = dbConnection.Query<HouseDashboard>(@"SELECT 
                                LocationCityName AS CityName,
                                Source, COUNT(id) AS HouseSum, 
                                MAX(PubTime) AS LastRecordPubTime
                            FROM 
                                " + tableName + $" GROUP BY LocationCityName, Source ORDER BY HouseSum desc;");
                    list.AddRange(dashboards);
                }
                return list.Where(dash => dash.LastRecordPubTime.CompareTo(DateTime.Now.AddDays(-30)) > 0 && dash.HouseSum > 100)
                .ToList();
            }
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