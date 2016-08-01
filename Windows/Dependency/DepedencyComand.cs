using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
namespace PTL.Windows.Dependency
{
    public class DepedencyComand : FrameworkElement
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command"
                , typeof(ICommand)
                , typeof(DepedencyComand)
                );
        
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return obj.GetValue(CommandProperty) as ICommand;
        }


        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter"
                , typeof(object)
                , typeof(DepedencyComand)
                );

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }
    }
}
