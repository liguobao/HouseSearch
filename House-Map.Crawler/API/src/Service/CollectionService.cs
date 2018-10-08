using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using HouseMap.Common;
using HouseMapAPI.CommonException;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;

namespace HouseMapAPI.Service
{

    public class CollectionService
    {
        private readonly HouseDataContext _context;

        private readonly HouseService _houseService;

        public CollectionService(HouseDataContext context, HouseService houseService)
        {
            _context = context;
            _houseService = houseService;
        }

        public Object GetUserDashboards(long userId)
        {
            var list = _context.UserCollections.Where(c => c.UserID == userId)
            .GroupBy(c => c.City).Select(item => new
            {
                city = item.Key,
                id = item.First().Id,
                sources = item.GroupBy(i => i.Source).Select(g => new { source = g.Key, count = g.Count() })
            });
            return list;
        }

        public List<DBUserCollection> FindUserCollections(long userId, string cityName = "", string source = "")
        {
            var collections = _context.UserCollections.AsQueryable();
            if (!string.IsNullOrEmpty(cityName))
            {
                collections = collections.Where(c => c.City == cityName);
            }
            if (!string.IsNullOrEmpty(source))
            {
                collections = collections.Where(c => c.Source == source);
            }
            return collections.ToList();
        }

        public void AddOne(DBUserCollection collection)
        {
            var house = _houseService.FindById(collection.HouseID);
            if (house == null)
            {
                throw new NotFoundException("房源信息不存在,请刷新页面后重试");
            }
            if (_context.UserCollections.Any(c => c.HouseID == collection.HouseID && c.UserID == collection.UserID))
            {
                throw new NotFoundException("房源信息已收藏.");
            }
            collection.Id = Tools.GetUUId();
            collection.CreateTime = DateTime.Now;
            _context.UserCollections.Add(collection);
            _context.SaveChanges();
        }

        public void RemoveOne(long userId, string collectionId)
        {
            var userCollection = _context.UserCollections.FirstOrDefault(c => c.UserID == userId && c.Id == collectionId);
            if (userCollection == null)
            {
                throw new NotFoundException("收藏信息不存在,请重试.");
            }
            _context.UserCollections.Remove(userCollection);
            _context.SaveChanges();
        }
    }
}