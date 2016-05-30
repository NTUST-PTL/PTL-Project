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
    public class IntArray1DToStringConverter : MarkupExtension, IValueConverter
    {
        public static IntArray1DToStringConverter _Converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            int[] arrayValue = (int[])value;
            string stringValue = "";
            for (int i = 0; i < arrayValue.Length; i++)
            {
                stringValue += arrayValue[i].ToString();
                if (i != arrayValue.Length - 1)
                    stringValue += ", ";
            }
            return stringValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (String.IsNullOrEmpty(value as string))
            return null;

            string stringValue = (string)value;
            string[] stringArray = stringValue.Trim(' ').Split(',');
            int[] intArray = new int[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
            {
                intArray[i] = System.Convert.ToInt32(stringArray[i]);
            }
            return intArray;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _Converter ?? (_Converter = new IntArray1DToStringConverter());
        }
    }
}
