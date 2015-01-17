using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassPlayer.Classes
{
    internal static class Extensisons
    {
        public static string ToShortTime(this TimeSpan ts)
        {
            return string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
        }
    }
}
