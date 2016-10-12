using _58HouseSearch.Core.Models;
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
            var webPVInfo = HTTPHelper.GetTheWebPVInfo();
            Dictionary<DateTime, List<PVInfo>> dicDateToLstPVInfo = new Dictionary<DateTime, List<PVInfo>>();
            foreach(var pvInfo in webPVInfo.SalesLstPVInfo)
            {
                var pvTime = DateTime.MinValue;
                if(DateTime.TryParse(pvInfo.PVTime, out pvTime))
                {
                    if(dicDateToLstPVInfo.ContainsKey(pvTime.Date))
                    {
                        dicDateToLstPVInfo[pvTime.Date].Add(pvInfo);
                    }else
                    {
                        dicDateToLstPVInfo.Add(pvTime.Date, new List<PVInfo>() { pvInfo });
                    }
                }
            }

            return View(dicDateToLstPVInfo.OrderBy(item=>item.Key));
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
