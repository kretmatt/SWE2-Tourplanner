using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SWE2_Tourplanner.Converters
{
    /// <summary>
    /// RouteInfoImageConverter is a used for string - BitmapImage conversion. 
    /// </summary>
    public class RouteInfoImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a path (string) to a BitmapImage
        /// </summary>
        /// <param name="value">The value that needs to be converted</param>
        /// <param name="targetType">TargetType for the conversion</param>
        /// <param name="parameter">Additional parameter for the conversion</param>
        /// <param name="culture">Culture settings for the conversion</param>
        /// <returns>Already loaded BitmapImage for Image control. If the path doesn't exist, return empty BitmapImage</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mapImage = new BitmapImage();

            if (File.Exists(Path.GetFullPath((string)value)))
            {
                using (FileStream fs = File.OpenRead(Path.GetFullPath((string)value)))
                {
                    mapImage.BeginInit();
                    mapImage.CacheOption = BitmapCacheOption.OnLoad;
                    mapImage.StreamSource = fs;
                    mapImage.EndInit();
                }
            }  
            return mapImage;
        }
        /// <summary>
        /// Converts BitmapImage back to Path (string)
        /// </summary>
        /// <param name="value">The value that needs to be converted</param>
        /// <param name="targetType">TargetType for the conversion</param>
        /// <param name="parameter">Additional parameter for the conversion</param>
        /// <param name="culture">Culture settings for the conversion</param>
        /// <returns>Converted object. In this case Binding.DoNothing is returned because a image can't really be converted back to a path.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
