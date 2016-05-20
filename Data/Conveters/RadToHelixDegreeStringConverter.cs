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
    public class RadToHelixDegreeStringConverter : MarkupExtension, IValueConverter
    {
        public static RadToHelixDegreeStringConverter _ratioConverter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int demicalNum = String.IsNullOrEmpty(parameter?.ToString()) ? 16 : System.Convert.ToInt32(parameter);
            string format = "0.";
            for (int i = 0; i < demicalNum; i++)
                format += "#";

            double doubleDegreeValue = System.Convert.ToDouble(value) / Math.PI * 180;
            string stringvalue = "";
            if (doubleDegreeValue > 0)
                stringvalue = doubleDegreeValue.ToString(format) + "°R";
            else if (doubleDegreeValue < 0)
                stringvalue = (-1.0 * doubleDegreeValue).ToString(format) + "°L";
            else
                stringvalue = "0°";
            return stringvalue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value.ToString();

            double degreeValue = 0;
            if (stringValue.ToUpper().Last() == 'R')
                degreeValue = System.Convert.ToDouble(stringValue.ToUpper().TrimEnd('R', '°'));
            else if (stringValue.ToUpper().Last() == 'L')
                degreeValue = -1 * System.Convert.ToDouble(stringValue.ToUpper().TrimEnd('L', '°'));
            else
                degreeValue = System.Convert.ToDouble(stringValue.ToUpper().TrimEnd('L', '°'));

            double radValue = degreeValue / 180 * Math.PI;
            return radValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _ratioConverter ?? (_ratioConverter = new RadToHelixDegreeStringConverter());
        }
    }
}
