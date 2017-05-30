using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public class BizCrawlerConfiguration
    {
        public long Id { get; set; }

        [MaxLength(255)]
        public string ConfigurationName { get; set; }

        [MaxLength(255)]
        public string ConfigurationValue { get; set; }

        public DateTime DataCreateTime { get; set; }

        public bool IsEnabled { get; set; }

        public int ConfigurationKey { get; set; }
    }

    public static class ConstConfigurationName
    {
        public static string Douban = "douban";

        public static string PinPaiGongYu = "pinpaigongyu";

        public static string HuZhuZuFang = "huzhuzufang";

        public static string PeopleRenting = "peoplerenting";
    }


}
