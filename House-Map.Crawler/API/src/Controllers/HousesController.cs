using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMapConsumer.Dao.DBEntity;
using System.Threading.Tasks;
using HouseMapConsumer.Dao;
using Swashbuckle.AspNetCore.Annotations;

namespace HouseCrawler.Web.API.Controllers
{

    public class HousesController : ControllerBase
    {
        private HouseService _houseService;

        private HouseMongoMapper _houseMongoMapper;

        public HousesController(HouseService houseService, HouseMongoMapper houseMongoMapper)
        {
            _houseService = houseService;
            _houseMongoMapper = houseMongoMapper;
        }

        /// <summary>
        /// 查询租房数据（v3）
        /// </summary>
        [HttpPost("api/v3/houses")]
        [EnableCors("APICors")]
        public async Task<IActionResult> SearchHouses([FromBody]MongoHouseQuery search)
        {
            return Ok(new { success = true, data = await _houseMongoMapper.QueryHouses(search) });
        }

        /// <summary>
        /// 查询租房数据（v3）
        /// </summary>
        [HttpGet("api/v3/houses")]
        [EnableCors("APICors")]
        public async Task<IActionResult> GetHouses([FromQuery, SwaggerParameter("所在城市，默认上海", Required = true)]string city = "上海",
                                      [FromQuery, SwaggerParameter("来源")]string source = "",
                                      [FromQuery, SwaggerParameter("页码")]int page = 0,
                                      [FromQuery, SwaggerParameter("分页数量")]int pageSize = 20,
                                      [FromQuery, SwaggerParameter("房源类型")]int? rentType=null,
                                      [FromQuery, SwaggerParameter("最低价")]int fromPrice = 0,
                                      [FromQuery, SwaggerParameter("最高价")]int toPrice = 0,
                                      [FromQuery, SwaggerParameter("经度")]double? longitude = null,
                                      [FromQuery, SwaggerParameter("维度")]double? latitude = null,
                                      [FromQuery, SwaggerParameter("距离范围（公里）")]double distance = 5)
        {
            var condition = new MongoHouseQuery();
            condition.city = city;
            condition.source = source;
            condition.page = page;
            condition.pageSize = pageSize;
            condition.rentType = rentType;
            condition.fromPrice = fromPrice;
            condition.toPrice = toPrice;
            condition.latitude = latitude;
            condition.longitude = longitude;
            condition.distance = distance;
            return Ok(new { success = true, data = await _houseMongoMapper.QueryHouses(condition) });
        }


        /// <summary>
        /// 查询租房数据（v2）
        /// </summary>
        [HttpPost("v2/houses")]
        [EnableCors("APICors")]
        public IActionResult Search([FromBody] DBHouseQuery search)
        {
            return Ok(new { success = true, data = _houseService.Search(search) });
        }


        /// <summary>
        /// 查询租房数据
        /// </summary>
        [HttpGet("v2/houses")]
        [EnableCors("APICors")]
        public IActionResult GetHouse([FromQuery, SwaggerParameter("所在城市，默认上海", Required = true)]string city = "上海",
                                      [FromQuery, SwaggerParameter("来源")]string source = "",
                                      [FromQuery, SwaggerParameter("关键字")]string keyword = null,
                                      [FromQuery, SwaggerParameter("页码")]int page = 0,
                                      [FromQuery, SwaggerParameter("分页数量")]int size = 20,
                                      [FromQuery, SwaggerParameter("房源类型")]int rentType = 0,
                                      [FromQuery, SwaggerParameter("最低价")]int fromPrice = 0,
                                      [FromQuery, SwaggerParameter("最高价")]int toPrice = 0)
        {
            var condition = new DBHouseQuery();
            condition.City = city;
            condition.Source = source;
            condition.Keyword = keyword;
            condition.Page = page;
            condition.Size = size;
            condition.RentType = rentType;
            condition.FromPrice = fromPrice;
            condition.ToPrice = toPrice;
            return Ok(new { success = true, data = _houseService.Search(condition) });
        }

        /// <summary>
        /// 获取房源详细信息
        /// </summary>
        [HttpGet("v2/houses/{houseId}")]
        [EnableCors("APICors")]
        public IActionResult FindById([FromRoute, SwaggerParameter("房源Id")]string houseId,
                                      [FromQuery, SwaggerParameter("房源URL")] string onlineURL = "")
        {
            return Ok(new { success = true, data = _houseService.FindById(houseId, onlineURL) });
        }

        [HttpPost("v2/houses-refresh")]
        [EnableCors("APICors")]
        public IActionResult RefreshSearch([FromBody] DBHouseQuery search)
        {
            _houseService.RefreshSearch(search);
            return Ok(new { success = true });
        }


        /// <summary>
        /// 更新房源经纬度
        /// </summary>
        [HttpPut("v2/houses-lat-lng")]
        [EnableCors("APICors")]
        public IActionResult UpdateHousesLatLng([FromBody]List<HousesLatLng> houses)
        {
            _houseService.UpdateHousesLngLat(houses);
            return Ok(new { success = true });
        }
    }
}
