using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace PTL.Data.Conveters
{
    public class RatioConverter : IValueConverter
    {
        private double _Ratio = 1;
        public double Ratio
        {
            get { return _Ratio; }
            set {
                _Ratio = value;
            }
        }

        public RatioConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = System.Convert.ToDouble(value);
            double ratio = parameter != null? System.Convert.ToDouble(parameter) : Ratio;
            if (targetType == typeof(string))
            {
                if (parameter is string)
                    return (doubleValue * ratio).ToString(parameter as string);
                else
                    return (doubleValue * ratio).ToString();
            }
            return doubleValue * ratio;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = System.Convert.ToDouble(value);
            double ratio = parameter != null ? System.Convert.ToDouble(parameter) : Ratio;
            if (targetType == typeof(string))
            {
                if (parameter is string)
                    return (doubleValue * ratio).ToString(parameter as string);
                else
                    return (doubleValue * ratio).ToString();
            }
            return doubleValue / ratio;
        }
    }
}
