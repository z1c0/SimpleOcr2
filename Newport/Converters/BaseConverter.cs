using System;
using System.Globalization;

namespace Newport.Converters
{
    public abstract class BaseConverter : BindableObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OnConvert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OnConvertBack(value);
        }

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return OnConvert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return OnConvertBack(value);
        }

        protected abstract object OnConvert(object value);

        protected abstract object OnConvertBack(object value);
    }
}

