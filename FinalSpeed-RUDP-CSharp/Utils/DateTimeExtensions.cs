using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.Utils
{
    public class DateTimeExtensions
    {
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long NanoTime() => (long)(Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000000000.0));
        public static long CurrentTimeMillis() => (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
    }
}
