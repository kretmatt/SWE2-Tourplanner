using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SWE2_Tourplanner.Converters
{
    public class StringToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dateTimeString = (string)value;
            DateTime dt = new DateTime();
            if (DateTime.TryParse(dateTimeString, out dt))
                return dt;
            return new DateTime();
        }
    }
}
