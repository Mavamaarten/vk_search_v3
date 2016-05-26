using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using vk_search_v3.Annotations;
using vk_search_v3.Model;

namespace vk_search_v3.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string elapsedTime = "0:00";
        public string ElapsedTime
        {
            get { return elapsedTime;}
            set
            {
                elapsedTime = value;
                OnPropertyChanged();
            }
        }

        private string remainingTime = "-0:00";
        public string RemainingTime
        {
            get { return remainingTime; }
            set
            {
                remainingTime = value;
                OnPropertyChanged();
            }
        }

        private string currentTrackTitle = "";
        public string CurrentTrackTitle
        {
            get { return currentTrackTitle; }
            set
            {
                currentTrackTitle = value;
                OnPropertyChanged();
            }
        }

        private string currentTrackArtist = "";
        public string CurrentTrackArtist
        {
            get { return currentTrackArtist; }
            set
            {
                currentTrackArtist = value;
                OnPropertyChanged();
            }
        }

        private long currentTrackPosition = 0;
        public long CurrentTrackPosition
        {
            get { return currentTrackPosition; }
            set
            {
                currentTrackPosition = value;
                OnPropertyChanged();
            }
        }

        private long currentTrackLength = 100;
        public long CurrentTrackLength
        {
            get { return currentTrackLength; }
            set
            {
                currentTrackLength = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Track> tracks = new ObservableCollection<Track>();
        public ObservableCollection<Track> Tracks
        {
            get { return tracks;}
            set
            {
                tracks = value;
                OnPropertyChanged();
            }
        }

        private bool loading = false;
        public bool Loading
        {
            get { return loading; }
            set
            {
                loading = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
