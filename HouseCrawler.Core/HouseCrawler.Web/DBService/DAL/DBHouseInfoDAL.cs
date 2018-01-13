//============================================================
//http://codelover.link author:李国宝
//============================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using HouseCrawler.Web.Model;
using System.Data.Common;

namespace HouseCrawler.Web.DAL
{

    public partial class DBHouseInfoDAL
    {






        #region 把DataRow转换成Model
        /// <summary>
        /// 把DataRow转换成Model
        /// </summary>
        public DBHouseInfo ToModel(MySqlDataReader dr)
        {
            DBHouseInfo houseInfo = new DBHouseInfo();

            houseInfo.Id = (long)ToModelValue(dr, "Id");
            houseInfo.HouseTitle = (string)ToModelValue(dr, "HouseTitle");
            houseInfo.HouseOnlineURL = (string)ToModelValue(dr, "HouseOnlineURL");
            houseInfo.HouseLocation = (string)ToModelValue(dr, "HouseLocation");
            houseInfo.DisPlayPrice = (string)ToModelValue(dr, "DisPlayPrice");
            houseInfo.PubTime = (DateTime)ToModelValue(dr, "PubTime");
            houseInfo.HousePrice = (decimal)ToModelValue(dr, "HousePrice");
            houseInfo.LocationCityName = (string)ToModelValue(dr, "LocationCityName");
            houseInfo.DataCreateTime = (DateTime)ToModelValue(dr, "DataCreateTime");
            houseInfo.Source = (string)ToModelValue(dr, "Source");
            houseInfo.HouseText = (string)ToModelValue(dr, "HouseText");
            houseInfo.DataChange_LastTime = (DateTime?)ToModelValue(dr, "DataChange_LastTime");
            houseInfo.IsAnalyzed = (bool?)ToModelValue(dr, "IsAnalyzed");
            return houseInfo;
        }
        #endregion


        #region 把DataRow转换成Model
        /// <summary>
        /// 把DataRow转换成Model
        /// </summary>
        public DBHouseInfo ToModel(DbDataReader dr)
        {
            DBHouseInfo houseInfo = new DBHouseInfo();

            houseInfo.Id = (long)ToModelValue(dr, "Id");
            houseInfo.HouseTitle = (string)ToModelValue(dr, "HouseTitle");
            houseInfo.HouseOnlineURL = (string)ToModelValue(dr, "HouseOnlineURL");
            houseInfo.HouseLocation = (string)ToModelValue(dr, "HouseLocation");
            houseInfo.DisPlayPrice = (string)ToModelValue(dr, "DisPlayPrice");
            houseInfo.PubTime = (DateTime)ToModelValue(dr, "PubTime");
            houseInfo.HousePrice = (decimal)ToModelValue(dr, "HousePrice");
            houseInfo.LocationCityName = (string)ToModelValue(dr, "LocationCityName");
            houseInfo.DataCreateTime = (DateTime)ToModelValue(dr, "DataCreateTime");
            houseInfo.Source = (string)ToModelValue(dr, "Source");
            houseInfo.HouseText = (string)ToModelValue(dr, "HouseText");
            houseInfo.DataChange_LastTime = (DateTime?)ToModelValue(dr, "DataChange_LastTime");
            houseInfo.IsAnalyzed = (bool)ToModelValue(dr, "IsAnalyzed");
            return houseInfo;
        }
        #endregion







