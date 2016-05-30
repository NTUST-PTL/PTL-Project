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
    public class RadToDegConverter : MarkupExtension, IValueConverter
    {
        public static RadToDegConverter _Converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = System.Convert.ToDouble(value);
            return doubleValue / Math.PI * 180;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = System.Convert.ToDouble(value);
            return doubleValue / 180 * Math.PI;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _Converter ?? (_Converter = new RadToDegConverter());
        }
    }
}
