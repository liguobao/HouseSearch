using HouseMapAPI.Common;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace HouseMapAPI.Service
{
    public class RedisService
    {
        private ConfigurationOptions GetRedisOptions()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(configuration.RedisConnectionString);
            options.SyncTimeout = 10 * 1000;
            return options;
        }

        private AppSettings configuration;

        public RedisService(IOptions<AppSettings> configuration)
        {
            this.configuration = configuration.Value;
        }


        public string ReadCache(string key, int dbName)
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public void WriteObject(string key, Object value, int dbName, int minutes = 60)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    db.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(value), new System.TimeSpan(0, minutes, 0));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public T ReadCache<T>(string key, int dbName)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    return db.KeyExists(key) == true ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(db.StringGet(key).ToString()) : default(T);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return default(T);
            }
        }




        public bool DelteCache(string key, int dbName = 0)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    return db.KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}