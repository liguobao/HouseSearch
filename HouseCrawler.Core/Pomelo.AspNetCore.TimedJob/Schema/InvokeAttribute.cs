using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.TimedJob
{
    public class InvokeAttribute : Attribute
    {
        public bool IsEnabled { get; set; } = true;

        public int Interval { get; set; } = 1000 * 60 * 60 * 24; // 24 hours

        public bool SkipWhileExecuting { get; set; } = false;

        public string BeginS
        {
            get => Begin.ToString(CultureInfo.InvariantCulture);
            set => Begin = Convert.ToDateTime(value);
        }

        public DateTime Begin { get; set; }
    }
}