        #region 获得分页记录集IEnumerable<>
        ///<summary>
        /// 获得分页记录集IEnumerable<>
        ///</summary>              
        public IEnumerable<DBHouseInfo> SearchHouseInfo(string cityName, string source = "",
            int houseCount = 100, int withinAnyDays = 7, string keyword = "")
        {
            List<DBHouseInfo> houses = new List<DBHouseInfo>();
            if (string.IsNullOrEmpty(source))
            {
                var douban = GetSQLText(cityName, ConstConfigurationName.Douban, houseCount, withinAnyDays, keyword);
                using (DbDataReader reader = MyDBHelper.ExecuteDataReader(douban.Item1, douban.Item2.ToArray()))
                {
                    houses.AddRange(ToModels(reader));
                }
                var huzu = GetSQLText(cityName, ConstConfigurationName.HuZhuZuFang, houseCount, withinAnyDays, keyword);
                using (DbDataReader reader = MyDBHelper.ExecuteDataReader(douban.Item1, douban.Item2.ToArray()))
                {
                    houses.AddRange(ToModels(reader));
                }

                var pinpai = GetSQLText(cityName, ConstConfigurationName.PinPaiGongYu, houseCount, withinAnyDays, keyword);
                using (DbDataReader reader = MyDBHelper.ExecuteDataReader(douban.Item1, douban.Item2.ToArray()))
                {
                    houses.AddRange(ToModels(reader));
                }
            }
            else
            {
                var result = GetSQLText(cityName, source, houseCount, withinAnyDays, keyword);
                using (DbDataReader reader = MyDBHelper.ExecuteDataReader(result.Item1, result.Item2.ToArray()))
                {
                    houses.AddRange(ToModels(reader));
                }
            }
            return houses;
        }

        private static Tuple<string, List<MySqlParameter>> GetSQLText(string cityName, string source, int houseCount,
            int withinAnyDays, string keyword)
        {
            string sqlText = $"SELECT * from { GetTableName(source)} where 1=1 ";
            List<MySqlParameter> lstMySqlParameter = new List<MySqlParameter>();
            if (!string.IsNullOrEmpty(cityName))
            {
                sqlText = sqlText + " and LocationCityName = @LocationCityName ";
                lstMySqlParameter.Add(new MySqlParameter() { ParameterName = "@LocationCityName", Value = cityName, DbType = DbType.String });

            }

            if (!string.IsNullOrEmpty(keyword))
            {
                string keywordTxt = "%" + keyword + "%";
                sqlText = sqlText + " and (HouseText like @keyword or HouseLocation like @keyword ) ";
                //仅仅显示有效数据
                lstMySqlParameter.Add(new MySqlParameter() { ParameterName = "@keyword", Value = keywordTxt, DbType = DbType.String });
            }
            sqlText = sqlText + string.Format(" order by PubTime limit {0} ", houseCount);

            return Tuple.Create<string, List<MySqlParameter>>(sqlText, lstMySqlParameter);
        }

        private static string GetTableName(string source)
        {
            string tableName = "HouseInfos";
            if (source == ConstConfigurationName.Douban)
            {
                tableName = "DoubanHouseInfos";
            }
            else if (source == ConstConfigurationName.HuZhuZuFang)
            {
                tableName = "MutualHouseInfos";
            }
            else if (source == ConstConfigurationName.PinPaiGongYu)
            {
                tableName = "ApartmentHouseInfos";
            };

            return tableName;
        }
        #endregion




        #region 把MySqlDataReader转换成IEnumerable<>
        ///<summary>
        /// 把MySqlDataReader转换成IEnumerable<>
        ///</summary> 
        protected IEnumerable<DBHouseInfo> ToModels(MySqlDataReader reader)
        {
            var list = new List<DBHouseInfo>();
            while (reader.Read())
            {
                list.Add(ToModel(reader));
            }
            return list;
        }
        #endregion


        #region 把MySqlDataReader转换成IEnumerable<>
        ///<summary>
        /// 把MySqlDataReader转换成IEnumerable<>
        ///</summary> 
        protected IEnumerable<DBHouseInfo> ToModels(DbDataReader reader)
        {
            var list = new List<DBHouseInfo>();
            while (reader.Read())
            {
                list.Add(ToModel(reader));
            }
            return list;
        }
        #endregion

        #region 判断数据是否为空
        ///<summary>
        /// 判断数据是否为空
        ///</summary>
        protected object ToDBValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }
        #endregion

        #region 判断数据表中是否包含该字段
        ///<summary>
        /// 判断数据表中是否包含该字段
        ///</summary>
        protected object ToModelValue(MySqlDataReader reader, string columnName)
        {
            if (reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                return null;
            }
            else
            {
                return reader[columnName];
            }
        }
        #endregion


        #region 判断数据表中是否包含该字段
        ///<summary>
        /// 判断数据表中是否包含该字段
        ///</summary>
        protected object ToModelValue(DbDataReader reader, string columnName)
        {
            if (reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                return null;
            }
            else
            {
                return reader[columnName];
            }
        }
        #endregion
    }
}