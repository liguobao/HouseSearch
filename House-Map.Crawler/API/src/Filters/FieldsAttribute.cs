using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using HouseMapAPI.CommonException;
using Newtonsoft.Json.Serialization;

namespace HouseMapAPI.Filters
{
    public class FieldsFilterAttribute : ActionFilterAttribute
    {

        public FieldsFilterAttribute()
        {
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Query.ContainsKey("fields") && context.Result is ObjectResult)
            {
                var fields = context.HttpContext.Request.Query["fields"].ToString();
                CheckFieldsStyle(fields);
                var fieldList = fields.Split(",");
                var objResult = (ObjectResult)context.Result;
                var jToken = JToken.FromObject(objResult.Value, new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                if (!jToken["data"].Any())
                {
                    return;
                }
                ExceptObjectFields(jToken["data"], fieldList);
                FillResultValues(fieldList, objResult, jToken);
            }

            if (context.HttpContext.Request.Query.ContainsKey("count") && context.Result is ObjectResult)
            {
                var index = context.HttpContext.Request.Query["index"];
                var count = context.HttpContext.Request.Query["count"];
                var objResult = (ObjectResult)context.Result;
                var jToken = JToken.FromObject(objResult.Value, new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                if (!jToken["data"].Any())
                {
                    return;
                }
                CutValues(objResult, jToken, index, count);
            }


        }

        private static void FillResultValues(string[] fieldList, ObjectResult objResult, JToken jToken)
        {
            if (!jToken["data"].Any())
            {
                return;
            }
            if (jToken["data"].Type == JTokenType.Array)
            {
                var data = jToken["data"].AsEnumerable();
                var dataDic = new List<Dictionary<string, JToken>>();
                foreach (var item in data)
                {
                    dataDic.Add(fieldList.ToDictionary(f => f, f => item[f]));
                }
                jToken["data"] = JToken.FromObject(dataDic);

            }
            else
            {
                jToken["data"] = JToken.FromObject(fieldList.ToDictionary(f => f, f => jToken["data"][f]));
            }
            objResult.Value = jToken;
        }

        private static void CutValues(ObjectResult objResult, JToken jToken, string index, string count)
        {
            if (!jToken["data"].Any())
            {
                return;
            }
            if (jToken["data"].Type == JTokenType.Array)
            {
                var data = jToken["data"].AsEnumerable();
                if (!string.IsNullOrEmpty(count) && !string.IsNullOrEmpty(index))
                {
                    data = data.Skip(int.Parse(index)).Take(int.Parse(count));
                }
                jToken["data"] = JToken.FromObject(data);
                objResult.Value = jToken;
            }

        }

        private static void CheckFieldsStyle(string fields)
        {
            if (string.IsNullOrEmpty(fields) || !fields.Split(",").Any())
            {
                throw new UnProcessableException($"unable handle the '{fields}' fields.");
            }
        }

        private static void ExceptObjectFields(JToken result, string[] fieldList)
        {
            if (!result.Any())
            {
                return;
            }
            JToken firstItem = result.Type == JTokenType.Array
            ? result.First()
            : result;
            var bookFields = firstItem.ToObject<JObject>().Properties().Select(p => p.Name).ToList();
            if (fieldList.Except(bookFields).Any())
            {
                throw new UnProcessableException($"data not contain '{string.Join(",", fieldList.Except(bookFields))}' fields");
            }
        }
    }
}