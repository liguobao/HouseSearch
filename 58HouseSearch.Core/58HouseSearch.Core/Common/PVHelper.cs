using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using _58HouseSearch.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace _58HouseSearch.Core
{
    public class PVHelper
    {
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

                if (!File.Exists(pvJsonPath))
                {
                    var pvFile = File.Create(pvJsonPath);
                    pvFile.Flush();
                    _webPVInfo = new WebPVInfo()
                    {
                        LstPVInfo = new List<PVInfo>(),
                        SalesLstPVInfo = new ConcurrentBag<PVInfo>()
                    };
                    return _webPVInfo;
                }


                using (var stream = new FileStream(_pvJsonPath, FileMode.OpenOrCreate))
                {
                    try
                    {
                        StreamReader sr = new StreamReader(stream);
                        JsonSerializer serializer = new JsonSerializer
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            Converters = { new JavaScriptDateTimeConverter() }
                        };
                        //构建Json.net的读取流  
                        using (var reader = new JsonTextReader(sr))
                        {
                            _webPVInfo = serializer.Deserialize<WebPVInfo>(reader);
                            _webPVInfo.SalesLstPVInfo = new ConcurrentBag<PVInfo>();
                            _webPVInfo?.LstPVInfo.ForEach(item => _webPVInfo.SalesLstPVInfo.Add(item));
                        }
                        //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                        // LogHelper.WriteLog("The First WebPVInfo Deserialize Success!");
                    }
                    catch (Exception ex)
                    {
                        _webPVInfo = new WebPVInfo()
                        {
                            LstPVInfo = new List<PVInfo>(),
                            SalesLstPVInfo = new ConcurrentBag<PVInfo>()
                        };

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
                AddPVLog(userHostAddress, actionAddress);

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
                PVTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
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
                JsonSerializer serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = { new JavaScriptDateTimeConverter() }
                };
                //构建Json.net的写入流  
                JsonWriter writer = new JsonTextWriter(sw);
                //把模型数据序列化并写入Json.net的JsonWriter流中  
                _webPVInfo.LstPVInfo = new List<PVInfo>(_webPVInfo.SalesLstPVInfo);
                serializer.Serialize(writer, _webPVInfo);
                writer.Close();
                //sw.Flush();
            }
        }
    }
}
