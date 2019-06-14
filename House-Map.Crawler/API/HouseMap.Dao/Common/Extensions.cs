using System;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    public static class Tool
    {
        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }

    }
}