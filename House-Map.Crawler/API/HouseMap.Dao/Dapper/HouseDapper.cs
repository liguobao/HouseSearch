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
    public class HouseDapper : BaseDapper
    {
        public HouseDapper(IOptions<AppSettings> configuration)
        : base(configuration)
        {
        }


        public IEnumerable<HouseInfo> SearchHouses(HouseCondition condition)
        {
            if (string.IsNullOrEmpty(condition.Source))
            {
                var houseList = new List<HouseInfo>();
                // 因为会走几个表,默认每个表取N条
                //var houseSources = GetCityHouseSources(condition.CityName);
                var limitCount = condition.HouseCount / ConstConfigName.HouseTableNameDic.Count;
                foreach (var houseSource in ConstConfigName.HouseTableNameDic)
                {
                    //建荣家园数据质量比较差,默认不出
                    if (houseSource.Key == ConstConfigName.CCBHouse || houseSource.Key == ConstConfigName.Chengdufgj)
                    {
                        continue;
                    }
                    condition.Source = houseSource.Key;
                    condition.HouseCount = limitCount;
                    houseList.AddRange(Search(condition));
                }
                return houseList.OrderByDescending(h => h.PubTime);
            }
            else
            {
                return Search(condition);
            }

        }
        public IEnumerable<HouseInfo> Search(HouseCondition condition)
        {
            var houses = new List<HouseInfo>();
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                houses = dbConnection.Query<HouseInfo>(condition.QueryText, condition).ToList();
                return houses;
            }
        }



        public HouseInfo GetHouseID(long houseID, string source)
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();

                return dbConnection.Query<HouseInfo>($"SELECT * FROM {ConstConfigName.GetTableName(source)} where ID = @ID",
                  new
                  {
                      ID = houseID
                  }).FirstOrDefault();
            }
        }




        public void BulkInsertHouses(List<HouseInfo> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            var tableName = ConstConfigName.GetTableName(list.FirstOrDefault().Source);
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                var result = dbConnection.Execute("INSERT INTO " + tableName + @" (`HouseTitle`, `HouseOnlineURL`, 
                                    `HouseLocation`, `DisPlayPrice`, 
                                    `PubTime`, `HousePrice`, 
                                    `LocationCityName`,
                                    `Source`,
                                    `HouseText`, 
                                    `IsAnalyzed`, 
                                    `Status`,`PicURLs`) 
                                     VALUES (@HouseTitle, @HouseOnlineURL,
                                            @HouseLocation, @DisPlayPrice,
                                            @PubTime, @HousePrice,
                                            @LocationCityName,
                                            @Source,
                                            @HouseText,
                                            @IsAnalyzed,
                                            @Status,@PicURLs)  ON DUPLICATE KEY UPDATE DataChange_LastTime=now();",
                                     list, transaction: transaction);
                transaction.Commit();
            }

        }
    }
}
