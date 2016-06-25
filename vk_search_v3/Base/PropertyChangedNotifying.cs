using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using vk_search_v3.Annotations;

namespace vk_search_v3.Base
{
    [Serializable]
    public class PropertyChangedNotifying : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
