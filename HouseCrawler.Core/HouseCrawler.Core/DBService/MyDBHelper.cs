using HouseCrawler.Core;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace HouseCrawler.Web
{

    public class MyDBHelper
    {
       

        #region 执行Command.ExecuteNonQuery(),返回受影响的行数
        /// <summary>
        /// 执行Command.ExecuteNonQuery(),返回受影响的行数
        /// </summary>
        /// <param name="cmdText">执行的语句</param>
        /// <param name="parameters">params传入的参数null</param>
        /// <returns></returns>         
        public static int ExecuteNonQuery(string cmdText, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionStrings.MySQLConnectionString))
            {
                int result = -1;
                conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    if (parameters == null)
                        result = cmd.ExecuteNonQuery();
                    else
                    {
                        cmd.Parameters.AddRange(parameters);
                        result = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    return result;
                }
            }
        }
        #endregion

        #region 执行Command.ExecuteScalar(),返回首行首列
        /// <summary>
        /// 执行Command.ExecuteScalar(),返回首行首列
        /// </summary>
        /// <param name="cmdText">执行的语句</param>
        /// <param name="parameters">params传入的参数null</param>
        /// <returns></returns>    
        public static object ExecuteScalar(string cmdText,
            params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionStrings.MySQLConnectionString))
            {
                object obj = null;
                conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    if (parameters == null)
                        obj = cmd.ExecuteScalar();
                    else
                    {
                        cmd.Parameters.AddRange(parameters);
                        obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                    return obj;
                }
            }
        }
        #endregion


        #region 执行ExecuteReader,返回MySqlDataReader
        /// <summary>
        /// 执行ExecuteReader,返回MySqlDataReader
        /// </summary>
        /// <param name="cmdText">执行的语句</param>
        /// <param name="parameters">params传入的参数null</param>
        /// <returns></returns>
        public static DbDataReader ExecuteDataReader(string cmdText, params MySqlParameter[] parameters)
        {
            MySqlConnection conn = new MySqlConnection(ConnectionStrings.MySQLConnectionString);
            DbDataReader read = null;
            conn.Open();
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                    read = cmd.ExecuteReader();
                    cmd.Parameters.Clear();
                }
                else
                {
                    read = cmd.ExecuteReader();
                }
                return read;
            }
        }
        #endregion



       
        #region 执行存储过程 ExecuteNonQuery(),返回受影响的行数
        /// <summary>
        /// 执行存储过程 ExecuteNonQuery(),返回受影响的行数
        /// </summary>
        /// <param name="cmdText">执行的语句</param>
        /// <param name="parameters">params传入的参数null</param>
        /// <returns></returns> 
        public static int ExecuteStoredProcedure(string procName, params MySqlParameter[] parameters)
        {
            MySqlConnection conn = new MySqlConnection(ConnectionStrings.MySQLConnectionString);
            int result = -1;
            conn.Open();
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                    result = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                else
                {
                    cmd.Parameters.AddRange(parameters);
                    result = cmd.ExecuteNonQuery();
                }
                return result;
            }
        }
        #endregion
    }
}
