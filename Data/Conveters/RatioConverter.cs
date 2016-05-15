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
    public class RatioConverter : MarkupExtension, IValueConverter
    {
        public static RatioConverter _ratioConverter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = System.Convert.ToDouble(value);
            double ratio = System.Convert.ToDouble(parameter);
            return doubleValue * ratio;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = System.Convert.ToDouble(value);
            double ratio = System.Convert.ToDouble(parameter);
            return doubleValue / ratio;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _ratioConverter ?? (_ratioConverter = new RatioConverter());
        }
    }
}
