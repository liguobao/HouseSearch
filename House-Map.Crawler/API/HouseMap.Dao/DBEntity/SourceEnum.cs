using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMap.Common;
using Newtonsoft.Json;

namespace HouseMap.Dao.DBEntity
{
    public enum SourceEnum
    {
        [Source("douban", "DoubanHouse", "豆瓣小组")]
        Douban,

        [Source("douban_wechat", "DoubanWechatHouse", "豆瓣租房")]
        DoubanWechat,

        [Source("pinpaigongyu", "ApartmentHouse", "公寓")]
        PinPaiGongYu,

        [Source("huzhuzufang", "HuzuHouse", "互助租房")]
        HuZhuZuFang,

        [Source("ccbhouse", "CCBHouse", "CCB建融")]
        CCBHouse,

        [Source("zuber", "ZuberHouse", "Zuber")]
        Zuber,

        [Source("hkspacious", "HKHouse", "千居")]
        HKSpacious,

        [Source("mogu", "MoguHouse", "蘑菇租房")]
        Mogu,
        [Source("baixing", "BaixingHouse", "百姓网")]
        BaiXing,

        [Source("baixing_wechat", "BaixingHouse", "百姓网")]
        BaixingWechat,

        [Source("beike", "BeikeHouse", "贝壳")]
        Beike,
        [Source("chengdufgj", "ChengduHouse", "成都房建局")]
        Chengdufgj,


        [Source("xianyu", "XianyuHouse", "闲鱼")]
        Xianyu,


        [Source("fangduoduo", "FangduoduoHouse", "房多多")]
        Fangduoduo,



        [Source("fangtianxia", "FangtianxiaHouse", "房天下")]
        Fangtianxia,


        [Source("hizhu", "HizhuHouse", "嗨住租房")]
        Hizhu,

        [Source("v2ex", "V2exHouse", "v2ex")]
        V2ex,

        [Source("pinshiyou", "PinshiyouHouse", "拼室友")]
        Pinshiyou,

        [Source("hezuzhaoshiyou", "HezuzhaoshiyouHouse", "合租找室友")]
        Hezuzhaoshiyou,

        [Source("baletu", "BaletuHouse", "巴乐兔租房")]
        Baletu,

        [Source("anjuke", "AnjukeHouse", "安居客租房")]
        Anjuke,

        [Source("ziroom", "ZiRoomHouse", "自如")]
        ZiRoom,

        [Source("qingke", "QingkeHouse", "青客")]
        Qingke,

        [Source("cjia", "CJiaHouse", "城家")]
        CJia,

        [Source("hangzhouzhufang", "GovernmentHouse", "杭州租赁")]
        Hangzhouzhufang,

        [Source("anxuan", "AnxuanHouse", "58安选")]
        Anxuan,

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

        public static string GetHouseTableName(string source)
        {
            string tableName = "doubanhouse";
            var dic = GetHouseTableNameDic();
            if (dic.ContainsKey(source))
            {
                return dic[source];
            }
            return tableName;

        }

        public static Dictionary<string, string> GetDescriptionDic()
        {
            var dic = new Dictionary<string, string>();
            foreach (SourceEnum sourceEnum in Enum.GetValues(typeof(SourceEnum)))
            {
                dic.Add(sourceEnum.GetSourceName(), sourceEnum.GetEnumDescription());
            }
            return dic;
        }

        public static string GetDisplayName(string source)
        {
            var displayName = "";
            var descriptionDic = GetDescriptionDic();
            if (descriptionDic.ContainsKey(source))
            {
                return descriptionDic[source];
            }
            return displayName;
        }

        internal static object GetHouseTableName(object source)
        {
            throw new NotImplementedException();
        }
    }

}