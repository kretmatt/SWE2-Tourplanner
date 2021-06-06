using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SWE2_Tourplanner.Converters
{
    /// <summary>
    /// StringToDateTimeConverter is used for string - DateTime conversion.
    /// </summary>
    public class StringToDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime to a string
        /// </summary>
        /// <param name="value">The value that needs to be converted</param>
        /// <param name="targetType">TargetType for the conversion</param>
        /// <param name="parameter">Additional parameter for the conversion</param>
        /// <param name="culture">Culture settings for the conversion</param>
        /// <returns>Converted DateTime string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString();
        }
        /// <summary>
        /// Converts a string to a DateTime instance
        /// </summary>
        /// <param name="value">The value that needs to be converted</param>
        /// <param name="targetType">TargetType for the conversion</param>
        /// <param name="parameter">Additional parameter for the conversion</param>
        /// <param name="culture">Culture settings for the conversion</param>
        /// <returns>Converted object. If string can't be parsed, return new DateTime instance</returns>
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
