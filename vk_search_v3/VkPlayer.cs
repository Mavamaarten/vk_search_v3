using System;
using System.Collections.Generic;
using vk_search_v3.API;

namespace vk_search_v3
{
    class VkPlayer
    {
        internal enum PlayerStates
        {
            SEARCH_RESULTS,
            FAVORITES,
            PLAYLIST,
            QUEUE
        }

        private readonly VkAPI vkAPI;
        private PlayerStates playerState;

        public List<Track> searchResultTracks { get; set; }
        public List<Track> queueTracks { get; set; }
        public List<Track> favoriteTracks { get; set; }
        public PlayerStates PlayerState
        {
            get
            {
                return playerState;
            }
            set
            {
                playerState = value;
                PlayerStateChanged(value);
            }
        }

        public event EventHandler<List<Track>> OnVisibleTracksChanged;
        public event EventHandler<PlayerStates> OnPlayerStateChanged;
        public event EventHandler<Exception> OnExceptionOccurred;

        public VkPlayer(string access_token)
        {
            vkAPI = new VkAPI(VkAPI.BASE_URL, access_token);
            searchResultTracks = new List<Track>();
            queueTracks = new List<Track>();
            favoriteTracks = new List<Track>();
        }

        public async void SearchTracks(string query)
        {
            try
            {
                var searchedTracks = await vkAPI.SearchAudio(query, APIConstants.TRUE, 0, 20);

                PlayerState = PlayerStates.SEARCH_RESULTS;
                OnPlayerStateChanged?.Invoke(this, PlayerStates.SEARCH_RESULTS);

                searchResultTracks.Clear();
                searchResultTracks.AddRange(searchedTracks);
                OnVisibleTracksChanged?.Invoke(this, searchResultTracks);
            }
            catch (Exception ex)
            {
                OnExceptionOccurred?.Invoke(this, ex);
            }
        }

        private void PlayerStateChanged(PlayerStates state)
        {
            OnPlayerStateChanged?.Invoke(this, state);
            switch (state)
            {
                case PlayerStates.SEARCH_RESULTS:
                    OnVisibleTracksChanged?.Invoke(this, searchResultTracks);
                    break;
                case PlayerStates.FAVORITES:
                    OnVisibleTracksChanged?.Invoke(this, favoriteTracks);
                    break;
                case PlayerStates.PLAYLIST:
                    OnVisibleTracksChanged?.Invoke(this, null);
                    break;
                case PlayerStates.QUEUE:
                    OnVisibleTracksChanged?.Invoke(this, queueTracks);
                    break;
            }
        }
    }
}
