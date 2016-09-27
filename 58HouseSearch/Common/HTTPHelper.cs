using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Converters;
using _58HouseSearch.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace _58HouseSearch
{
    public class HTTPHelper
    {
        public static string GetHTMLByURL(string url)
        {
            string htmlCode = string.Empty;
            try
            {
                HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                webRequest.Timeout = 30000;
                webRequest.Method = "GET";
                webRequest.UserAgent = "Mozilla/4.0";
                webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                HttpWebResponse webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();
                //获取目标网站的编码格式
                string contentype = webResponse.Headers["Content-Type"];
                Regex regex = new Regex("charset\\s*=\\s*[\\W]?\\s*([\\w-]+)", RegexOptions.IgnoreCase);
                if (webResponse.ContentEncoding.ToLower() == "gzip")//如果使用了GZip则先解压
                {
                    using (System.IO.Stream streamReceive = webResponse.GetResponseStream())
                    {
                        using (var zipStream = new System.IO.Compression.GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress))
                        {
                            //匹配编码格式
                            if (regex.IsMatch(contentype))
                            {
                                Encoding ending = Encoding.GetEncoding(regex.Match(contentype).Groups[1].Value.Trim());
                                using (StreamReader sr = new System.IO.StreamReader(zipStream, ending))
                                {
                                    htmlCode = sr.ReadToEnd();
                                }
                            }
                            else
                            {
                                using (StreamReader sr = new System.IO.StreamReader(zipStream, Encoding.UTF8))
                                {
                                    htmlCode = sr.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (System.IO.Stream streamReceive = webResponse.GetResponseStream())
                    {
                        var encoding = Encoding.Default;
                        if (contentype.Contains("utf"))
                            encoding = Encoding.UTF8;
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(streamReceive, encoding))
                        {
                            htmlCode = sr.ReadToEnd();
                        }

                    }
                }
                return htmlCode;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static WebPVInfo _webPVInfo;

        /// <summary>
        /// 获取PV数据
        /// </summary>
        /// <param name="pvJsonPath"></param>
        /// <returns></returns>
        public static WebPVInfo GetTheWebPVInfo(string pvJsonPath)
        {
            if (_webPVInfo == null)
            {
                using (StreamReader sr = new StreamReader(pvJsonPath))
                {
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Converters.Add(new JavaScriptDateTimeConverter());
                        serializer.NullValueHandling = NullValueHandling.Ignore;

                        //构建Json.net的读取流  
                        JsonReader reader = new JsonTextReader(sr);
                        //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                        _webPVInfo = serializer.Deserialize<WebPVInfo>(reader);
                        reader.Close();
                    }catch
                    {
                        _webPVInfo = new WebPVInfo();
                        _webPVInfo.LstPVInfo = new ConcurrentBag<PVInfo>();
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
        public static void WritePVInfo(string jsonPath, string userHostAddress, string actionAddress)
        {
            try
            {
                WebPVInfo webPV = AddPVLog(jsonPath, userHostAddress, actionAddress);

                WriteToJsonFile(jsonPath, webPV);
            }
            catch (Exception ex)
            {

            }


        }

        /// <summary>
        /// 增加访问记录
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="userHostAddress"></param>
        /// <param name="actionAddress"></param>
        /// <returns></returns>
        private static WebPVInfo AddPVLog(string jsonPath, string userHostAddress, string actionAddress)
        {
            var webPV = GetTheWebPVInfo(jsonPath);
            if (webPV == null)
            {
                webPV = new WebPVInfo();
                webPV.PVCount = 1;
                webPV.LstPVInfo = new ConcurrentBag<PVInfo>()
                    {
                        new PVInfo()
                        {
                            PVIP = userHostAddress,
                            PVTime = DateTime.Now.ToString(),
                            PVActionAddress = actionAddress
                        }
                    };
            }
            else
            {

                webPV.LstPVInfo.Add(new PVInfo() { PVIP = userHostAddress, PVTime = DateTime.Now.ToString(), PVActionAddress = actionAddress });
                webPV.PVCount = webPV.LstPVInfo.Count;
            }

            return webPV;
        }

        /// <summary>
        /// PV数据写入文件
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="webPV"></param>
        private static void WriteToJsonFile(string jsonPath, WebPVInfo webPV)
        {
            if (webPV.PVCount % 10 == 0)
            {

                using (StreamWriter sw = new StreamWriter(jsonPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    //构建Json.net的写入流  
                    JsonWriter writer = new JsonTextWriter(sw);
                    //把模型数据序列化并写入Json.net的JsonWriter流中  
                    serializer.Serialize(writer, webPV);
                    //ser.Serialize(writer, ht);  
                    writer.Close();
                    sw.Close();
                }
            }
        }
    }

    
}