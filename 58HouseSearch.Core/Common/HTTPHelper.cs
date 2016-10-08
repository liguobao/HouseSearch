using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Collections.Concurrent;
using _58HouseSearch.Core.Models;

namespace _58HouseSearch.Core
{
    public class HTTPHelper
    {

        public static string GetHTMLByURL(string Url, string type = "UTF-8")
        {
            try
            {
                Url = Url.ToLower();

                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponseAsync().Result;
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }

        }


        internal static WebPVInfo GetTheWebPVInfo()
        {
            return _webPVInfo;
        }

        private static WebPVInfo _webPVInfo;

        private static string _pvJsonPath;

        /// <summary>
        /// 获取PV数据
        /// </summary>
        /// <param name="pvJsonPath"></param>
        /// <returns></returns>
        public static WebPVInfo InitWebPVInfo(string pvJsonPath)
        {
            _pvJsonPath = pvJsonPath;
            if (_webPVInfo == null)
            {
                using (var stream = new FileStream(_pvJsonPath, FileMode.OpenOrCreate))
                {
                    try
                    {
                        StreamReader sr = new StreamReader(stream);

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Converters.Add(new JavaScriptDateTimeConverter());
                        serializer.NullValueHandling = NullValueHandling.Ignore;

                        //构建Json.net的读取流  
                        JsonReader reader = new JsonTextReader(sr);
                        //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                        _webPVInfo = serializer.Deserialize<WebPVInfo>(reader);
                        _webPVInfo.SalesLstPVInfo = new ConcurrentBag<PVInfo>();
                        if (_webPVInfo.LstPVInfo!=null)
                        {
                            _webPVInfo.LstPVInfo.ForEach(item=>_webPVInfo.SalesLstPVInfo.Add(item));
                        }
                        reader.Close();
                       // LogHelper.WriteLog("The First WebPVInfo Deserialize Success!");
                    }catch(Exception ex)
                    {
                        _webPVInfo = new WebPVInfo();
                        _webPVInfo.LstPVInfo = new List<PVInfo>();
                        _webPVInfo.SalesLstPVInfo = new ConcurrentBag<PVInfo>();

                       // LogHelper.WriteError("Deserialize WebPVInfo Exception", ex);
                    }
                }
            }
            return _webPVInfo;
        }

        /// <summary>
        /// 写入访问记录
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="userHostAddress"></param>
        /// <param name="actionAddress"></param>
        public static void WritePVInfo(string userHostAddress, string actionAddress)
        {
            try
            {
               AddPVLog( userHostAddress, actionAddress);

                if (_webPVInfo.PVCount % 10 == 0)
                {
                    WriteToJsonFile();
                }
            }
            catch (Exception ex)
            {
               // LogHelper.WriteError("WritePVInfo Exception", ex);
            }


        }

        /// <summary>
        /// 增加访问记录
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="userHostAddress"></param>
        /// <param name="actionAddress"></param>
        /// <returns></returns>
        private static WebPVInfo AddPVLog(string userHostAddress, string actionAddress)
        {

            _webPVInfo.SalesLstPVInfo.Add(new PVInfo()
            {
                PVIP = userHostAddress,
                PVTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                PVActionAddress = actionAddress
            });
            _webPVInfo.PVCount = _webPVInfo.SalesLstPVInfo.Count;
            return _webPVInfo;
        }

        /// <summary>
        /// PV数据写入文件
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="webPV"></param>
        public static void WriteToJsonFile()
        {
            using (var stream = new FileStream(_pvJsonPath, FileMode.OpenOrCreate))
            {
                StreamWriter sw = new StreamWriter(stream);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                //构建Json.net的写入流  
                JsonWriter writer = new JsonTextWriter(sw);
                //把模型数据序列化并写入Json.net的JsonWriter流中  

                _webPVInfo.LstPVInfo = new List<PVInfo>();
                foreach(var item in _webPVInfo.SalesLstPVInfo)
                {
                    _webPVInfo.LstPVInfo.Add(item);
                }
                serializer.Serialize(writer, _webPVInfo);
              
                //writer.Close();
                //sw.Flush();
            }
        }
    }

    
}