using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMapAPI.CommonException;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using HouseMap.Common;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;

namespace HouseMapAPI.Service
{

    public class UserHouseService
    {

        private HouseDataContext _context;

        public UserHouseService(HouseDataContext context)
        {
            _context = context;
        }

        public UserHouse AddOne(UserHouse newOne)
        {
            if (newOne == null)
            {
                throw new UnProcessableException("object can not empty.");
            }
            newOne.Id = Tools.GetUUId();
            _context.UserHouses.Add(newOne);
            _context.SaveChanges();
            return newOne;
        }

    }
}