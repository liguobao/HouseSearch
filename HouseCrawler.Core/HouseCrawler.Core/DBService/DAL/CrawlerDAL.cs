using HouseCrawler.Web;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.DBService.DAL
{
    public class CrawlerDAL
    {

        /// <summary>
        /// 获取所有的互助租房URL
        /// </summary>
        /// <returns></returns>
        public  HashSet<String> GetAllHuzhuzufangHouseOnlineURL()
        {
            HashSet<String> hsURL = new HashSet<string>();
            string sqlText = "select distinct HouseOnlineURL from MutualHouseInfos";
            using (DbDataReader reader = MyDBHelper.ExecuteDataReader(sqlText))
            {
                while (reader.Read())
                {
                    string houseOnlineURL = (string)ToModelValue(reader, "HouseOnlineURL");
                    if (!hsURL.Contains(houseOnlineURL))
                        hsURL.Add(houseOnlineURL);
                }
                reader.Dispose();
            }
            return hsURL;
        }


        /// <summary>
        /// 获取城市房源URL HashSet（默认取1000条）
        /// </summary>
        /// <returns></returns>
        public HashSet<String> GetAllHouseOnlineURL(string source,string cityName,int limit=1000)
        {
            HashSet<String> hsURL = new HashSet<string>();
            string sqlText = @"select distinct HouseOnlineURL from housecrawler.HouseInfos where Source =@Source 
                            and LocationCityName=@LocationCityName order by PubTime limit " + limit;

            MySqlParameter[] para = new MySqlParameter[]
           {
                new MySqlParameter() { ParameterName = "@Source", Value = source, DbType = DbType.String },
                new MySqlParameter() { ParameterName = "@LocationCityName", Value = cityName, DbType = DbType.String },
           };

            using (DbDataReader reader = MyDBHelper.ExecuteDataReader(sqlText, para))
            {
                while (reader.Read())
                {
                    string houseOnlineURL = (string)ToModelValue(reader, "HouseOnlineURL");
                    if (!hsURL.Contains(houseOnlineURL))
                        hsURL.Add(houseOnlineURL);
                }
                reader.Dispose();
            }
            return hsURL;
        }



        /// <summary>
        /// 是否包含此URL
        /// </summary>
        /// <param name="houseOnlineURL"></param>
        /// <returns></returns>
        public bool ContainsHouseOnlineURL(string houseOnlineURL)
        {
            string sqlText = "SELECT count(id) as Count FROM housecrawler.HouseInfos where HouseOnlineURL=@HouseOnlineURL";
            List<MySqlParameter> lstMySqlParameter = new List<MySqlParameter>()
            {
                new MySqlParameter() { ParameterName = "@HouseOnlineURL", Value = houseOnlineURL, DbType = DbType.String }
            };
            try
            {
                var result = MyDBHelper.ExecuteScalar(sqlText, lstMySqlParameter.ToArray());
                return Convert.ToInt32(result) > 0;

            }
            catch(Exception ex)
            {
                LogHelper.Error("ContainsHouseOnlineURL", ex, new { URL = houseOnlineURL });
                return true;
            }
            
        }

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
