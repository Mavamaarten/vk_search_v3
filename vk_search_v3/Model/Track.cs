using vk_search_v3.Base;
using vk_search_v3.Util;

namespace vk_search_v3.Model
{
    public class Track : PropertyChangedNotifying
    {
        private long _id;
        private long _aid;
        private long _ownerId;
        private string _artist;
        private string _title;
        private string _url;
        private bool _playing;
        private int _bitrate;
        private int _duration;
        private long _albumId;
        private long _lyricsId;
        private long _genreId;

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

        public long aid
        {
            get { return _aid; }
            set
            {
                if (value == _aid) return;
                _aid = value;
                OnPropertyChanged();
            }
        }

        public long owner_id
        {
            get { return _ownerId; }
            set
            {
                if (value == _ownerId) return;
                _ownerId = value;
                OnPropertyChanged();
            }
        }

        public string artist
        {
            get { return _artist; }
            set
            {
                if (value == _artist) return;
                _artist = value;
                OnPropertyChanged();
            }
        }

        public string title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string url
        {
            get { return _url; }
            set
            {
                if (value == _url) return;
                _url = value;
                OnPropertyChanged();
            }
        }

        public bool playing
        {
            get { return _playing; }
            set
            {
                if (value == _playing) return;
                _playing = value;
                OnPropertyChanged();
            }
        }

        public int bitrate
        {
            get { return _bitrate; }
            set
            {
                if (value == _bitrate) return;
                _bitrate = value;
                OnPropertyChanged();
            }
        }

        public int duration
        {
            get { return _duration; }
            set
            {
                if (value == _duration) return;
                _duration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationString));
            }
        }

        public long album_id
        {
            get { return _albumId; }
            set
            {
                if (value == _albumId) return;
                _albumId = value;
                OnPropertyChanged();
            }
        }

        public long lyrics_id
        {
            get { return _lyricsId; }
            set
            {
                if (value == _lyricsId) return;
                _lyricsId = value;
                OnPropertyChanged();
            }
        }

        public long genre_id
        {
            get { return _genreId; }
            set
            {
                if (value == _genreId) return;
                _genreId = value;
                OnPropertyChanged();
            }
        }

        public string DurationString => FormatUtil.secondsToShortTimespan(duration);
    }
}