using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;

namespace PTL.Data.Conveters
{
    /// <summary>
    /// 支援一對多綁定。
    /// 
    /// 例如一個Text屬性綁定至多個物件屬性，以Converter的MasterElementIndex指定被綁定的物件屬性的主要Source的索引。
    /// 若變更Text屬性(TwoWay或OneWayToSource的情況下)，則所有被綁定的Source將被更新；
    /// 若變更主要Source的值(需呼叫PropertyChanged委派)，則Text屬性及其餘被綁定的Source將被更新；
    /// 但變更非主要Source的值(不論呼叫PropertyChanged委派與否)，Text屬性及其餘被綁定的Source皆不會更新。
    /// </summary>
    public class MultiSyncNumberConverter : IMultiValueConverter
    {
        public int MasterElementIndex { get; set; } = 0;
        public IValueConverter[] Converters { get; set; }

        public MultiSyncNumberConverter()
        {

        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                object mainValue = values[MasterElementIndex];
                object returnValue = mainValue;
                if (Converters != null && Converters.Length > MasterElementIndex && Converters[MasterElementIndex] != null)
                {
                    returnValue = Converters[MasterElementIndex].Convert(mainValue, targetType, parameter, culture);
                }
                return returnValue;
            }
            catch
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
                object[] values = new object[targetTypes.Length];
                for (int i = 0; i < targetTypes.Length; i++)
                {
                    values[i] = value;
                    if (Converters != null
                        && Converters.Length > i
                        && Converters[i] != null)
                    {
                        values[i] = Converters[i].ConvertBack(value, targetTypes[i], parameter, culture);
                    }
                }
                return values;
            }
            catch
            {
                return null;
            }
        }
    }
}
