using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMap.Common;
using HouseMapAPI.CommonException;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;
using HouseMapAPI.Models;

namespace HouseMapAPI.Service
{

    public class CollectionService
    {
        private readonly HouseMapContext _context;

        private readonly HouseService _houseService;


        private readonly HouseDapper _newHouseDapper;


        public CollectionService(HouseMapContext context, HouseService houseService,
        HouseDapper newHouseDapper)
        {
            _context = context;
            _houseService = houseService;
            _newHouseDapper = newHouseDapper;
        }

        public Object GetUserDashboards(long userId)
        {
            var list = _context.UserCollections.Where(c => c.UserID == userId && c.Deleted == 0)
            .GroupBy(c => c.City).Select(item => new
            {
                city = item.Key,
                id = item.First().Id,
                sources = item.GroupBy(i => i.Source).Select(g => new DBConfig()
                {
                    Id = g.First().Id,
                    City = g.First().City,
                    Source = g.Key,
                    HouseCount = g.Count()
                })
            });
            return list;
        }

        public List<DBHouse> FindUserCollections(long userId, string cityName = "", string source = "", string id = "")
        {
            var collections = _context.UserCollections.Where(c => c.UserID == userId).AsQueryable();
            if (!string.IsNullOrEmpty(cityName))
            {
                collections = collections.Where(c => c.City == cityName);
            }
            if (!string.IsNullOrEmpty(source))
            {
                collections = collections.Where(c => c.Source == source);
            }
            if (!string.IsNullOrEmpty(id))
            {
                collections = collections.Where(c => c.Id == id);
            }
            return collections.Where(c => c.Deleted == 0)
            .Select(c => JsonConvert.DeserializeObject<DBHouse>(c.HouseJson))
            .ToList();
        }

        public CollectionDetail FindUserCollection(long userId, string id)
        {
            var dbCollection = _context.UserCollections.Where(c => c.UserID == userId && c.Id == id).FirstOrDefault();
            if (dbCollection == null)
            {
                throw new NotFoundException("房源收藏不存在.");

            }
            var collectionDetail = dbCollection.ToModel<CollectionDetail>();
            var house = _houseService.FindById(dbCollection.HouseID);
            collectionDetail.House = house;
            return collectionDetail;
        }


        public DBUserCollection AddOne(long userId, string houseID)
        {
            var house = _houseService.FindById(houseID);
            if (house == null)
            {
                throw new NotFoundException("房源信息不存在,请刷新页面后重试");
            }
            if (_context.UserCollections.Any(c => c.HouseID == houseID && c.UserID == userId))
            {
                throw new UnProcessableException("房源信息已收藏.");
            }
            var collection = new DBUserCollection();
            collection.Source = house.Source;
            collection.Title = house.Title;
            collection.OnlineURL = house.OnlineURL;
            collection.City = house.City;
            collection.Id = Tools.GetGuid();
            collection.HouseID = houseID;
            collection.UserID = userId;
            collection.CreateTime = DateTime.Now;
            collection.HouseJson = JsonConvert.SerializeObject(house);
            _context.UserCollections.Add(collection);
            _context.SaveChanges();
            return collection;
        }

        public void RemoveOne(long userId, string collectionId)
        {
            var userCollection = _context.UserCollections.FirstOrDefault(c => c.UserID == userId && (c.Id == collectionId || c.HouseID == collectionId));
            if (userCollection == null)
            {
                throw new NotFoundException("收藏信息不存在,请重试.");
            }
            _context.UserCollections.Remove(userCollection);
            _context.SaveChanges();
        }
    }
}