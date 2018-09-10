using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HouseCrawler.Web.Service
{

    public class UserService
    {
        private APPConfiguration configuration;

        private RedisService redisService;

        public UserService(IOptions<APPConfiguration> configuration, RedisService redisService)
        {
            this.configuration = configuration.Value;
            this.redisService = redisService;
        }

        public UserInfo GetUserInfo(long userId, string token)
        {
            var userToken = redisService.ReadCache("user_token_" + userId, 0);
            if (userToken != null && userToken == token)
            {
                var userJson = redisService.ReadCache("user_" + userId, 0);
                return JsonConvert.DeserializeObject<UserInfo>(userJson);
            }
            return null;
        }

        public UserInfo GetUserByToken(string token)
        {
            var userJson = redisService.ReadCache(token, 0);
            if (!string.IsNullOrEmpty(userJson))
            {
                return JsonConvert.DeserializeObject<UserInfo>(userJson);
            }
            return null;
        }


        public void WriteUserToken(UserInfo loginUser, string token)
        {
            redisService.WriteCache("user_token_" + loginUser.ID, token, 0, 60 * 24 * 30);
            redisService.WriteCache("user_" + loginUser.ID, JsonConvert.SerializeObject(loginUser), 0, 60 * 24 * 30);
            redisService.WriteCache(token, JsonConvert.SerializeObject(loginUser), 0, 60 * 24 * 30);
        }
    }
}