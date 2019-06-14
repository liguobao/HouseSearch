using System;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using HouseMapAPI.CommonException;
using HouseMapAPI.Service;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HouseMapAPI.Filters
{
    public class UserTokenFilter : ActionFilterAttribute
    {
        private RedisTool _RedisTool;

        public UserTokenFilter(RedisTool RedisTool)
        {
            _RedisTool = RedisTool;
        }



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.Keys.Contains("token"))
            {
                throw new TokenInvalidException("token not found.");
            }

            var token = context.HttpContext.Request.Headers["token"].ToString();
            var userInfo = _RedisTool.ReadCache<UserInfo>(token, RedisKeys.Token.DBName);
            if (userInfo == null)
            {
                throw new TokenInvalidException("token invalid.");
            }
            if (context.ActionArguments.ContainsKey("userId"))
            {
                var userId = (long)context.ActionArguments?["userId"];
                if (userInfo.ID != userId)
                {
                    throw new TokenInvalidException("token invalid.");
                }
            }
        }
    }
}