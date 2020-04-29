using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ServiceDebugger.Views;

namespace ServiceDebugger.Wpf
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ServiceStatus status))
                return Binding.DoNothing;

            switch (status)
            {
                case ServiceStatus.Stopped:
                    return FromHex("#80FFDE00");
                case ServiceStatus.Running:
                    return FromHex("#8054BB57");
                case ServiceStatus.Paused:
                    return FromHex("#80FDFF00");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static SolidColorBrush FromHex(string hex) => (SolidColorBrush)new BrushConverter().ConvertFrom(hex);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}