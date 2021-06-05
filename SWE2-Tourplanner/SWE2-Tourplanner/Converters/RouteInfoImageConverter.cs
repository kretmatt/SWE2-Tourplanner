﻿using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SWE2_Tourplanner.Converters
{
    public class RouteInfoImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mapImage = new BitmapImage();
            if (value==null||string.IsNullOrWhiteSpace((string)value))
                return Binding.DoNothing;
            
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
