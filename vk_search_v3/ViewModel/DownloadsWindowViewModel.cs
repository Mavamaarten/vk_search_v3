using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using vk_search_v3.Annotations;
using vk_search_v3.Model;

namespace vk_search_v3.ViewModel
{
    public class DownloadsWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Track> tracks;
        public ObservableCollection<Track> Tracks
        {
            get { return tracks; }
            set
            {
                tracks = value;
                OnPropertyChanged();
            }
        }

        public DownloadsWindowViewModel()
        {
            tracks = new ObservableCollection<Track>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}