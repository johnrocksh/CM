namespace CastManager.UI.Converters
{ 
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uri = value as Uri;

            if (uri != null)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.CacheOption = BitmapCacheOption.None;
                image.UriSource = uri;
                image.EndInit();
                return image;
            }
            var url = value as string;
            if (!string.IsNullOrEmpty(url))
            {
                var image = new BitmapImage();
                image.BeginInit();
                if (parameter != null)
                {
                    image.CreateOptions = (BitmapCreateOptions)parameter;
                }
                image.UriSource = new Uri(url);
                image.EndInit();
                return image;
            }

            return value as ImageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}