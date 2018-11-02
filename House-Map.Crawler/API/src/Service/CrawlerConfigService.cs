using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using HouseMap.Common;

namespace HouseMapAPI.Service
{

    public class CrawlerConfigService
    {
        private HouseMapContext _dataContext;



        public CrawlerConfigService(HouseMapContext dataContext)
        {
            _dataContext = dataContext;

        }

        public void AddDoubanConfig(string groupId, string cityName)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(cityName))
            {
                throw new Exception("请输入豆瓣小组Group和城市名称。");
            }
            var result = DoubanService.CheckDBGroupData(groupId, cityName, 1);
            if (!result)
            {
                throw new Exception("保存失败!请检查豆瓣小组ID（如：XMhouse）/城市名称（如：厦门）是否正确...");
            }
            var json = $"{{ 'groupid':'{groupId}','cityname':'{cityName}','pagecount':5}}";
            if (_dataContext.Configs.Any(c => c.City == cityName && c.Source == SourceEnum.Douban.GetSourceName() && c.Json == json))
            {
                return;
            }
            var config = new DBConfig()
            {
                Id = Tools.GetUUId(),
                Json = json,
                City = cityName,
                Source = SourceEnum.Douban.GetSourceName(),
                CreateTime = DateTime.Now,
                PageCount = 10
            };
            _dataContext.Configs.Add(config);
            _dataContext.SaveChanges();
            return;
        }



    }

}