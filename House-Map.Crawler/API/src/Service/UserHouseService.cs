using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMapAPI.CommonException;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMap.Common;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;
using System.Linq;
using System.Collections.Generic;

namespace HouseMapAPI.Service
{

    public class UserHouseService
    {

        private readonly int PubTimeDay = 7;

        private HouseMapContext _context;

        public UserHouseService(HouseMapContext context)
        {
            _context = context;
        }

        public UserHouse AddOne(UserHouse newOne)
        {
            if (newOne == null)
            {
                throw new UnProcessableException("object can not empty.");
            }
            // todo check newOne field:title,text,price latitude,throw UnProcessableException

            if (CheckLastHousePubtime(newOne))
            {
                throw new UnProcessableException("7天只允许发布一条房源信息.");
            }
            newOne.Id = Tools.GetGuid();
            _context.UserHouses.Add(newOne);
            _context.SaveChanges();
            return newOne;
        }

        private bool CheckLastHousePubtime(UserHouse newOne)
        {
            return _context.UserHouses.Any(h => h.UserId == newOne.UserId && h.PubTime >= DateTime.Now.Date.AddDays(-PubTimeDay));
        }

        public UserHouse Modify(long userId, UserHouse update)
        {
            var modifyOne = _context.UserHouses.FirstOrDefault(h => h.Id == update.Id && h.UserId == userId);
            if (modifyOne == null)
            {
                throw new NotFiniteNumberException("house not found.");
            }
            modifyOne.Price = update.Price;
            modifyOne.Tags = update.Tags;
            modifyOne.Title = update.Title;
            modifyOne.Text = update.Text;
            modifyOne.RentType = update.RentType;
            modifyOne.Latitude = update.Longitude;
            modifyOne.Longitude = update.Longitude;
            modifyOne.Location = update.Location;
            modifyOne.UpdateTime = DateTime.Now;
            _context.SaveChanges();
            return modifyOne;
        }


        public List<UserHouse> GetUserHouses(long userId)
        {
            return _context.UserHouses.Where(h => h.UserId == userId)
            .ToList();
        }

    }
}