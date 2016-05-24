using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using vk_search_v3.Models;

namespace vk_search_v3
{
    public partial class MainWindow
    {
        public static readonly string access_token = "cc4b37f60d806d650d28c650c7dfde75c068699736ed17e21dbb9cee169357430b45dac5c9e9959f0bb98";
        private readonly VkPlayer player;

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                loadingIndicator.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            player = new VkPlayer(access_token);
            player.OnVisibleTracksChanged += Player_OnVisibleTracksChanged;
            player.OnPlaylistsChanged += Player_OnPlaylistsChanged;
            player.OnPlayerStateChanged += Player_OnPlayerStateChanged;
            player.OnExceptionOccurred += PlayerOnOnExceptionOccurred;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            player.SearchTracks(txtSearch.Text);
            Loading = true;
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
                    player.PlayerState = VkPlayer.PlayerStates.SEARCH_RESULTS;
                    break;
                case 1:
                    player.PlayerState = VkPlayer.PlayerStates.FAVORITES;
                    break;
                case 2:
                    player.PlayerState = VkPlayer.PlayerStates.QUEUE;
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

        private void Player_OnPlayerStateChanged(object sender, VkPlayer.PlayerStates state)
        {
            switch (state)
            {
                case VkPlayer.PlayerStates.SEARCH_RESULTS:
                    lvPlaylists.SelectedIndex = 0;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
                case VkPlayer.PlayerStates.FAVORITES:
                    lvPlaylists.SelectedIndex = 1;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
                case VkPlayer.PlayerStates.PLAYLIST:
                    lvPlaylists.SelectedIndex = -1;
                    break;
                case VkPlayer.PlayerStates.QUEUE:
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
            lvTracks.ItemsSource = tracks;
            lvTracks.Items.Refresh();
            Loading = false;
        }

        private void PlayerOnOnExceptionOccurred(object sender, Exception exception)
        {
            Loading = false;
            MessageBox.Show(exception.Message, "An error occurred...", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        private void LvTracks_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Track selectedTrack = (Track) lvTracks.SelectedItem;
            player.PlayTrack(selectedTrack);
        }
    }
}
