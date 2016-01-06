using System;
using Windows.UI.Xaml.Data;

namespace Patronage2016WP.Converters
{
    class NegativeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool newValue = (bool)value;
            return !newValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
