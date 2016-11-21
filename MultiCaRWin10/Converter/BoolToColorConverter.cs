using System;
using Windows.UI.Xaml.Data;

namespace MultiCaRWin10.Converter
{
    class BoolToColorConverter : IValueConverter
    {
        /// <summary>
        /// Boolean to Color
        /// </summary>
        /// <param name="value">la donnée</param>
        /// <param name="targetType">targettype</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">Culture</param>
        /// <returns>la conversion</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value)  ?"Goldenrod": "Tomato";
        }

        /// <summary>
        /// Color To boolean
        /// </summary>
        /// <param name="value">la donnée</param>
        /// <param name="targetType">targettype</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">Culture</param>
        /// <returns>la conversion</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((string)value).Equals("Goldenrod");
        }
    }
}
