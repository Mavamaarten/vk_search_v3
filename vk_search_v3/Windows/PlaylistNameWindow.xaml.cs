using System.Windows;

namespace vk_search_v3.Windows
{
    public partial class PlaylistNameWindow
    {
        public string PlaylistName => txtPlaylistName.Text;

        public PlaylistNameWindow()
        {
            InitializeComponent();
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
