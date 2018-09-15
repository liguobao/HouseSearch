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
using HouseMapAPI.DBEntity;

namespace HouseCrawler.Web.API.Controllers
{
    [Route("v1/users/")]
    public class CollectionsController : ControllerBase
    {

        private UserCollectionService _collectionService;


        public CollectionsController(UserCollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpGet("{userId}/[controller]/dashboard", Name = "GetUserDashboard")]
        [EnableCors("APICors")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult GetUserDashboard(long userId, [FromHeader] string token)
        {
            return Ok(new { success = true, data = _collectionService.GetUserDashboards(userId) });
        }

        [EnableCors("APICors")]
        [HttpGet("{userId}/[controller]/", Name = "ListCollection")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult List(long userId, [FromHeader] string token, string cityName, string source = "")
        {
            var rooms = _collectionService.FindUserCollections(userId, cityName, source);
            return Ok(new { success = true, data = rooms });
        }

        [EnableCors("APICors")]
        [HttpGet("{userId}/[controller]/{id}", Name = "GetOne")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult GetOne(long userId, [FromHeader] string token, long id)
        {
            var houses = _collectionService.FindUserCollections(userId);
            return Ok(new { success = true, data = houses.FirstOrDefault(h => h.Id == id) });
        }

        [EnableCors("APICors")]
        [HttpPost("{userId}/[controller]", Name = "CreateCollection")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult Create(long userId, [FromBody] UserCollection userCollection, [FromHeader] string token)
        {
            _collectionService.AddOne(userId, userCollection);
            return Ok(new { success = true, message = "收藏成功." }); ;
        }

        [EnableCors("APICors")]
        [HttpDelete("{userId}/[controller]/{id}", Name = "RemoveCollection")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult Remove(long userId, long id, [FromHeader] string token)
        {
            _collectionService.RemoveOne(userId, id);
            return Ok(new { success = true, message = "删除成功." });
        }


    }

}