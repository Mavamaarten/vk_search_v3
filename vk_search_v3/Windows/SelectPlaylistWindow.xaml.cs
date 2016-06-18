using System.Collections.ObjectModel;
using System.Windows;
using vk_search_v3.Model;
using vk_search_v3.ViewModel;

namespace vk_search_v3.Windows
{
    public partial class SelectPlaylistWindow : Window
    {
        private readonly SelectPlaylistWindowViewModel viewModel;
        public Playlist SelectedPlaylist => viewModel?.SelectedPlaylist;

        public SelectPlaylistWindow(ObservableCollection<Playlist> playlists)
        {
            viewModel = new SelectPlaylistWindowViewModel
            {
                Playlists = playlists
            };
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = viewModel.SelectedPlaylist != null;
        }
    }
}
