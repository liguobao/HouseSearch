using NLog;
using System;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public static class LogHelper
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();


        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Error(string message, Exception ex, object oj = null)
        {
            Logger.Error(message + ",Exception:" + ex.ToString(), ex, oj);
        }


        public static void RunActionNotThrowEx(Action action, string functionName = "default", Object oj = null)
        {
            try
            {
                action.Invoke();

            }
            catch (Exception ex)
            {
                if (oj != null)
                {
                    Logger.Info("关键数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(oj));
                }
                Error(functionName, ex);
            }
        }

        
        public static void RunActionTaskNotThrowEx(Action action, string functionName = "default", Object oj = null)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    if (oj != null)
                    {
                        Logger.Info("关键数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(oj));
                    }
                    Error(functionName, ex);
                }
            });
        }

    }
}
