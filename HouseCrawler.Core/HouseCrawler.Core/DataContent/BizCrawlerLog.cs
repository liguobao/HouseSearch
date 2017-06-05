using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public class BizCrawlerLog
    {
        public long ID { get; set; }

        public int LogType { get; set; }

        public string LogContent { get; set; } 

        public DateTime DataChange_LastTime { get; set; }

        public string LogTile { get; set; }


        public static void SaveLog(string logTitle, string logContent,int logType)
        {

        }

        

    }
}
