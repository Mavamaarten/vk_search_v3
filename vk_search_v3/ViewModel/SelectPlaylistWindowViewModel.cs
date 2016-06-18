using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using vk_search_v3.Annotations;
using vk_search_v3.Model;

namespace vk_search_v3.ViewModel
{
    public class SelectPlaylistWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Playlist> _playlists;
        public ObservableCollection<Playlist> Playlists
        {
            get { return _playlists; }
            set
            {
                if (Equals(value, _playlists)) return;
                _playlists = value;
                OnPropertyChanged();
            }
        }

        public Playlist SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set
            {
                if (Equals(value, _selectedPlaylist)) return;
                _selectedPlaylist = value;
                OnPropertyChanged();
            }
        }
        private Playlist _selectedPlaylist;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}