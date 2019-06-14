
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMap.Common;
using Newtonsoft.Json;

namespace HouseMap.Dao.DBEntity
{
    public enum IntentionEnum
    {

        [Source("douban_wechat", "DoubanWechatPeople", "豆瓣求租")]
        DoubanWechat,

        [Source("zuber", "ZuberPeople", "Zuber求租")]
        Zuber,
    }


    public static class IntentionSourceTool
    {
        public static Dictionary<string, string> GetHouseTableNameDic()
        {
            var dic = new Dictionary<string, string>();
            foreach (IntentionEnum sourceEnum in Enum.GetValues(typeof(IntentionEnum)))
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
            foreach (IntentionEnum sourceEnum in Enum.GetValues(typeof(IntentionEnum)))
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
    }
}
