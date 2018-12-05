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

using Newtonsoft.Json.Linq;

namespace HouseCrawler.Web.API.Controllers
{
    [Route("v2/users/")]
    public class CollectionController : ControllerBase
    {

        private CollectionService _collectionService;


        public CollectionController(CollectionService collectionService)
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
        public IActionResult List(long userId, string cityName="", string source = "")
        {
            var rooms = _collectionService.FindUserCollections(userId, cityName, source);
            return Ok(new { success = true, data = rooms });
        }

        [EnableCors("APICors")]
        [HttpGet("{userId}/collections/{id}")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult GetOne(long userId, string id)
        {
            var collection = _collectionService.FindUserCollection(userId, id);
            return Ok(new { success = true, data = collection });
        }

        [EnableCors("APICors")]
        [HttpPost("{userId}/collections/")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult Create(long userId, [FromBody] JToken userCollection)
        {
            var houseID = userCollection?["houseID"]?.ToString();
            var collection = _collectionService.AddOne(userId, houseID);
            return Ok(new { success = true, message = "收藏成功.", data = collection });
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