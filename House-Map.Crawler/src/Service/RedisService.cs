using HouseMap.Crawler.Common;

using HouseMap.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace HouseMap.Crawler.Service
{
    public class RedisService
    {
        private APPConfiguration _configuration;

        public RedisService(IOptions<APPConfiguration> configuration)
        {
           _configuration = configuration.Value;
        }

        private ConfigurationOptions GetRedisOptions()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(_configuration.RedisConnectionString);
            options.SyncTimeout = 10 * 1000;
            return options;
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

        public void WriteSearchCache(string key, List<HouseInfo> house)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase();
                    db.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(house), new System.TimeSpan(0, 60, 0));
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteSearchCache", ex);
            }

        }



        public string ReadCache(string key)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase();
                    return db.KeyExists(key) == true ? db.StringGet(key).ToString() : null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("ReadCache", ex);
                return null;
            }


        }


        public void WriteCache(string key, string value)
        {

            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase();
                    db.StringSet(key, value, new System.TimeSpan(0, 30, 0));
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteCache", ex);
            }

        }
    }
}