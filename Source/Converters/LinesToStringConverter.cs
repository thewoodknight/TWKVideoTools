using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TWKVideoTools.Converters
{
    public class LinesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lines = (List<string>)value;
            return string.Join(" ", lines);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
