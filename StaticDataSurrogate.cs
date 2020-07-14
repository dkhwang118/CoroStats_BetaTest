///
///     StaticDataSurrogate.cs
///     
///     Static DateTime Data Setup Credit goes to: https://stackoverflow.com/a/2431741
///     Edited By: David K. Hwang
/// 
///     Used to house DateTime and Formatter for DateTime
/// 



using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace CoroStats_BetaTest
{
    public class StaticDataSurrogate
    {
        public DateTime DateToday { get { return DateTime.Today; } }
        public DateTime DateNow { get { return DateTime.Now; } }
    }

    public class FormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                string formatterString = parameter.ToString();

                if (!String.IsNullOrEmpty(formatterString))
                {
                    return String.Format(culture, String.Format("{{0:{0}}}", formatterString), value);
                }
            }

            return (value ?? "").ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
