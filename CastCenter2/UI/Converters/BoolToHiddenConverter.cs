namespace CastManager.UI.Converters
{
    using System.Windows;
    using System.Windows.Data;

    public class BoolToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = (bool) value;
            return b ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = (Visibility)value;
            return v == Visibility.Visible;
        }
    }
}
