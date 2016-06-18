using System;
using System.Collections.ObjectModel;
using System.Threading;
using vk_search_v3.API;
using vk_search_v3.API.Bing;
using vk_search_v3.Base;
using vk_search_v3.Model;
using vk_search_v3.Playback;
using vk_search_v3.Util;

namespace vk_search_v3.ViewModel
{
    public class MainWindowViewModel : PropertyChangedNotifying
    {
        public enum PlaybackSources
        {
            SEARCH_RESULTS,
            FAVORITES,
            PLAYLIST,
            QUEUE
        }

        private readonly VkAPI vkAPI;
        private readonly Mp3Player player;
        private readonly IAlbumArtProvider albumArtProvider;
        private PlaybackSources playbackSource;
        private Track currentTrack;
        private Playlist currentlyVisiblePlaylist;
        private ObservableCollection<Playlist> playlists;
        private Tuple<long, long> playbackPosition;
        private Mp3Player.PlaybackStates playbackState;
        private bool _isLoading;
        private string elapsedTimeString;
        private string remainingTimeString;
        private string _currentTrackAlbumImage;

        public Track CurrentTrack
        {
            get { return currentTrack; }
            set
            {
                if (Equals(value, currentTrack)) return;
                currentTrack = value;
                OnPropertyChanged();
            }
        }
        public Playlist CurrentlyVisiblePlaylist
        {
            get { return currentlyVisiblePlaylist; }
            set
            {
                if (Equals(value, currentlyVisiblePlaylist)) return;
                currentlyVisiblePlaylist = value;
                OnPropertyChanged();
            }
        }
        public Playlist SelectedPlaylist { get; set; }
        public Playlist SearchResults { get; set; }
        public Playlist Queue { get; set; }
        public Playlist Favorites { get; set; }
        public ObservableCollection<Playlist> Playlists
        {
            get { return playlists; }
            set
            {
                if (Equals(value, playlists)) return;
                playlists = value;
                OnPropertyChanged();
            }
        }
        public PlaybackSources PlaybackSource
        {
            get
            {
                return playbackSource;
            }
            set
            {
                playbackSource = value;
                SetCurrentlyVisiblePlaylist(value);
                OnPlaybackSourceChanged?.Invoke(this, value);
                OnPropertyChanged();
            }
        }
        public Mp3Player.PlaybackStates PlaybackState
        {
            get { return playbackState; }
            set
            {
                if (value == playbackState) return;
                playbackState = value;
                OnPropertyChanged();
            }
        }
        public Tuple<long, long> PlaybackPosition
        {
            get { return playbackPosition; }
            set
            {
                if (Equals(value, playbackPosition)) return;
                playbackPosition = value;
                OnPropertyChanged();
            }
        }
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (value == _isLoading) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        public string ElapsedTimeString
        {
            get { return elapsedTimeString; }
            set
            {
                if (value == elapsedTimeString) return;
                elapsedTimeString = value;
                OnPropertyChanged();
            }
        }
        public string RemainingTimeString
        {
            get { return remainingTimeString; }
            set
            {
                if (value == remainingTimeString) return;
                remainingTimeString = value;
                OnPropertyChanged();
            }
        }
        public string CurrentTrackAlbumImage
        {
            get { return _currentTrackAlbumImage; }
            set
            {
                if (value == _currentTrackAlbumImage) return;
                _currentTrackAlbumImage = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<Exception> OnExceptionOccurred;
        public event EventHandler<PlaybackSources> OnPlaybackSourceChanged;

        public MainWindowViewModel()
        {
            vkAPI = new VkAPI();
            albumArtProvider = new BingAlbumArtProvider();
            player = new Mp3Player();
            player.OnPlaybackEnded += Player_OnPlaybackEnded;
            player.OnPlaybackStateChanged += Player_OnPlaybackStateChanged;
            player.OnPlaybackPositionUpdated += PlayerOnPlaybackPositionUpdated;

            SearchResults = new Playlist("Search results");
            Queue = new Playlist("Queue");
            Favorites = new Playlist("Favorites");
            Playlists = new ObservableCollection<Playlist>();
        }

        public MainWindowViewModel(string access_token)
        {
            vkAPI = new VkAPI(VkAPI.BASE_URL, access_token);
            player = new Mp3Player();
            player.OnPlaybackEnded += Player_OnPlaybackEnded;
            player.OnPlaybackStateChanged += Player_OnPlaybackStateChanged;
            player.OnPlaybackPositionUpdated += PlayerOnPlaybackPositionUpdated;

            SearchResults = new Playlist("Search results");
            Queue = new Playlist("Queue");
            Favorites = new Playlist("Favorites");
            Playlists = new ObservableCollection<Playlist>();
        }

        public void SetAccessToken(string accessToken)
        {
            vkAPI.AccessToken = accessToken;
        }

        private void Player_OnPlaybackStateChanged(object sender, Mp3Player.PlaybackStates playbackState)
        {
            PlaybackState = playbackState;
        }

        private void PlayerOnPlaybackPositionUpdated(object sender, Tuple<long, long> e)
        {
            PlaybackPosition = e;
            ElapsedTimeString = FormatUtil.secondsToShortTimespan((int) e.Item1);
            RemainingTimeString = "-" + FormatUtil.secondsToShortTimespan((int)e.Item2);
        }

        private void Player_OnPlaybackEnded(object sender, EventArgs e)
        {
            Next();
        }

        /// <summary>
        /// Searches for tracks
        /// </summary>
        /// <param name="query">The query to be searched</param>
        public async void SearchTracks(string query)
        {
            try
            {
                IsLoading = true;
                var searchedTracks = await vkAPI.SearchAudio(query, APIConstants.TRUE, 0, 100);

                SearchResults.Tracks = new ObservableCollection<Track>(searchedTracks);
                PlaybackSource = PlaybackSources.SEARCH_RESULTS;

                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                OnExceptionOccurred?.Invoke(this, ex);
            }
        }

        /// <summary>
        /// Gets called when the player state changes
        /// </summary>
        /// <param name="playbackSource">The current state</param>
        private void SetCurrentlyVisiblePlaylist(PlaybackSources playbackSource)
        {
            switch (playbackSource)
            {
                case PlaybackSources.SEARCH_RESULTS:
                    SelectedPlaylist = null;
                    CurrentlyVisiblePlaylist = SearchResults;
                    break;

                case PlaybackSources.FAVORITES:
                    SelectedPlaylist = null;
                    CurrentlyVisiblePlaylist = Favorites;
                    break;

                case PlaybackSources.QUEUE:
                    SelectedPlaylist = null;
                    CurrentlyVisiblePlaylist = Queue;
                    break;

                case PlaybackSources.PLAYLIST:
                    CurrentlyVisiblePlaylist = SelectedPlaylist;
                    break;
            }

            foreach (var t in CurrentlyVisiblePlaylist.Tracks)
            {
                new Thread(() =>
                {
                    if (t.bitrate != 0) return;
                    t.bitrate = BitrateChecker.CheckBitrate(t);
                }).Start();
            }
        }

        /// <summary>
        /// Creates a new playlist
        /// </summary>
        /// <param name="playlistName"></param>
        public void CreatePlaylist(string playlistName)
        {
            Playlists.Add(new Playlist(playlistName));
        }

        /// <summary>
        /// Sets a playlist as currently visible
        /// </summary>
        /// <param name="playlist"></param>
        public void SelectPlaylist(Playlist playlist)
        {
            SelectedPlaylist = playlist;
            CurrentlyVisiblePlaylist = playlist;
            PlaybackSource = PlaybackSources.PLAYLIST;
        }

        /// <summary>
        /// Resumes the playback
        /// </summary>
        public void Play()
        {
            player.Play();
        }

        /// <summary>
        /// Pauses the playback
        /// </summary>
        public void Pause()
        {
            player.Pause();
        }

        /// <summary>
        /// Plays a track using the Mp3Player class
        /// </summary>
        /// <param name="track">The track to be played</param>
        /// <param name="setQueue">If true, clears the queue and fills it up with the tracks that are currently visible</param>
        public async void PlayTrack(Track track, bool setQueue = true)
        {
            IsLoading = true;

            if (CurrentTrack != null) CurrentTrack.playing = false;
            track.playing = true;
            CurrentTrack = track;

            switch (PlaybackSource)
            {
                case PlaybackSources.SEARCH_RESULTS:
                    CurrentlyVisiblePlaylist = SearchResults;
                    break;

                case PlaybackSources.FAVORITES:
                    CurrentlyVisiblePlaylist = Favorites;
                    break;

                case PlaybackSources.PLAYLIST:
                    CurrentlyVisiblePlaylist = SelectedPlaylist;
                    break;

                case PlaybackSources.QUEUE:
                    CurrentlyVisiblePlaylist = Queue;
                    break;
            }

            if (setQueue)
            {
                Queue.Tracks.Clear();
                foreach(var t in CurrentlyVisiblePlaylist.Tracks)
                {
                    Queue.Tracks.Add(t);
                }
            }

            player.PlayUrl(track.url);
            CurrentTrackAlbumImage = await albumArtProvider.GetAlbumArtUrl(track.artist + " - " + track.title);
            IsLoading = false;
        }

        /// <summary>
        /// Adds a track in the queue right below the currently playing track
        /// </summary>
        /// <param name="track"></param>
        public void PlayNext(Track track)
        {  // POSSIBLE IMPROVEMENT maybe move the track up if it's already in queue?
            var currentIndex = -1;
            if(CurrentTrack != null) currentIndex = Queue.Tracks.IndexOf(CurrentTrack);
            Queue.Tracks.Insert(currentIndex + 1, track);
        }

        public void Next()
        {
            var currentIndex = Queue.Tracks.IndexOf(CurrentTrack);
            if (currentIndex >= 0 && Queue.Tracks.Count > currentIndex + 1)
            {
                var nextTrack = Queue.Tracks[currentIndex + 1];
                PlayTrack(nextTrack, false);
            }
        }

        public void Previous()
        {
            var currentIndex = Queue.Tracks.IndexOf(CurrentTrack);
            if (currentIndex >= 1 && Queue.Tracks.Count > 1)
            {
                var previousTrack = Queue.Tracks[currentIndex - 1];
                PlayTrack(previousTrack, false);
            }
        }

        public void PlayPause()
        {
            player.PlayPause();
        }

        /// <summary>
        /// Adds a track to the end of the queue
        /// </summary>
        /// <param name="track">The track to be added to the queue</param>
        public void AddToQueue(Track track)
        {
            Queue.Tracks.Add(track);
        }
    }
}
