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

    public class UserCollectionService
    {


        private HouseDapper _houseDapper;


        private UserCollectionDapper _dapper;

        public UserCollectionService(HouseDapper houseDapper, UserCollectionDapper userCollectionDapper)
        {
            _houseDapper = houseDapper;
            _dapper = userCollectionDapper;
        }

        public Object GetUserDashboards(long userId)
        {
            var id = 1;
            var list = _dapper.LoadUserHouseDashboard(userId)
            .GroupBy(d => d.CityName)
            .Select(i => new
            {
                id = id++,
                cityName = i.Key,
                sources = i.ToList()
            });
            return list;
        }

        public List<HouseInfo> FindUserCollections(long userId, string cityName = "", string source = "")
        {
            return _dapper.FindUserCollections(userId, cityName, source);
        }

        public void AddOne(long userId, UserCollection userCollection)
        {
            var house = _houseDapper.GetHouseID(userCollection.HouseID, userCollection.Source);
            if (house == null)
            {
                throw new NotFoundException("房源信息不存在,请刷新页面后重试");
            }
            userCollection.UserID = userId;
            userCollection.Source = house.Source;
            userCollection.HouseCity = house.LocationCityName;
            _dapper.InsertUser(userCollection);
        }

        public void RemoveOne(long userId,long collectionId)
        {
            var userCollection = _dapper.FindByIDAndUserID(collectionId, userId);
            if (userCollection == null)
            {
                throw new NotFoundException("收藏信息不存在,请重试.");
            }
            _dapper.RemoveByIDAndUserID(userCollection.ID, userCollection.UserID);
        }
    }
}