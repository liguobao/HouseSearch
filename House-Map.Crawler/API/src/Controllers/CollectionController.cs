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
using Swashbuckle.AspNetCore.Annotations;
using HouseMapAPI.Models;

namespace HouseCrawler.Web.API.Controllers
{
    public class CollectionController : ControllerBase
    {

        private CollectionService _collectionService;


        public CollectionController(CollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        /// <summary>
        /// 获取用户收藏夹的来源列表
        /// </summary>
        [HttpGet("v2/users/{userId}/collections/city-source")]
        [EnableCors("APICors")]
        [ServiceFilter(typeof(UserTokenFilter))]
        [SwaggerResponse(200, "success", typeof(List<SourceDashboard>))]
        public IActionResult GetUserDashboard([FromRoute]long userId)
        {
            return Ok(new { success = true, data = _collectionService.GetUserDashboards(userId) });
        }

        /// <summary>
        /// 获取用户收藏的房源
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("v2/users/{userId}/collections/")]
        [ServiceFilter(typeof(UserTokenFilter))]
        [SwaggerResponse(200, "success", typeof(List<DBHouse>))]
        public IActionResult List([FromRoute]long userId, [FromQuery]string cityName = "", [FromQuery]string source = "")
        {
            var rooms = _collectionService.FindUserCollections(userId, cityName, source);
            return Ok(new { success = true, data = rooms });
        }

        /// <summary>
        /// 获取某一个收藏的房源信息
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("v2/users/{userId}/collections/{id}")]
        [ServiceFilter(typeof(UserTokenFilter))]
        [SwaggerResponse(200, "success", typeof(CollectionDetail))]
        public IActionResult GetOne([FromRoute]long userId, [FromRoute]string id)
        {
            var collection = _collectionService.FindUserCollection(userId, id);
            return Ok(new { success = true, data = collection });
        }

        /// <summary>
        /// 添加一个收藏
        /// </summary>
        [EnableCors("APICors")]
        [HttpPost("v2/users/{userId}/collections/")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult Create([FromRoute]long userId, [FromBody] CollectionDTO userCollection)
        {
            var collection = _collectionService.AddOne(userId, userCollection?.houseID);
            return Ok(new { success = true, message = "收藏成功.", data = collection });
        }

        /// <summary>
        /// 移除一个收藏
        /// </summary>
        [EnableCors("APICors")]
        [HttpDelete("v2/users/{userId}/collections/{id}")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult Remove([FromRoute]long userId, [FromRoute]string id)
        {
            _collectionService.RemoveOne(userId, id);
            return Ok(new { success = true, message = "删除成功." });
        }


    }

    public class CollectionDTO
    {
        public string houseID { get; set; }
    }

}