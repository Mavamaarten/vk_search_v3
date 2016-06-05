﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using vk_search_v3.Annotations;
using vk_search_v3.Base;
using vk_search_v3.Model;

namespace vk_search_v3.ViewModel
{
    public class DownloadsWindowViewModel : PropertyChangedNotifying
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

        private ObservableCollection<DownloadTrackThread> _downloadThreads;
        public ObservableCollection<DownloadTrackThread> DownloadThreads
        {
            get { return _downloadThreads; }
            set
            {
                if (Equals(value, _downloadThreads)) return;
                _downloadThreads = value;
                OnPropertyChanged();
            }
        }

        public DownloadsWindowViewModel()
        {
            Tracks = new ObservableCollection<Track>();
            DownloadThreads = new ObservableCollection<DownloadTrackThread>();
            DownloadAllTracks();
        }

        private void TracksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            foreach (Track track in notifyCollectionChangedEventArgs.NewItems)
            {
                var downloadThread = new DownloadTrackThread(track);
                DownloadThreads.Add(downloadThread);
                downloadThread.Start();
            }
        }

        private void DownloadAllTracks()
        {
            foreach (var track in Tracks)
            {
                var downloadThread = new DownloadTrackThread(track);
                DownloadThreads.Add(downloadThread);
                downloadThread.Start();
            }
        }

        public class DownloadTrackThread : INotifyPropertyChanged
        {
            private readonly Thread thread;
            public Track Track { get; }

            private bool _downloading;
            public bool Downloading
            {
                get { return _downloading; }
                set
                {
                    if (value == _downloading) return;
                    _downloading = value;
                    OnPropertyChanged();
                }
            }

            private string _statusText;
            public string StatusText
            {
                get { return _statusText; }
                set
                {
                    if (value == _statusText) return;
                    _statusText = value;
                    OnPropertyChanged();
                }
            }

            private int _downloadProgress;
            public int DownloadProgress
            {
                get { return _downloadProgress; }
                set
                {
                    if (value == _downloadProgress) return;
                    _downloadProgress = value;
                    OnPropertyChanged();
                }
            }

            private bool _completed;
            public bool Completed
            {
                get { return _completed; }
                set
                {
                    if (value == _completed) return;
                    _completed = value;
                    OnPropertyChanged();
                }
            }

            public DownloadTrackThread(Track track)
            {
                Track = track;
                thread = new Thread(() => Run(Track));
                OnPropertyChanged(nameof(Track));
            }

            public void Start()
            {
                thread.Start();
            }

            public void Run(Track track)
            {
                var client = new WebClient();
                client.DownloadProgressChanged += (sender, args) =>
                {
                    DownloadProgress = args.ProgressPercentage;
                    StatusText = DownloadProgress + "%";
                };
                client.DownloadFileCompleted += (sender, args) =>
                {
                    Downloading = false;
                    Completed = true;
                    StatusText = "Completed";
                };
                client.DownloadFileAsync(new Uri(track.url), @"C:\Users\Maarten\Desktop\" + track.artist + " - " + track.title + ".mp3");
                Downloading = true;
                StatusText = "Starting...";
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
