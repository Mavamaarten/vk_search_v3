using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace vk_search_v3
{
    public partial class MainWindow
    {
        public static readonly string access_token = "83caf2471405b8764766c48a77eeb3bfaf4e53c7de6de930272c78d5a3d564be19316049c4163fe6bc0c2";
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
            player.OnExceptionOccurred += PlayerOnOnExceptionOccurred;
        }

        private void Player_OnVisibleTracksChanged(object sender, System.Collections.Generic.List<Track> tracks)
        {
            lvTracks.ItemsSource = tracks;
            lvTracks.Items.Refresh();
            Loading = false;
        }

        private void PlayerOnOnExceptionOccurred(object sender, Exception exception)
        {
            Loading = false;
            MessageBox.Show(exception.Message, "An error occurred..." , MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            player.SearchTracks(txtSearch.Text);
            Loading = true;
        }

        private void TxtSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Return) btnSearch_Click(sender, null);
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
    }
}
