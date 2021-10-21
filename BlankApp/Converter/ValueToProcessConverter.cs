using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BlankApp.Converter
{
    public class ValueToProcessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
       
            double v = System.Convert.ToDouble(value);
            return string.Format("{0:P2}", v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(System.Convert.ToString(value));
        }
    }
}
