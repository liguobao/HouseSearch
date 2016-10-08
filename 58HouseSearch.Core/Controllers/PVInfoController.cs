using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace _58HouseSearch.Core.Controllers
{
    public class PVInfoController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WritePVToJson()
        {
            HTTPHelper.WriteToJsonFile();
            return View(HTTPHelper.GetTheWebPVInfo());
            
        }

        public ActionResult GetPVCount()
        {
            try
            {
                var pvInfo = HTTPHelper.GetTheWebPVInfo();
                return Json(new { IsSuccess = true, PVCount = pvInfo.PVCount });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false,Error = ex.ToString() });
            }
           
        }

    }
}
