using System;
using System.Globalization;
using System.Windows.Data;

namespace ServiceDebugger.Wpf
{
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumeration)
                return $"({Enum.GetName(enumeration.GetType(), enumeration)})";

            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}