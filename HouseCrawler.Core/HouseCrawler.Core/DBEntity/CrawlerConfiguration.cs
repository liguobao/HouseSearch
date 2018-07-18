using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public class CrawlerConfiguration
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

    public static class ConstConfigName
    {
        public static string Douban = "douban";

        public static string PinPaiGongYu = "pinpaigongyu";

        public static string CCBHouse = "ccbhouse";

        public static string HuZhuZuFang = "huzhuzufang";

        public static string HKSpacious = "hkspacious";

        public static string Zuber = "zuber";


        public static string MoguHouse = "mogu";

        public static string BaiXing = "baixing";

        public static string Beike = "beike";

        public static string Chengdufgj = "chengdufgj";

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
                { Beike, "贝壳租房" },
                { Chengdufgj, "成都房建局" },
                { CityHouseInfo,"城市租房信息"},
            };

            return (dicNameToDisplayName.ContainsKey(configurationName)) ? dicNameToDisplayName[configurationName] : "";
        }


        public static Dictionary<String, String> HouseTableNameDic = new Dictionary<string, string>() {
            { Douban, "DoubanHouseInfos"},
            { HuZhuZuFang, "MutualHouseInfos"},
            { PinPaiGongYu, "ApartmentHouseInfos"},
            { CCBHouse, "CCBHouseInfos"},
            { Zuber, "ZuberHouseInfos"},
            { MoguHouse, "MoguHouseInfos"},
            { HKSpacious, "HKHouseInfos"},
            { BaiXing, "BaiXingHouseInfos"},
            { Beike,"BeiKeHouseInfos"},
            { Chengdufgj,"ChengduHouseInfos"}
        };

        public static string GetTableName(string source)
        {
            if (ConstConfigName.HouseTableNameDic.ContainsKey(source))
            {
                return ConstConfigName.HouseTableNameDic[source];
            }

            return "HouseInfos";
        }
    }
}
