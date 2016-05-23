using System;

namespace vk_search_v3
{
    public class Track
    {
        public long Id { get; set; }
        public long aid { get; set; }
        public long owner_id { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public bool playing { get; set; }
        public int bitrate { get; set; }
        public int duration { get; set; }
        public long album_id { get; set; }
        public long lyrics_id { get; set; }
        public long genre_id { get; set; }

        public string DurationString
        {
            get
            {
                var timespan = new TimeSpan(0, 0, duration);
                if (timespan.Hours > 0)
                    return timespan.ToString("g");
                else
                    return timespan.ToString("mm\\:ss");
            }
        }
    }
}