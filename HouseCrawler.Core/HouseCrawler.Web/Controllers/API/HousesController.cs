using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Web.Models;
using HouseCrawler.Web.Common;
using HouseCrawler.Web;
using HouseCrawler.Web.Service;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Web.API.Controllers
{
    [Route("api/[controller]/")]
    public class HousesController : ControllerBase
    {
        private HouseDapper houseDapper;

        private HouseDashboardService houseDashboardService;

        private ConfigDapper configurationDapper;

        private UserCollectionDapper userCollectionDapper;


        public HousesController(HouseDapper houseDapper,
                              HouseDashboardService houseDashboardService,
                              ConfigDapper configurationDapper,
                              UserCollectionDapper userCollectionDapper)
        {
            this.houseDapper = houseDapper;
            this.houseDashboardService = houseDashboardService;
            this.configurationDapper = configurationDapper;
            this.userCollectionDapper = userCollectionDapper;
        }



        [HttpPost("", Name = "Search")]
        [EnableCors("APICors")]
        public IActionResult Search([FromBody] HouseSearchCondition search)
        {
            try
            {
                if (search == null || search.CityName == null)
                {
                    return Ok(new { success = false, error = "查询条件不能为null" });
                }
                var houseList = houseDapper.SearchHouses(search);
                return Ok(new { success = true, data = houseList });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.ToString() });
            }
        }

        [EnableCors("APICors")]
        [HttpGet("dashboard")]
        public IActionResult Dashboards()
        {
            var dashboards = houseDashboardService.LoadDashboard();
            return Ok(new { success = true, data = dashboards });
        }

    }
}
