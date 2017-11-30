using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Jobs
{
    public class GCJob : Job
    {
        [Invoke(BeginS = "2017-11-30 04:30", Interval = 1000 * 3600 * 6, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("开始GC...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            GC.Collect();
            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("GC结束，花费时间：" + copyTime);
        }
    }
}
