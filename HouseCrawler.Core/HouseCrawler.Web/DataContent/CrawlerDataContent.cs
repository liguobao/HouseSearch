using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HouseCrawler.Web
{
    public class CrawlerDataContent: DbContext
    {

        public DbSet<BizCrawlerConfiguration> CrawlerConfigurations { get; set; }

        public DbSet<BizCrawlerLog> CrawlerLogs { get; set; }
        /// <summary>
        /// Server =服务器IP，database = 数据库名称 uid=账号，pwd=密码
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConnectionStrings.MySQLConnectionString);
        }
        
           


    }
}
