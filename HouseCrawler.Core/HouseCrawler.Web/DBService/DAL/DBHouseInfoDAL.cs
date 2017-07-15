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
        public IEnumerable<DBHouseInfo> SearchHouseInfo(string cityName, string source = "", 
            int houseCount = 100, int withinAnyDays = 3,bool showDoubanInvalidData = true, string keyword = "")
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

            //是否展示豆瓣无效数据
            if(!showDoubanInvalidData && source == ConstConfigurationName.Douban)
            {
                sqlText = sqlText + " and Status = @Status and HousePrice > 0 ";
                //仅仅显示有效数据
                lstMySqlParameter.Add(new MySqlParameter() { ParameterName = "@Status", Value = 1, DbType = DbType.Int32 });
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword +"%";
                sqlText = sqlText + " and (HouseText like @keyword or HouseLocation like @keyword ) ";
                //仅仅显示有效数据
                lstMySqlParameter.Add(new MySqlParameter() { ParameterName = "@keyword", Value = keyword, DbType = DbType.String });
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