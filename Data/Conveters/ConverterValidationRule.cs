using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Controls;

namespace PTL.Data.Conveters
{
    public class ConverterValidationRule : ValidationRule
    {
        public IValueConverter Converter { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                Converter.ConvertBack(value, typeof(int[]), null, null);
                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, "Converter轉換失敗!請檢察輸入");
            }
        }
    }
}
