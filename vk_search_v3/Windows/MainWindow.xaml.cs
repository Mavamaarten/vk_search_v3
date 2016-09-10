using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using vk_search_v3.Annotations;
using vk_search_v3.Model;
using vk_search_v3.ViewModel;

namespace vk_search_v3.Windows
{
    public partial class MainWindow : INotifyPropertyChanged
    {
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

        private DownloadsWindowViewModel downloadsWindowViewModel = new DownloadsWindowViewModel();
        public DownloadsWindowViewModel DownloadsWindowViewModel
        {
            get { return downloadsWindowViewModel; }
            set
            {
                if (Equals(value, downloadsWindowViewModel)) return;
                downloadsWindowViewModel = value;
                OnPropertyChanged();
            }
        }

        private DownloadWindow downloadWindow;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
            ViewModel.OnExceptionOccurred += PlayerOnOnExceptionOccurred;
            ViewModel.OnPlaybackSourceChanged += ViewModel_OnPlaybackSourceChanged;
            if (BinaryRage.DB.Exists("playlists", AppDomain.CurrentDomain.BaseDirectory))
            {
                var savedPlaylists = BinaryRage.DB.Get<IEnumerable<Playlist>>("playlists", AppDomain.CurrentDomain.BaseDirectory);
                ViewModel.Playlists = new ObservableCollection<Playlist>(savedPlaylists);
            }

            DataContext = ViewModel;

            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                ViewModel.SetAccessToken(loginWindow.AccessToken);
            }
            else
            {
                Close();
            }
        }

        private void ViewModel_OnPlaybackSourceChanged(object sender, MainWindowViewModel.PlaybackSources playbackSource)
        {
            switch (playbackSource)
            {
                case MainWindowViewModel.PlaybackSources.SEARCH_RESULTS:
                    lvPlaylists.SelectedIndex = 0;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
                case MainWindowViewModel.PlaybackSources.FAVORITES:
                    lvPlaylists.SelectedIndex = 1;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
                case MainWindowViewModel.PlaybackSources.PLAYLIST:
                    lvPlaylists.SelectedIndex = -1;
                    break;
                case MainWindowViewModel.PlaybackSources.QUEUE:
                    lvPlaylists.SelectedIndex = 2;
                    lvCustomPlaylists.SelectedIndex = -1;
                    break;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SearchTracks(txtSearch.Text);
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
                    ViewModel.PlaybackSource = MainWindowViewModel.PlaybackSources.SEARCH_RESULTS;
                    break;
                case 1:
                    ViewModel.PlaybackSource = MainWindowViewModel.PlaybackSources.FAVORITES;
                    break;
                case 2:
                    ViewModel.PlaybackSource = MainWindowViewModel.PlaybackSources.QUEUE;
                    break;
            }
        }

        private void BtnAddPlaylist_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var playlistNameWindow = new PlaylistNameWindow {Owner = this};
            if (playlistNameWindow.ShowDialog() == true)
            {
                ViewModel.CreatePlaylist(playlistNameWindow.PlaylistName);
            }
        }

        private void LvCustomPlaylists_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            ViewModel.SelectPlaylist((Playlist) e.AddedItems[0]);
        }

        private void PlayerOnOnExceptionOccurred(object sender, Exception exception)
        {
            MessageBox.Show(exception.Message, "An error occurred...", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LvTracks_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedTrack = (Track) lvTracks.SelectedItem;
            if (selectedTrack == null) return;
            ViewModel.PlayTrack(selectedTrack, ViewModel.PlaybackSource != MainWindowViewModel.PlaybackSources.QUEUE);
        }

        private void BtnNext_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Next();
        }

        private void PlayNextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedTrack = (Track)lvTracks.SelectedItem;
            if (selectedTrack == null) return;
            ViewModel.PlayNext(selectedTrack);
        }

        private void AddToQueueCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedTrack = (Track)lvTracks.SelectedItem;
            if (selectedTrack == null) return;
            ViewModel.AddToQueue(selectedTrack);
        }

        private void AddToPlaylistCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (lvTracks.SelectedItems == null) return;

            var selectPlaylistDialog = new SelectPlaylistWindow(viewModel.Playlists) {Owner = this};
            if (selectPlaylistDialog.ShowDialog() == true)
            {
                var playlist = selectPlaylistDialog.SelectedPlaylist;
                foreach (Track track in lvTracks.SelectedItems)
                {
                    playlist.Tracks.Add(track);
                }
            }
        }

        private void DownloadCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (lvTracks.SelectedItems == null) return;

            foreach (Track track in lvTracks.SelectedItems)
            {
                DownloadsWindowViewModel.Tracks.Add(track);
            }

            if (downloadWindow == null || !downloadWindow.IsLoaded)
            {
                downloadWindow = new DownloadWindow(downloadsWindowViewModel);
            }
            if (!downloadWindow.IsVisible)
            {
                downloadWindow.RemoveCompletedTracksFromQueue();
            }

            downloadWindow.Show();
        }

        private void BtnFavorite_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnPlayPause_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.PlayPause();
        }

        private void BtnPrevious_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Previous();
        }

        private void LvTracks_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                var gridView = listView.View as GridView;
                var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 12;
                const double col1 = 0.07;
                const double col2 = 0.27;
                const double col3 = 0.42;
                const double col4 = 0.12;
                const double col5 = 0.12;

                if (gridView != null)
                {
                    gridView.Columns[0].Width = workingWidth * col1;
                    gridView.Columns[1].Width = workingWidth * col2;
                    gridView.Columns[2].Width = workingWidth * col3;
                    gridView.Columns[3].Width = workingWidth * col4;
                    gridView.Columns[4].Width = workingWidth * col5;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ProgressCurrentTrack_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var percentage = e.GetPosition(progressCurrentTrack).X / progressCurrentTrack.ActualWidth;
            ViewModel.SetPosition(percentage);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            BinaryRage.DB.Insert("playlists", ViewModel.Playlists.AsEnumerable(), AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
