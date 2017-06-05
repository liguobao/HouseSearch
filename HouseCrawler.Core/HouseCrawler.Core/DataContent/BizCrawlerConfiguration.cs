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

        public static string HuZhuZuFang = "huzhuzufang";

        public static string CityHouseInfo = "cityhouse";


        public static string ConvertToDisPlayName(string configurationName)
        {
            Dictionary<string, string> dicNameToDisplayName = new Dictionary<string, string>()
            {
                { ConstConfigurationName.Douban,"豆瓣小組"},
                { ConstConfigurationName.PinPaiGongYu,"品牌公寓"},
                { ConstConfigurationName.HuZhuZuFang,"互助租房"},
                { ConstConfigurationName.CityHouseInfo,"城市租房信息"},
            };

            return (dicNameToDisplayName.ContainsKey(configurationName))? dicNameToDisplayName[configurationName]:"";
        }
    }





}
