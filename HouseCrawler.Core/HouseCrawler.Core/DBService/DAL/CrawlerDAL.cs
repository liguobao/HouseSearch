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
            string sqlText = "select distinct HouseOnlineURL from housecrawler.HouseInfos where Source ='huzhuzufang'";
            using (DbDataReader reader = MyDBHelper.ExecuteDataReader(sqlText))
            {
                while (reader.Read())
                {
                    string houseOnlineURL = (string)ToModelValue(reader, "HouseOnlineURL");
                    if (!hsURL.Contains(houseOnlineURL))
                        hsURL.Add(houseOnlineURL);
                }
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
            using (DbDataReader reader = MyDBHelper.ExecuteDataReader(sqlText, lstMySqlParameter.ToArray()))
            {
                var result= ToModelValue(reader, "Count");
                if (result != null)
                    return Convert.ToInt32(result) > 0;
                else
                    return false;
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
