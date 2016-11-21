using System;
using Windows.UI.Xaml.Data;
using MultiCaRWin10.Utils;

namespace MultiCaRWin10.Converter
{
    class MilisecToStringConverter : IValueConverter
    {
        /// <summary>
        /// Milisecondes en string
        /// </summary>
        /// <param name="value">la donnée</param>
        /// <param name="targetType">targettype</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">Culture</param>
        /// <returns>la conversion</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return DateUtils.ConvertiNbSecondesEnStringAvecMs((long)value);
        }

        /// <summary>
        /// string en milisecondes
        /// </summary>
        /// <param name="value">la donnée</param>
        /// <param name="targetType">targettype</param>
        /// <param name="parameter">parameter</param>
        /// <param name="language">Culture</param>
        /// <returns>la conversion</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DateUtils.ConvertirStringEnNbMilisec(value as string);
        }
    }
}
