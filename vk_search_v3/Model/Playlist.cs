using System;
using System.Collections.ObjectModel;
using vk_search_v3.Base;

namespace vk_search_v3.Model
{
    [Serializable]
    public class Playlist : PropertyChangedNotifying
    {
        private long _id;
        private ObservableCollection<Track> _tracks;
        private string _name;

        public long Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Track> Tracks
        {
            get { return _tracks; }
            set
            {
                if (Equals(value, _tracks)) return;
                _tracks = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public Playlist(string name)
        {
            Name = name;
            Tracks = new ObservableCollection<Track>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
