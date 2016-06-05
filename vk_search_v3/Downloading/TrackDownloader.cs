using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using vk_search_v3.Model;

namespace vk_search_v3.Downloading
{
    class TrackDownloader
    {
        private ObservableCollection<Track> _tracks;
        public ObservableCollection<Track> Tracks
        {
            get { return _tracks; }
            set
            {
                _tracks = value;
                _tracks.CollectionChanged += TracksOnCollectionChanged;
            }
        }

        public TrackDownloader(ObservableCollection<Track> tracks)
        {
            Tracks = tracks;
            DownloadAllTracks();
        }

        private void TracksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            foreach (Track track in notifyCollectionChangedEventArgs.NewItems)
            {
                new Thread(() => DownloadTrack(track)).Start();
            }
        }

        private void DownloadAllTracks()
        {
            foreach (var track in Tracks)
            {
                new Thread(() => DownloadTrack(track)).Start();
            }
        }

        public void DownloadTrack(Track track)
        {
            var client = new WebClient();
            client.DownloadProgressChanged += (sender, args) =>
            {
                track.DownloadProgress = args.ProgressPercentage;
            };
            client.DownloadFileAsync(new Uri(track.url), @"C:\Users\Maarten\Desktop\" + track.artist + " - " + track.title + ".mp3");
        }
    }
}
