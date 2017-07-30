using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.TimedJob
{
    public class InvokeAttribute : Attribute
    {
        public bool IsEnabled { get; set; } = true;

        public int Interval { get; set; } = 1000 * 60 * 60 * 24; // 24 hours

        public bool SkipWhileExecuting { get; set; } = false;

        public string Begin
        {
            get { return _begin.ToString(); }
            set { _begin = Convert.ToDateTime(value); }
        }

        public DateTime _begin { get; set; }
    }
}
