using NLog;
using System;

namespace HouseCrawler.Core
{
    public static class LogHelper
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();


        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Error(string message,Exception ex,object oj =null)
        {
            Logger.Error(message + ",Exception:" + ex.ToString(), ex, oj);
        }


        public static void RunActionNotThrowEx(Action action,string functionName,Object oj)
        {
            try
            {
                action.Invoke();

            }catch(Exception ex)
            {
                Logger.Info("关键数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(oj));
                Error(functionName, ex);
            }
        }
       
    }
}
