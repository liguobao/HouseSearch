
namespace HouseCrawler.Core.Common
{
    public class WebAPIHelper
    {
        /// <summary>
        /// 通过接口获取数据
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="apiURL">接口AIP地址</param>
        /// <returns>返回数据类型</returns>
        public static T GetAPIResult<T>(string apiURL)
        {
            var jsonResult = HTTPHelper.GetHTMLByURL(apiURL);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonResult);
        }
    }
}
