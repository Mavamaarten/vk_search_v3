using System;
using System.Linq;
using System.Net;
using vk_search_v3.Model;

namespace vk_search_v3.Util
{
    class BitrateChecker
    {
        public static readonly int[] validBitrates = { 8, 0x10, 0x18, 0x20, 40, 0x30, 0x38, 0x40, 80, 0x60, 0x70, 0x80, 0x90, 160, 0xc0, 0xe0, 0x100, 320 };

        public static int CheckBitrate(Track track)
        {
            var request = (HttpWebRequest)WebRequest.Create(track.url);
            request.Method = "HEAD";

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                var ContentLength = Convert.ToInt64(response.Headers.Get("Content-Length"));
                response.Close();

                if (ContentLength == 0) return -1;

                var bitrate = (int) (ContentLength / 128 / track.duration);

                return validBitrates
                    .Concat(new[] { bitrate })
                    .First(validSize => Math.Abs(validSize - bitrate) < 10);
            }
            catch (Exception)
            {
                return -1;
            }
            
        }
    }
}
