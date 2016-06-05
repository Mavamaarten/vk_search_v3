using System;
using System.Globalization;
using System.Windows.Data;
using vk_search_v3.Util;

namespace vk_search_v3.ValueConverter
{
    class DurationValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is long)) return string.Empty;
            return FormatUtil.secondsToShortTimespan((int) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
