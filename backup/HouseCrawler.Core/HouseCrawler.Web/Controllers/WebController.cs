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
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

namespace HouseCrawler.Web.Controllers
{
    public class WebController : Controller
    {
        private IOAuthClient _authClient;

        private UserDataDapper _userDataDapper;

        private UserService _userService;

        private EncryptionTools _encryptionTools;


        public WebController(UserDataDapper userDataDapper,
                          RedisService redisService,
                          QQOAuthClient qqOAuthClient,
                          UserService userService,
                          EncryptionTools encryptionTools)
        {
            _authClient = qqOAuthClient.GetAPIOAuthClient();
            _userDataDapper = userDataDapper;
            _userService = userService;
            _encryptionTools = encryptionTools;
        }

        
        public ActionResult Callback(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var accessToken = _authClient.GetAccessToken(code).Result;
                    var qqUser = _authClient.GetUserInfo(accessToken).Result;
                    //未登录,通过此ID获取用户
                    var userInfo = _userDataDapper.FindUserByQQOpenUID(qqUser.Id);
                    if (userInfo == null)
                    {
                        //新增用户
                        _userDataDapper.InsertUserForQQAuth(new UserInfo() { UserName = qqUser.Name, QQOpenUID = qqUser.Id });
                        userInfo = _userDataDapper.FindUserByQQOpenUID(qqUser.Id);
                    }
                    string token = _encryptionTools.Crypt($"{userInfo.ID}|{userInfo.UserName}");
                    _userService.WriteUserToken(userInfo, token);
                    return Ok(new { success = true, token = token, message = "登录成功!", data = userInfo });
                }
                catch (Exception ex)
                {
                    return Ok(new { success = false, error = ex.ToString() });
                }
            }
            return Ok(new { success = false, error = "无效的auth code" });
        }




    }
}