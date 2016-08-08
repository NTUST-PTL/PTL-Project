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
    public class ColorConverter : IValueConverter
    {
        public ColorConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.Color)
            {
                System.Windows.Media.Color wmColor = (System.Windows.Media.Color)value;
                System.Drawing.Color sdColor = System.Drawing.Color.FromArgb(wmColor.A, wmColor.R, wmColor.G, wmColor.B);
                if (targetType == typeof(System.Drawing.Color))
                {
                    return sdColor;
                }
                else
                {
                    return null;
                }
            }
            else if (value is System.Windows.Media.SolidColorBrush)
            {
                System.Windows.Media.Color wmColor = ((System.Windows.Media.SolidColorBrush)value).Color;
                System.Drawing.Color sdColor = System.Drawing.Color.FromArgb(wmColor.A, wmColor.R, wmColor.G, wmColor.B);
                if (targetType == typeof(System.Drawing.Color))
                {
                    return sdColor;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color)
            {
                System.Drawing.Color sdColor = (System.Drawing.Color)value;
                System.Windows.Media.Color wmColor = System.Windows.Media.Color.FromArgb(sdColor.A, sdColor.R, sdColor.G, sdColor.B);
                if (targetType == typeof(System.Windows.Media.Color))
                {
                    return wmColor;
                }
                else if (targetType == typeof(System.Windows.Media.SolidColorBrush))
                {
                    return new System.Windows.Media.SolidColorBrush() { Color = wmColor };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
