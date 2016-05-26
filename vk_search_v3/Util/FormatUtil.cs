using System;

namespace vk_search_v3.Util
{
    static class FormatUtil
    {
        public static string secondsToShortTimespan(int seconds)
        {
            var timespan = new TimeSpan(0, 0, seconds);
            return timespan.ToString(timespan.Hours > 0 ? "g" : "mm\\:ss");
        }
    }
}
