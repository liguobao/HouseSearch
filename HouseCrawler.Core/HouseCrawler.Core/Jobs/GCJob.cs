using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Jobs
{
    public class GCJob : Job
    {
        [Invoke(Begin = "2017-08-05 04:30", Interval = 1000 * 3600 * 24, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始GC...");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            System.GC.Collect();
            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString();
            LogHelper.Info("GC结束，花费时间：" + copyTime);
        }
    }
}
