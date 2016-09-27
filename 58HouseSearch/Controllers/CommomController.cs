using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _58HouseSearch.Controllers
{
    public class CommomController : Controller
    {
        //
        // GET: /Commom/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetPVCount()
        {
            try
            {
                var pvInfo = HTTPHelper.GetTheWebPVInfo(Server.MapPath("../pv.json"));
                return Json(new { IsSuccess = true, PVCount = pvInfo.PVCount });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false,Error = ex.ToString() });
            }
           
        }

    }
}
