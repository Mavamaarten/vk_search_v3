using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Refit;

namespace vk_search_v3.API
{
    class VkAPI
    {
        public static readonly string BASE_URL = "https://api.vk.com/method";

        public string BaseUrl { get; private set; }
        public string AccessToken { get; }

        private readonly IvkAPI api;

        public VkAPI(string baseUrl, string accessToken)
        {
            BaseUrl = baseUrl;
            AccessToken = accessToken;
            api = RestService.For<IvkAPI>(baseUrl);
        }

        public Task<RestResponse> GetTracks(long audio_ids, APIConstants need_user, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public Task<RestResponse> GetTracksById(string audios)
        {
            throw new NotImplementedException();
        }

        public Task<RestResponse> GetRecommendations(string target_audio, long? user_id, APIConstants shuffle, int count, int offset)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Track>> SearchAudio(string q, APIConstants auto_complete, int offset, int count)
        {
            var searchResult = await api.SearchAudio(q, auto_complete, offset, count, AccessToken);

            if (searchResult.error != null)
            {
                throw new Exception(searchResult.error.error_msg); //TODO: custom exceptions
            }

            var resultCount = (long)searchResult.response.First();
            if (resultCount == 0) return new List<Track>();

            var tracks = searchResult.response.Skip(1).Cast<JObject>().Select(o => fixEncoding(o.ToObject<Track>()));
            return tracks;
        }

        public Task<long> AddTrack(long audio_id, long owner_id)
        {
            throw new NotImplementedException();
        }

        public Task<long> DeleteTrack(long audio_id, long owner_id)
        {
            throw new NotImplementedException();
        }

        private Track fixEncoding(Track track)
        {
            track.artist = HttpUtility.HtmlDecode(track.artist).Trim();
            track.title = HttpUtility.HtmlDecode(track.title).Trim();
            track.playing = true;
            return track;
        }
    }
}
