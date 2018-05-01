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

        [MaxLength(4096)]
        public string ConfigurationValue { get; set; }

        public DateTime DataCreateTime { get; set; }

        public bool IsEnabled { get; set; }

        public int ConfigurationKey { get; set; }
    }

    public static class ConstConfigurationName
    {
        public static string Douban = "douban";

        public static string PinPaiGongYu = "pinpaigongyu";

        public static string CCBHouse = "ccbhouse";

        public static string HuZhuZuFang = "huzhuzufang";

        public static string Zuber = "zuber";


        public static string MoguHouse = "mogu";

        public static string CityHouseInfo = "cityhouse";


        public static string ConvertToDisPlayName(string configurationName)
        {
            var dicNameToDisplayName = new Dictionary<string, string>()
            {
                { Douban,"豆瓣小組"},
                { PinPaiGongYu,"品牌公寓"},
                { HuZhuZuFang,"互助租房"},
                { Zuber,"Zuber平台"},
                { CCBHouse,"CCB建融家园"},
                { MoguHouse, "蘑菇租房" },
                { CityHouseInfo,"城市租房信息"},
            };

            return (dicNameToDisplayName.ContainsKey(configurationName)) ? dicNameToDisplayName[configurationName] : "";
        }
    }
}
