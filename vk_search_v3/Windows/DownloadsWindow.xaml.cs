using System.ComponentModel;
using System.Runtime.CompilerServices;
using vk_search_v3.Annotations;
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

        public DownloadWindow(DownloadsWindowViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        public void RemoveCompletedTracksFromQueue()
        {
            _viewModel.RemoveCompletedTracksFromQueue();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
