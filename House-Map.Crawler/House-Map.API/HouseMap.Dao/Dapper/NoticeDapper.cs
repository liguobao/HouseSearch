using Dapper;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMap.Dao
{
    public class NoticeDapper : BaseDapper
    {
        public NoticeDapper(IOptions<AppSettings> configuration)
        : base(configuration)
        {
        }

        public Notice FindLastNotice()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Notice>(@"SELECT 
                        *
                    FROM
                        Notice
                    WHERE EndShowDate > @Today order by EndShowDate desc;",
                new
                {
                    Today = DateTime.Now.Date
                }).FirstOrDefault();
            }
        }

        public List<Notice> FindAllNotice()
        {
            using (IDbConnection dbConnection = GetConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Notice>(@"SELECT 
                        *
                    FROM
                        Notice
                    order by DataCreateTime desc limit 10;").ToList();
            }
        }
    }
}