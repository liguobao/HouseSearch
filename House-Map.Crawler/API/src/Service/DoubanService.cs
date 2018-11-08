using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMap.Common;
using System.Linq;
using HouseMapAPI.CommonException;

namespace HouseMapAPI.Service
{
    public class DBGroupService
    {

        private readonly HouseMapContext _dataContext;

        public DBGroupService(HouseMapContext dataContext)
        {
            _dataContext = dataContext;

        }

        private bool CheckDBGroupData(string groupID, string cityName, int pageIndex)
        {
            var apiURL = $"https://api.douban.com/v2/group/{groupID}/topics?start={pageIndex * 50}";
            var result = GetAPIResult(apiURL);
            return !string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(JToken.Parse(result)?["topics"]?.ToString());
        }

        private static string GetAPIResult(string apiURL)
        {

            var client = new RestClient(apiURL);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("referer", apiURL);
            request.AddHeader("accept", "*/*");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("cookie", "bid=qLvbOle-G58; ps=y; ue=^\\^codelover^@qq.com^^; push_noty_num=0; push_doumail_num=0; __utmz=30149280.1521636704.1.1.utmcsr=(direct)^|utmccn=(direct)^|utmcmd=(none); __utmv=30149280.15460; ll=^\\^108296^^; _vwo_uuid_v2=D87414308A33790472DBB4D2B1DC0DE7B^|6a9fc300e5ea8c7485f9a922d87e820b; __utma=30149280.2046746446.1521636704.1522567163.1522675073.3");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";

        }



        public bool AddGroupConfig(string groupId, string city)
        {
            string json = PreDataCheck(groupId, city);
            var config = new DBConfig()
            {
                Id = Tools.GetGuid(),
                Json = json,
                City = city,
                Source = SourceEnum.Douban.GetSourceName(),
                CreateTime = DateTime.Now,
                PageCount = 10
            };
            _dataContext.Configs.Add(config);
            _dataContext.SaveChanges();
            return true;
        }

        private string PreDataCheck(string groupId, string city)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(city))
            {
                throw new UnProcessableException("请输入豆瓣小组Id和城市。");
            }
            var result = CheckDBGroupData(groupId, city, 1);
            if (!result)
            {
                throw new UnProcessableException("保存失败！请检查豆瓣小组ID或者城市名是否准确。");
            }
            var json = $"{{'groupid':'{groupId}','cityname':'{city}','pagecount':5}}";
            if (!_dataContext.Configs.Any(c => c.City == city))
            {
                throw new UnProcessableException($"暂不支持当前城市【{city}】或者城市名称不正确，如需添加请邮件联系管理员codelover@qq.com。");
            }
            if (_dataContext.Configs.Any(c => c.City == city && c.Source == SourceEnum.Douban.GetSourceName() && c.Json == json))
            {
                throw new UnProcessableException("保存失败！已存在当前豆瓣小组配置啦。");
            }

            return json;
        }
    }
}
