using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CopyRightPDF.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "New":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60"));
                case "Opened":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2980b9"));
                case "Expired":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7f8c8d"));
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
