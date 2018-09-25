using System;

namespace HouseMap.Common
{
    public static class RedisKey
    {
        public static KeyConfig Token = new KeyConfig()
        {
            Key = null,
            ExpireTime = new TimeSpan(TimeSpan.TicksPerDay * 14),
            DBName = 0
        };

        public static KeyConfig UserToken = new KeyConfig()
        {
            Key = "user_token_",
            ExpireTime = new TimeSpan(TimeSpan.TicksPerDay * 14),
            DBName = 0
        };

        public static KeyConfig UserId = new KeyConfig()
        {
            Key = "user_",
            ExpireTime = new TimeSpan(TimeSpan.TicksPerDay * 14),
            DBName = 0
        };


        public static KeyConfig Houses = new KeyConfig()
        {
            Key = null,
            ExpireTime = new TimeSpan(TimeSpan.TicksPerMinute * 55),
            DBName = 1
        };

        public static KeyConfig HouseDashboard = new KeyConfig()
        {
            Key = "HouseDashboard",
            ExpireTime = new TimeSpan(TimeSpan.TicksPerMinute * 60 * 12),
            DBName = 1
        };

        public static KeyConfig CityHouseSource = new KeyConfig()
        {
            Key = "CitySource-",
            ExpireTime = new TimeSpan(TimeSpan.TicksPerMinute * 60 * 12),
            DBName = 1
        };



        public static KeyConfig FindPasswordCode = new KeyConfig()
        {
            Key = null,
            ExpireTime = new TimeSpan(TimeSpan.TicksPerDay * 1),
            DBName = 0
        };

    }

    public class KeyConfig
    {
        public string Key { get; set; }

        public TimeSpan ExpireTime { get; set; }

        public int DBName { get; set; } = 0;
    }
}