using System.Windows.Input;

namespace vk_search_v3.Commands
{
    public static class PlayerCommands
    {
        public static readonly RoutedUICommand PlayNext = new RoutedUICommand(
            "Play Next",
            "PlayNext",
            typeof (PlayerCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N, ModifierKeys.Alt)
            }
        );

        public static readonly RoutedUICommand AddToQueue = new RoutedUICommand(
            "Add to queue",
            "AddToQueue",
            typeof (PlayerCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Q, ModifierKeys.Alt)
            }
        );

        public static readonly RoutedUICommand AddToPlaylist = new RoutedUICommand(
            "Add to playlist",
            "AddToPlaylist",
            typeof(PlayerCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.P, ModifierKeys.Alt)
            }
        );

        public static readonly RoutedUICommand Download = new RoutedUICommand(
            "Download",
            "Download",
            typeof(PlayerCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.J, ModifierKeys.Control)
            }
        );
    }
}