using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using vk_search_v3.Annotations;
using vk_search_v3.Downloading;
using vk_search_v3.ViewModel;

namespace vk_search_v3.Windows
{
    public partial class DownloadWindow : INotifyPropertyChanged
    {
        private DownloadsWindowViewModel _viewModel;
        public DownloadsWindowViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged();
            }
        }

        private TrackDownloader trackDownloader;

        public DownloadWindow(DownloadsWindowViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
            trackDownloader = new TrackDownloader(viewModel.Tracks);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
