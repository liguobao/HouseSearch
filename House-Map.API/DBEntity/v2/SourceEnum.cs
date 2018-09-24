using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMapAPI.Common;
using Newtonsoft.Json;

namespace HouseMapAPI.DBEntity
{
    public enum SourceEnum
    {
        [Source("douban", "DoubanHouse", "豆瓣小组")]
        Douban,

        [Source("pinpaigongyu", "ApartmentHouse", "公寓")]
        PinPaiGongYu,

        [Source("huzhuzufang", "MutualHouse", "互助租房")]
        HuZhuZuFang,

        [Source("ccbhouse", "CCBHouse", "互助租房")]
        CCBHouse,

        [Source("zuber", "ZuberHouse", "Zuber")]
        Zuber,

        [Source("hkspacious", "HKHouse", "千居")]
        HKSpacious,

        [Source("mogu", "MoguHouse", "蘑菇租房")]
        Mogu,
        [Source("baixing", "BaixingHouse", "百姓网")]
        BaiXing,

        [Source("beike", "Beikehouse", "贝壳")]
        Beike,
        [Source("chengdufgj", "ChengduHouse", "成都房建局")]
        Chengdufgj


    }


    public static class SourceTool
    {
        public static Dictionary<string, string> GetHouseTableNameDic()
        {
            var dic = new Dictionary<string, string>();
            foreach (SourceEnum sourceEnum in Enum.GetValues(typeof(SourceEnum)))
            {
                dic.Add(sourceEnum.GetSourceName(), sourceEnum.GetTableName());
            }
            return dic;
        }
    }

}