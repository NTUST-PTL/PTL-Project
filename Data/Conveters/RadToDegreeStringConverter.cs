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
    public class RadToDegreeStringConverter : MarkupExtension, IValueConverter
    {
        public static RadToDegreeStringConverter _ratioConverter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int demicalNum = String.IsNullOrEmpty(parameter?.ToString())? 16 : System.Convert.ToInt32(parameter);
            string format = "0.";
            for (int i = 0; i < demicalNum; i++)
                format += "#";

            double doubleDegreeValue = System.Convert.ToDouble(value) / Math.PI * 180;
            return doubleDegreeValue.ToString(format) + "°";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value.ToString();
            double degreeValue = System.Convert.ToDouble(stringValue.ToUpper().TrimEnd('°'));
            double radValue = degreeValue / 180 * Math.PI;
            return radValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _ratioConverter ?? (_ratioConverter = new RadToDegreeStringConverter());
        }
    }
}
