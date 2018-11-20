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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HouseMapAPI.Service
{
    public class NoticeService
    {

        private readonly HouseMapContext _dataContext;

        public NoticeService(HouseMapContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Notice FindNotice(long id)
        {
            var notice = _dataContext.Notices.FirstOrDefault(n => n.Id == id);
            return notice;
        }
    }
}