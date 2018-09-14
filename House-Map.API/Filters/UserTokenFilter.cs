using System;
using HouseMapAPI.Common;
using HouseMapAPI.CommonException;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Service;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HouseMapAPI.Filters
{
    public class UserTokenFilter : ActionFilterAttribute
    {
        private RedisService _redisService;

        public UserTokenFilter(RedisService redisService)
        {
            _redisService = redisService;
        }



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.Keys.Contains("token"))
            {
                var token = context.HttpContext.Request.Headers["token"].ToString();
                var userInfo = _redisService.ReadCache<UserInfo>(token, RedisKey.Token.DBName);
                if (userInfo == null)
                {
                    throw new TokenInvalidException("token invalid.");
                }
                if (context.ActionArguments.ContainsKey("userId"))
                {
                    var userId = (int)context.ActionArguments?["userId"];
                    if (userInfo.ID != userId)
                    {
                        throw new TokenInvalidException("token invalid.");
                    }
                }
            }
            else
            {
                throw new TokenInvalidException("token not found.");
            }
        }
    }
}