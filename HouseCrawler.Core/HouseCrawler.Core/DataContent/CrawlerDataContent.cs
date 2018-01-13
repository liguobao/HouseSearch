using HouseCrawler.Core.DataContent;
using Microsoft.EntityFrameworkCore;

namespace HouseCrawler.Core
{
    public class CrawlerDataContent: DbContext
    {

        public DbSet<ApartmentHouseInfo> ApartmentHouseInfos { get; set; }

        public DbSet<DoubanHouseInfo> DoubanHouseInfos { get; set; }

        public DbSet<MutualHouseInfo> MutualHouseInfos { get; set; }

        public DbSet<BizCrawlerConfiguration> CrawlerConfigurations { get; set; }


        public DbSet<BizCrawlerLog> CrawlerLogs { get; set; }
        /// <inheritdoc />
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
