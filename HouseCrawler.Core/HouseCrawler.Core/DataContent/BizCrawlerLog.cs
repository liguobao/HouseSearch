using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public class BizCrawlerLog
    {
        public long ID { get; set; }

        /// <summary>
        /// -1 系统异常 0 一般日志 1 爬虫日志
        /// </summary>
        public int LogType { get; set; }

        public string LogContent { get; set; }



        public string LogTitle { get; set; }

        public static void SaveLog(string logTitle, string logContent, int logType)
        {
            var log = new BizCrawlerLog
            {
                LogTitle = logTitle,
                LogContent = logContent,
                LogType = logType
            };

        }




    }
}
