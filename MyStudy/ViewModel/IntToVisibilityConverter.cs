using System;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace MyStudy.View
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If TotalEntries is 0, show the message (Visible)
            if (value is int totalEntries && totalEntries == 0)
            {
                return true;  // True means visible
            }

            return false;  // False means hidden
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
