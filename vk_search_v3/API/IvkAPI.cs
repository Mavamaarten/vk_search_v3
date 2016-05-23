using System.Threading.Tasks;
using Refit;

namespace vk_search_v3.API
{
    interface IvkAPI
    {
        /// <summary>
        /// Returns a list of Track objects
        /// </summary>
        /// <param name="audio_ids">IDs of the audio files to return. </param>
        /// <param name="need_user">1 — to return information about users who uploaded audio files</param>
        /// <param name="offset">Offset needed to return a specific subset of audio files.</param>
        /// <param name="count">Number of audio files to return.</param>
        /// <param name="access_token">The access token to authorhize the request</param>
        /// <returns>a list of Track objects</returns>
        [Get("/audio.get")]
        Task<RestResponse> GetTracks(long? audio_ids, APIConstants need_user, int offset, int count, string access_token);

        /// <summary>
        /// Returns a list of Track objects
        /// </summary>
        /// <param name="audios">Audio file IDs, in the following format: {owner_id}_{audio_id}</param>
        /// <param name="access_token">The access token to authorhize the request</param>
        /// <returns>a list of Track objects</returns>
        [Get("/audio.getById")]
        Task<RestResponse> GetTracksById(string audios, string access_token);

        /// <summary>
        /// Returns a list of suggested audio files based on a user's playlist or a particular audio file.
        /// </summary>
        /// <param name="target_audio">Use to get recommendations based on a particular audio file. The ID of the user or community that owns an audio file and that audio file's ID, separated by an underscore.</param>
        /// <param name="user_id">Use to get recommendations based on a user's playlist. User ID. By default, the current user ID. </param>
        /// <param name="shuffle">1 - shuffle on</param>
        /// <param name="count">Number of audio files to return.</param>
        /// <param name="offset">Offset needed to return a specific subset of audio files.</param>
        /// <param name="access_token"></param>
        /// <returns> a list Track objects</returns>
        [Get("/audio.getById")]
        Task<RestResponse> GetRecommendations(string target_audio, long? user_id, APIConstants shuffle, int count, int offset, string access_token);

        /// <summary>
        /// Searches for audio, returns a list of Track objects.
        /// </summary>
        /// <param name="q">Search query string (e.g., The Beatles).</param>
        /// <param name="auto_complete">1 — to correct for mistakes in the search query (e.g., if you enter Beetles, the system will search for Beatles)</param>
        /// <param name="offset">Offset needed to return a specific subset of audio files.</param>
        /// <param name="count">Number of audio files to return.</param>
        /// <param name="access_token">The access token to authorhize the request</param>
        /// <returns>a list of Track objects</returns>
        [Get("/audio.search")]
        Task<RestResponse> SearchAudio(string q, APIConstants auto_complete, int offset, int count, string access_token);

        /// <summary>
        /// Copies an audio file to a user page or community page.
        /// </summary>
        /// <param name="audio_id">Track ID. </param>
        /// <param name="owner_id">ID of the user or community that owns the audio file.</param>
        /// <param name="access_token">The access token to authorhize the request</param>
        /// <returns>the ID of the created audio file.</returns>
        [Get("/audio.add")]
        Task<long> AddTrack(long audio_id, long owner_id, string access_token);

        /// <summary>
        /// Deletes an audio file from a user page or community page.
        /// </summary>
        /// <param name="audio_id">Track ID. </param>
        /// <param name="owner_id">ID of the user or community that owns the audio file.</param>
        /// <param name="access_token">The access token to authorhize the request</param>
        /// <returns>returns 1</returns>
        [Get("/audio.delete")]
        Task<long> DeleteTrack(long audio_id, long owner_id, string access_token);
    }
}
