using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using HouseMapAPI.Service;
using HouseMapAPI.Filters;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;

namespace HouseCrawler.Web.API.Controllers
{
    [Route("v2/users/")]
    public class NewCollectionController : ControllerBase
    {

        private CollectionService _collectionService;


        public NewCollectionController(CollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpGet("{userId}/collections/city-source")]
        [EnableCors("APICors")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult GetUserDashboard(long userId)
        {
            return Ok(new { success = true, data = _collectionService.GetUserDashboards(userId) });
        }

        [EnableCors("APICors")]
        [HttpGet("{userId}/collections/")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult List(long userId, string cityName, string source = "")
        {
            var rooms = _collectionService.FindUserCollections(userId, cityName, source);
            return Ok(new { success = true, data = rooms });
        }

        [EnableCors("APICors")]
        [HttpGet("{userId}/collections/{id}")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult GetOne(long userId, string id)
        {
            var houses = _collectionService.FindUserCollections(userId);
            return Ok(new { success = true, data = houses.FirstOrDefault(h => h.Id == id) });
        }

        [EnableCors("APICors")]
        [HttpPost("{userId}/collections/")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult Create(long userId, [FromBody] DBUserCollection userCollection)
        {
            userCollection.UserID = userId;
            _collectionService.AddOne(userCollection);
            return Ok(new { success = true, message = "收藏成功." }); ;
        }

        [EnableCors("APICors")]
        [HttpDelete("{userId}/collections/{id}")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult Remove(long userId, string id)
        {
            _collectionService.RemoveOne(userId, id);
            return Ok(new { success = true, message = "删除成功." });
        }


    }

}