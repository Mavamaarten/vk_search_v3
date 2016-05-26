using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using vk_search_v3.Annotations;
using vk_search_v3.Model;
using vk_search_v3.Playback;
using vk_search_v3.Util;
using vk_search_v3.ViewModel;
using vk_search_v3.Windows;

namespace vk_search_v3
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly VkPlayer player;

        private MainWindowViewModel viewModel;
        public MainWindowViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                viewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;

            LoginWindow loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                player = new VkPlayer(loginWindow.AccessToken);
                player.OnLoadingChanged += Player_OnLoadingChanged;
                player.OnVisibleTracksChanged += Player_OnVisibleTracksChanged;
                player.OnPlaylistsChanged += Player_OnPlaylistsChanged;
                player.OnPlaybackSourceChanged += PlayerOnPlaybackSourceChanged;
                player.OnPlaybackPositionUpdated += Player_OnPlaybackPositionUpdated;
                player.OnCurrentTrackChanged += Player_OnCurrentTrackChanged;
                player.OnExceptionOccurred += PlayerOnOnExceptionOccurred;
            }
            else
            {
                Close();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            player.SearchTracks(txtSearch.Text);
        }

        private void TxtSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return) btnSearch_Click(sender, null);
        }

        private void LvPlaylists_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (lvPlaylists.SelectedIndex)
            {
                case 0:
                    player.PlaybackSource = VkPlayer.PlaybackSources.SEARCH_RESULTS;
                    break;
                case 1:
                    player.PlaybackSource = VkPlayer.PlaybackSources.FAVORITES;
                    break;
                case 2:
                    player.PlaybackSource = VkPlayer.PlaybackSources.QUEUE;
                    break;
            }
        }

        private void BtnAddPlaylist_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            player.CreatePlaylist("Swaglord");
        }

        private void LvCustomPlaylists_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            player.SelectPlaylist((Playlist) e.AddedItems[0]);
        }

        #region Player events

        private void Player_OnLoadingChanged(object sender, bool loading)
        {
            ViewModel.Loading = loading;
        }

        private void PlayerOnPlaybackSourceChanged(object sender, VkPlayer.PlaybackSources state)
        {
            switch (state)
            {
                case VkPlayer.PlaybackSources.SEARCH_RESULTS:
                    lvPlaylists.SelectedIndex = 0;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
                case VkPlayer.PlaybackSources.FAVORITES:
                    lvPlaylists.SelectedIndex = 1;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
                case VkPlayer.PlaybackSources.PLAYLIST:
                    lvPlaylists.SelectedIndex = -1;
                    break;
                case VkPlayer.PlaybackSources.QUEUE:
                    lvPlaylists.SelectedIndex = 2;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
            }
        }

        private void Player_OnPlaylistsChanged(object sender, List<Playlist> playlists)
        {
            lvCustomPlaylists.ItemsSource = playlists;
            lvCustomPlaylists.Items.Refresh();
        }

        private void Player_OnVisibleTracksChanged(object sender, List<Track> tracks)
        {
            ViewModel.Tracks.Clear();
            tracks.ForEach(t => ViewModel.Tracks.Add(t));
        }

        private void Player_OnPlaybackPositionUpdated(object sender, Tuple<long, long> e)
        {
            ViewModel.ElapsedTime = FormatUtil.secondsToShortTimespan((int) e.Item1);
            ViewModel.RemainingTime = "-" + FormatUtil.secondsToShortTimespan((int) e.Item2);

            ViewModel.CurrentTrackPosition = e.Item1;
        }

        private void Player_OnCurrentTrackChanged(object sender, Track track)
        {
            ViewModel.CurrentTrackArtist = track.artist;
            ViewModel.CurrentTrackTitle = track.title;
            ViewModel.CurrentTrackPosition = 0;
            ViewModel.CurrentTrackLength = track.duration;
        }

        private void PlayerOnOnExceptionOccurred(object sender, Exception exception)
        {
            MessageBox.Show(exception.Message, "An error occurred...", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        private void LvTracks_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedTrack = (Track) lvTracks.SelectedItem;
            if (selectedTrack == null) return;
            player.PlayTrack(selectedTrack, player.PlaybackSource != VkPlayer.PlaybackSources.QUEUE);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnNext_OnClick(object sender, RoutedEventArgs e)
        {
            player.Next();
        }

        private void PlayNextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedTrack = (Track)lvTracks.SelectedItem;
            if (selectedTrack == null) return;
            player.PlayNext(selectedTrack);
        }

        private void AddToQueueCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedTrack = (Track)lvTracks.SelectedItem;
            if (selectedTrack == null) return;
            player.AddToQueue(selectedTrack);
        }

        private void AddToPlaylistCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DownloadCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnFavorite_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnPlayPause_OnClick(object sender, RoutedEventArgs e)
        {
            player.PlayPause();
        }

        private void BtnPrevious_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
