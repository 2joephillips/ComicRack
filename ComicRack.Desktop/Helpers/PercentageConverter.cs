using System.Globalization;
using System.Windows.Data;

namespace ComicRack.Desktop.Helpers
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double originalValue && double.TryParse(parameter?.ToString(), out double percentage))
            {
                return originalValue * percentage;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
