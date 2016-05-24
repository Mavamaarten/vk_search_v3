using System;
using System.Collections.Generic;
using vk_search_v3.API;
using vk_search_v3.Models;
using vk_search_v3.Playback;

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
        private readonly Mp3Player player;
        private PlayerStates playerState;

        public List<Track> SearchResultTracks { get; set; }
        public List<Track> QueueTracks { get; set; }
        public List<Track> FavoriteTracks { get; set; }
        public List<Playlist> Playlists { get; set; }
        public Track CurrentTrack { get; set; }
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
        public event EventHandler<List<Playlist>> OnPlaylistsChanged;
        public event EventHandler<PlayerStates> OnPlayerStateChanged;
        public event EventHandler<Track> OnCurrentTrackChanged;
        public event EventHandler<Exception> OnExceptionOccurred;
        public event EventHandler<Tuple<long, long>> OnPlaybackPositionUpdated;

        public VkPlayer(string access_token)
        {
            vkAPI = new VkAPI(VkAPI.BASE_URL, access_token);
            player = new Mp3Player();
            player.OnPlaybackEnded += Player_OnPlaybackEnded;
            player.OnPlaybackPositionUpdated += PlayerOnPlaybackPositionUpdated;

            SearchResultTracks = new List<Track>();
            QueueTracks = new List<Track>();
            FavoriteTracks = new List<Track>();
            Playlists = new List<Playlist>();
        }

        private void PlayerOnPlaybackPositionUpdated(object sender, Tuple<long, long> e)
        {
            OnPlaybackPositionUpdated?.Invoke(this, e);
        }

        private void Player_OnPlaybackEnded(object sender, EventArgs e)
        {
            //TODO play next track
        }

        public async void SearchTracks(string query)
        {
            try
            {
                var searchedTracks = await vkAPI.SearchAudio(query, APIConstants.TRUE, 0, 20);

                PlayerState = PlayerStates.SEARCH_RESULTS;
                OnPlayerStateChanged?.Invoke(this, PlayerStates.SEARCH_RESULTS);

                SearchResultTracks.Clear();
                SearchResultTracks.AddRange(searchedTracks);
                OnVisibleTracksChanged?.Invoke(this, SearchResultTracks);
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
                    OnVisibleTracksChanged?.Invoke(this, SearchResultTracks);
                    break;
                case PlayerStates.FAVORITES:
                    OnVisibleTracksChanged?.Invoke(this, FavoriteTracks);
                    break;
                case PlayerStates.QUEUE:
                    OnVisibleTracksChanged?.Invoke(this, QueueTracks);
                    break;
            }
        }

        public void CreatePlaylist(string playlistName)
        {
            Playlists.Add(new Playlist
            {
                Name = playlistName,
                Tracks = new List<Track>()
            });
            OnPlaylistsChanged?.Invoke(this, Playlists);
        }

        public void SelectPlaylist(Playlist playlist)
        {
            PlayerState = PlayerStates.PLAYLIST;
            OnVisibleTracksChanged?.Invoke(this, playlist.Tracks);
        }

        public void Play()
        {
            
        }

        public void PlayTrack(Track track)
        {
            track.playing = true;
            CurrentTrack = track;
            player.PlayUrl(track.url);
        }

        public void Pause()
        {
            
        }

        public void Next()
        {
            
        }
    }
}
