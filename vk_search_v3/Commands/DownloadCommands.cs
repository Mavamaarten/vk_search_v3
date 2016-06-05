using System.Windows.Input;

namespace vk_search_v3.Commands
{
    public static class DownloadCommands
    {
        public static readonly RoutedUICommand Cancel = new RoutedUICommand(
            "Cancel",
            "Cancel",
            typeof(PlayerCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Delete, ModifierKeys.None)
            }
        );
    }
}