using System;
using System.ComponentModel;
using System.Linq;

namespace HouseMap.Common
{

    public static class Extensions
    {
        public static string GetEnumDescription<TEnum>(this TEnum item)
            => item.GetType()
                   .GetField(item.ToString())
                   .GetCustomAttributes(typeof(DescriptionAttribute), false)
                   .Cast<DescriptionAttribute>()
                   .FirstOrDefault()?.Description ?? string.Empty;

        public static string GetTableName<TEnum>(this TEnum item)
        => item.GetType()
               .GetField(item.ToString())
               .GetCustomAttributes(typeof(SourceAttribute), false)
               .Cast<SourceAttribute>()
               .FirstOrDefault()?.TableName ?? string.Empty;

        public static string GetSourceName<TEnum>(this TEnum item)
        => item.GetType()
               .GetField(item.ToString())
               .GetCustomAttributes(typeof(SourceAttribute), false)
               .Cast<SourceAttribute>()
               .FirstOrDefault()?.Name ?? string.Empty;
    }
}