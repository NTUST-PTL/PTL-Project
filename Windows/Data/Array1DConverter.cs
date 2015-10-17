using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Windows.Data
{
    public class Array1DConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(String))
            {
                StringBuilder strBuilder = new StringBuilder();
                if (value?.GetType().GetElementType() == typeof(double))
                {
                    double[] arr = (double[])value;
                    strBuilder.Append("{ ");
                    for (int i = 0; i < arr.Length - 1; i++)
                        strBuilder.Append(arr[i] + ", ");
                    strBuilder.Append(arr.Last());
                    strBuilder.Append(" }");
                }
                if (value?.GetType().GetElementType() == typeof(int))
                {
                    int[] arr = (int[])value;
                    strBuilder.Append("{ ");
                    for (int i = 0; i < arr.Length - 1; i++)
                        strBuilder.Append(arr[i] + ", ");
                    strBuilder.Append(arr.Last());
                    strBuilder.Append(" }");
                }
                return strBuilder.ToString();
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType() == typeof(String))
            {
                string str = (string)value;
                str = str.Trim(new char[] { '{', '}' });
                string[] elements = str.Split(',');

                if (targetType.GetElementType() == typeof(double))
                {
                    double[] arr = new double[elements.Length];
                    for (int i = 0; i < elements.Length; i++)
                        arr[i] = System.Convert.ToDouble(elements[i]);
                    return arr;
                }
                else if (targetType.GetElementType() == typeof(int))
                {
                    int[] arr = new int[elements.Length];
                    for (int i = 0; i < elements.Length; i++)
                        arr[i] = System.Convert.ToInt32(elements[i]);
                    return arr;
                }
                else
                    return null;
            }
            else
                return null;
        }
    }
}
