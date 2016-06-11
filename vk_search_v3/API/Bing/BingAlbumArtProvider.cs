using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace vk_search_v3.API.Bing
{
    class BingAlbumArtProvider : IAlbumArtProvider
    {
        private const string image_delimiter = "\" class=\"thumb\" ";

        public async Task<string> GetAlbumArtUrl(string albumName)
        {
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "Googlebot/2.1 (+http://www.google.com/bot.html)");

            var q = HttpUtility.UrlEncode(albumName + " Album");
            var searchResult = await client.DownloadStringTaskAsync("http://www.bing.com/?q=" + q + "&scope=images&qft=+filterui:aspect-square");

            return ParseImageUrlFromResult(searchResult);
        }

        private string ParseImageUrlFromResult(string result)
        {
            if (!result.Contains(image_delimiter)) return null;
            result = result.Split(new [] {image_delimiter}, StringSplitOptions.None)[0];
            result = result.Substring(result.LastIndexOf('"') + 1);
            return result;
        }
    }
}
