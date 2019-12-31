using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace HouseCrawler.Web
{

    public class ViewResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }


        /// <summary>
        /// 错误信息
        /// </summary>
        public string error { get; set; }

    }

}