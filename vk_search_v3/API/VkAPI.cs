using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Refit;
using vk_search_v3.API.Exceptions;
using vk_search_v3.Model;

namespace vk_search_v3.API
{
    class VkAPI
    {
        public static readonly string BASE_URL = "https://api.vk.com/method";

        public string BaseUrl { get; private set; }
        public string AccessToken { get; set; }

        private readonly IvkAPI api;

        public VkAPI()
        {
            BaseUrl = BASE_URL;
            api = RestService.For<IvkAPI>(BASE_URL);
        }

        public VkAPI(string baseUrl, string accessToken)
        {
            BaseUrl = baseUrl;
            AccessToken = accessToken;
            api = RestService.For<IvkAPI>(baseUrl);
        }

        public async Task<IEnumerable<Track>> GetTracks(long? audio_ids, APIConstants need_user, int offset, int count)
        {
            var tracksResult = await api.GetTracks(audio_ids, need_user, offset, count, AccessToken);

            HandleAPIErrors(tracksResult.error);

            var resultCount = (long)tracksResult.response.First();
            if (resultCount == 0) return new List<Track>();

            var tracks = tracksResult.response.Skip(1).Cast<JObject>().Select(o => fixEncoding(o.ToObject<Track>()));
            return tracks;
        }

        public async Task<IEnumerable<Track>> GetTracksById(string audios)
        {
            var tracksResult = await api.GetTracksById(audios, AccessToken);

            HandleAPIErrors(tracksResult.error);

            var resultCount = (long)tracksResult.response.First();
            if (resultCount == 0) return new List<Track>();

            var tracks = tracksResult.response.Skip(1).Cast<JObject>().Select(o => fixEncoding(o.ToObject<Track>()));
            return tracks;
        }

        public async Task<IEnumerable<Track>> GetRecommendations(string target_audio, long? user_id, APIConstants shuffle, int count, int offset)
        {
            var recommendationsResult = await api.GetRecommendations(target_audio, user_id, shuffle, count, offset, AccessToken);

            HandleAPIErrors(recommendationsResult.error);

            var resultCount = (long)recommendationsResult.response.First();
            if (resultCount == 0) return new List<Track>();

            var tracks = recommendationsResult.response.Skip(1).Cast<JObject>().Select(o => fixEncoding(o.ToObject<Track>()));
            return tracks;
        }

        public async Task<IEnumerable<Track>> SearchAudio(string q, APIConstants auto_complete, int offset, int count)
        {
            var searchResult = await api.SearchAudio(q, auto_complete, offset, count, AccessToken);

            HandleAPIErrors(searchResult.error);

            var resultCount = (long)searchResult.response.First();
            if (resultCount == 0) return new List<Track>();

            var tracks = searchResult.response.Skip(1).Cast<JObject>().Select(o => fixEncoding(o.ToObject<Track>()));
            return tracks;
        }

        public async Task<long> AddTrack(long audio_id, long owner_id)
        {
            var result = await api.AddTrack(audio_id, owner_id, AccessToken);
            return result;
        }

        public async Task<long> DeleteTrack(long audio_id, long owner_id)
        {
            var result = await api.DeleteTrack(audio_id, owner_id, AccessToken);
            return result;
        }

        private Track fixEncoding(Track track)
        {
            track.artist = HttpUtility.HtmlDecode(track.artist).Trim();
            track.title = HttpUtility.HtmlDecode(track.title).Trim();
            return track;
        }

        private void HandleAPIErrors(APIError error)
        {
            if (error == null) return;

            switch (error.error_code)
            {
                case 5:
                    throw new UnauthorizedException(error.error_msg);

                default:
                    throw new UnknownAPIException(error.error_msg);
            }
        }
    }
}
