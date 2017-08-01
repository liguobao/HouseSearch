using HouseCrawler.Web.DBService.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Web.DBService.DAL
{
    public class DBHouseSourceInfoDAL
    {


        #region 
        ///<summary>
        /// 
        ///</summary>              
        public IEnumerable<DBHouseSourceInfo> GetHouseSourceInfoList()
        {
            string sqlText = @"SELECT 
                                LocationCityName AS CityName, Source, COUNT(id) AS HouseSum
                            FROM
                                housecrawler.HouseInfos
                            GROUP BY LocationCityName, Source; ";

            using (DbDataReader reader = MyDBHelper.ExecuteDataReader(sqlText))
            {
                return ToModels(reader);
            }
        }
        #endregion



        #region 把MySqlDataReader转换成IEnumerable<>
        ///<summary>
        /// 把MySqlDataReader转换成IEnumerable<>
        ///</summary> 
        protected IEnumerable<DBHouseSourceInfo> ToModels(DbDataReader reader)
        {
            var list = new List<DBHouseSourceInfo>();
            while (reader.Read())
            {
                list.Add(ToModel(reader));
            }
            return list;
        }
        #endregion


        #region 把DataRow转换成Model
        /// <summary>
        /// 把DataRow转换成Model
        /// </summary>
        public DBHouseSourceInfo ToModel(DbDataReader dr)
        {
            DBHouseSourceInfo model = new DBHouseSourceInfo();
            model.CityName = (string)ToModelValue(dr, "CityName");
            model.HouseSum = (int)ToModelValue(dr, "HouseSum");
            model.Source = (string)ToModelValue(dr, "Source");
            model.UpdateTime = DateTime.Now;
            return model;
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
