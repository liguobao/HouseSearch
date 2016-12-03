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
using System.Threading.Tasks;
using System.Linq;

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
                if(!File.Exists(pvJsonPath))
                {
                   var pvFile =  File.Create(pvJsonPath);
                    pvFile.Close();
                    _webPVInfo = new WebPVInfo();
                    _webPVInfo.LstPVInfo = new List<PVInfo>();
                    _webPVInfo.SalesLstPVInfo = new ConcurrentBag<PVInfo>();
                    return _webPVInfo;
                }

                using (StreamReader sr = new StreamReader(_pvJsonPath))
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
                        _webPVInfo.SalesLstPVInfo = new ConcurrentBag<PVInfo>();
                        if (_webPVInfo.LstPVInfo!=null)
                        {
                            _webPVInfo.LstPVInfo.ForEach(item=>_webPVInfo.SalesLstPVInfo.Add(item));
                        }
                        reader.Close();
                        LogHelper.WriteLog("The First WebPVInfo Deserialize Success!");
                    }catch(Exception ex)
                    {
                        _webPVInfo = new WebPVInfo();
                        _webPVInfo.LstPVInfo = new List<PVInfo>();
                        _webPVInfo.SalesLstPVInfo = new ConcurrentBag<PVInfo>();

                        LogHelper.WriteError("Deserialize WebPVInfo Exception", ex);
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
                LogHelper.WriteError("WritePVInfo Exception", ex);
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

            _webPVInfo.SalesLstPVInfo.Add(new PVInfo() { PVIP = userHostAddress, PVTime = DateTime.Now.ToString(), PVActionAddress = actionAddress });
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
           
            using (StreamWriter sw = new StreamWriter(_pvJsonPath))
            {
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
                //ser.Serialize(writer, ht);  
                writer.Close();
                sw.Close();
            }
        }
    }

    
}