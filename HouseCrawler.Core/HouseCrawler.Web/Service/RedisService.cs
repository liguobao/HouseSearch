
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace HouseCrawler.Web
{
    public class RedisService
    {
        private ConfigurationOptions GetRedisOptions()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(configuration.RedisConnectionString);
            options.SyncTimeout = 10 * 1000;
            return options;
        }

        private APPConfiguration configuration;
        public RedisService(IOptions<APPConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }

        public List<HouseInfo> ReadSearchCache(string key)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase();
                    if (db.KeyExists(key))
                    {
                        string houseJson = db.StringGet(key);
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseInfo>>(houseJson);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error("ReadSearchCache", ex);
                return null;
            }

        }

        public void WriteSearchCache(string key, List<HouseInfo> houses)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase();
                    db.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(houses), new System.TimeSpan(0, 60, 0));
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteSearchCache", ex);
            }

        }



        public string ReadCache(string key, int dbName = 1)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    return db.KeyExists(key) == true ? db.StringGet(key).ToString() : null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ReadCache", ex);
                return null;
            }
        }


        public void WriteCache(string key, string value, int dbName = 1, int minutes = 60)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    db.StringSet(key, value, new System.TimeSpan(0, minutes, 0));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteCache", ex);
            }

        }
    }
}
