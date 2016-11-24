using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace _58HouseSearch.Core.Controllers
{
    public class DoubanHomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(string groupID="")
        {
            PVHelper.WritePVInfo(Request.HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path);

            if (string.IsNullOrEmpty(groupID))
                groupID = "shanghaizufang";
            var url = $"https://www.douban.com/group/{groupID}/discussion?start=0";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var page = new HtmlParser().Parse(htmlResult);




            return View();
        }
    }
}
