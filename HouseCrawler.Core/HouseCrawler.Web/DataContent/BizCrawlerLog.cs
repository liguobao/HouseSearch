using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Web
{
    public class BizCrawlerLog
    {
        public long ID { get; set; }

        /// <summary>
        /// -1 系统异常 0 一般日志 1 爬虫日志
        /// </summary>
        public int LogType { get; set; }

        public string LogContent { get; set; } 

        public DateTime DataChange_LastTime { get; set; }

        public string LogTitle { get; set; }


        private static CrawlerDataContent dataContent = new CrawlerDataContent();

        public static void SaveLog(string logTitle, string logContent,int logType)
        {
            BizCrawlerLog log = new BizCrawlerLog();
            log.LogTitle = logTitle;
            log.LogContent = logContent;
            log.LogType = logType;

            dataContent.Add(log);
            dataContent.SaveChanges();
        }


        public static List<BizCrawlerLog> LoadLogByType(int logType)
        {
            return dataContent.CrawlerLogs.Where(l => l.LogType ==logType)
                .OrderByDescending(l=>l.DataChange_LastTime).ToList();

        }

        

    }
}
