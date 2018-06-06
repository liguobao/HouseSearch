using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Common
{
    public class RedisService
    {
        private APPConfiguration config;
        public RedisService(IOptions<APPConfiguration> configuration)
        {
            this.config = configuration.Value;
        }
        
        public bool ContainsHouse(string key, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer
                .Connect(config.RedisConnectionString);
            IDatabase db = redis.GetDatabase();
            if (db.KeyExists(key))
            {
                return true;
            }
            else
            {
                db.StringSet(key, "");
                return false;
            }
        }
    }
}
