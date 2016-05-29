using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PTL.Windows.Dependency
{
    public class DepedentComand : DependencyObject
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command"
                , typeof(ICommand)
                , typeof(DepedentComand)
                );
        
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return obj.GetValue(CommandProperty) as ICommand;
        }
    }
}
