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


        #region  根据Id删除数据记录
        /// <summary>
        /// 根据Id删除数据记录
        /// </summary>
        public int DeleteById(long id)
        {
            string sql = "DELETE from HouseInfos WHERE Id = @Id";

            MySqlParameter[] para = new MySqlParameter[]
            {
                new MySqlParameter(){SourceColumn="@Id",Value =id,DbType =DbType.Int64}
            };

            return MyDBHelper.ExecuteNonQuery(sql, para);
        }
        #endregion



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
            houseInfo.PubTime = (DateTime?)ToModelValue(dr, "PubTime");
            houseInfo.HousePrice = (decimal?)ToModelValue(dr, "HousePrice");
            houseInfo.LocationCityName = (string)ToModelValue(dr, "LocationCityName");
            houseInfo.DataCreateTime = (DateTime?)ToModelValue(dr, "DataCreateTime");
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
            houseInfo.PubTime = (DateTime?)ToModelValue(dr, "PubTime");
            houseInfo.HousePrice = (decimal?)ToModelValue(dr, "HousePrice");
            houseInfo.LocationCityName = (string)ToModelValue(dr, "LocationCityName");
            houseInfo.DataCreateTime = (DateTime?)ToModelValue(dr, "DataCreateTime");
            houseInfo.Source = (string)ToModelValue(dr, "Source");
            houseInfo.HouseText = (string)ToModelValue(dr, "HouseText");
            houseInfo.DataChange_LastTime = (DateTime?)ToModelValue(dr, "DataChange_LastTime");
            houseInfo.IsAnalyzed = (bool)ToModelValue(dr, "IsAnalyzed");
            return houseInfo;
        }
        #endregion


        #region  获得总记录数
        ///<summary>
        /// 获得总记录数
        ///</summary>        
        public int GetTotalCount()
        {
            string sql = "SELECT count(*) FROM HouseInfos";
            return (int)MyDBHelper.ExecuteScalar(sql);
        }
        #endregion





        #region 获得分页记录集IEnumerable<>
        ///<summary>
        /// 获得分页记录集IEnumerable<>
        ///</summary>              
        public IEnumerable<DBHouseInfo> SearchHouseInfo(string cityName, string source = "", int houseCount = 100, int withinAnyDays = 3)
        {
            string sqlText = "SELECT * from HouseInfos where 1=1 ";

            List<MySqlParameter> lstMySqlParameter = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(cityName))
            {
                sqlText = sqlText + " and LocationCityName = @LocationCityName ";
                lstMySqlParameter.Add(new MySqlParameter() { ParameterName = "@LocationCityName", Value = cityName, DbType = DbType.String });

            }
            if (!string.IsNullOrEmpty(source))
            {
                sqlText = sqlText + " and Source = @Source ";
                lstMySqlParameter.Add(new MySqlParameter() { ParameterName = "@Source", Value = source, DbType = DbType.String });
            }
            sqlText = sqlText + " and PubTime>= @PubTime ";
            lstMySqlParameter.Add(new MySqlParameter() { ParameterName = "@PubTime", Value = DateTime.Now.Date.AddDays(-withinAnyDays), DbType = DbType.Date });
            sqlText = sqlText + string.Format(" limit {0} ", houseCount);

            using (DbDataReader reader = MyDBHelper.ExecuteDataReader(sqlText, lstMySqlParameter.ToArray()))
            {
                return ToModels(reader);
            }
        }
        #endregion



        #region
        ///<summary>
        /// 获取需要分析的数据（一次2000条）
        ///</summary>              
        public IEnumerable<DBHouseInfo> LoadUnAnalyzeList()
        {
            string sqlText = "SELECT * FROM housecrawler.HouseInfos where Source='douban' and Status =0 limit 2000 ;";

            using (DbDataReader reader = MyDBHelper.ExecuteDataReader(sqlText))
            {
                return ToModels(reader);
            }
        }
        #endregion



        #region 根据传入Model更新数据并返回更新后的Model
        /// <summary>
        /// 根据传入Model更新数据并返回更新后的Model
        /// </summary>
        public int UpdateHouseInfo(DBHouseInfo dbHouseInfo)
        {
            string sql =
                "UPDATE HouseInfos " +
                "SET " +
            " HouseTitle = @HouseTitle"
                + ", HouseOnlineURL = @HouseOnlineURL"
                + ", HouseLocation = @HouseLocation"
                + ", DisPlayPrice = @DisPlayPrice"
                + ", HousePrice = @HousePrice"
                + ", LocationCityName = @LocationCityName"
                + ", Source = @Source"
                + ", HouseText = @HouseText"
                + ", IsAnalyzed = @IsAnalyzed"
                + ", Status = @Status"

            + " WHERE Id = @Id";

            MySqlParameter[] para = new MySqlParameter[]
            {
                new MySqlParameter() { ParameterName = "@Id", Value = dbHouseInfo.Id, DbType = DbType.Int64 },
                new MySqlParameter() { ParameterName = "@HouseTitle", Value = dbHouseInfo.HouseTitle, DbType = DbType.String },
                new MySqlParameter() { ParameterName = "@HouseOnlineURL", Value = dbHouseInfo.HouseOnlineURL, DbType = DbType.String },
                new MySqlParameter() { ParameterName = "@HouseLocation", Value = dbHouseInfo.HouseLocation, DbType = DbType.String },

                new MySqlParameter() { ParameterName = "@DisPlayPrice", Value = dbHouseInfo.DisPlayPrice, DbType = DbType.String },
                new MySqlParameter() { ParameterName = "@HousePrice", Value = dbHouseInfo.HousePrice, DbType = DbType.Decimal },
                new MySqlParameter() { ParameterName = "@LocationCityName", Value = dbHouseInfo.LocationCityName, DbType = DbType.String },
                new MySqlParameter() { ParameterName = "@Source", Value = dbHouseInfo.Source, DbType = DbType.String },
                new MySqlParameter() { ParameterName = "@HouseText", Value = dbHouseInfo.HouseText, DbType = DbType.String },
                new MySqlParameter() { ParameterName = "@IsAnalyzed", Value = dbHouseInfo.HouseText, DbType = DbType.Boolean },
                new MySqlParameter() { ParameterName = "@Status", Value = dbHouseInfo.Status, DbType = DbType.Int32 }
            };

            return MyDBHelper.ExecuteNonQuery(sql, para);
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