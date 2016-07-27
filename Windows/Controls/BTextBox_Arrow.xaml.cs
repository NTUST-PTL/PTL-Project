using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PTL.Base;

namespace PTL.Windows.Controls
{
    /// <summary>
    /// BindableTextBox_Arrow.xaml 的互動邏輯
    /// </summary>
    public partial class BTextBox_Arrow : UserControl
    {
        public BTextBox_Arrow()
        {
            InitializeComponent();
        }

        private double _Gradiation = 0.1;
        public double Gradiation
        {
            get
            {
                return _Gradiation;
            }
            set
            {
                if (value > 0 && _Gradiation != value)
                    this._Gradiation = value;
            }
        }

        public static FrameworkPropertyMetadata TextPropertymetadata =
            new FrameworkPropertyMetadata(""
                , FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                | FrameworkPropertyMetadataOptions.Journal
                , new PropertyChangedCallback(Text_PropertyChanged)
                , new CoerceValueCallback(Text_CoerceValue)
                , false
                , UpdateSourceTrigger.PropertyChanged);
        public static FrameworkPropertyMetadata CommandPropertymetadata =
            new FrameworkPropertyMetadata(null
                , FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                | FrameworkPropertyMetadataOptions.Journal
                , new PropertyChangedCallback(Command_PropertyChanged)
                , new CoerceValueCallback(Command_CoerceValue)
                , false
                , UpdateSourceTrigger.PropertyChanged);
        public static DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text)
            , typeof(string)
            , typeof(BTextBox_Arrow)
            , TextPropertymetadata
            , new ValidateValueCallback(Text_Validate));
        public static DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command)
            , typeof(ICommand)
            , typeof(BTextBox_Arrow)
            , CommandPropertymetadata
            , new ValidateValueCallback(Command_Validate));

        private static void Text_PropertyChanged(
            DependencyObject dobj,
            DependencyPropertyChangedEventArgs e)
        {
        }

        private static object Text_CoerceValue(DependencyObject dobj, object Value)
        {
            return Value;
        }

        private static bool Text_Validate(object Value)
        {
            return true;
        }

        private static void Command_PropertyChanged(
            DependencyObject dobj,
            DependencyPropertyChangedEventArgs e)
        {
        }

        private static object Command_CoerceValue(DependencyObject dobj, object Value)
        {
            return Value;
        }

        private static bool Command_Validate(object Value)
        {
            return true;
        }

        public string Text
        {
            get { return this.GetValue(TextProperty) as string; }
            set { this.SetValue(TextProperty, value); } 
        }
        public ICommand Command
        {
            get { return this.GetValue(CommandProperty) as ICommand; }
            set { this.SetValue(CommandProperty, value); }
        }

        private void Button_minus_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(System.Text.RegularExpressions.Regex.Replace(this.Text, "[^0-9.]", "")))
                return;

            this.Text = (
                Convert.ToDouble(System.Text.RegularExpressions.Regex.Replace(this.Text, "[^-0-9.]", ""))
                - Gradiation).ToString();
            this.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            Command?.Execute(null);
        }

        private void Button_add_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(System.Text.RegularExpressions.Regex.Replace(this.Text, "[^0-9.]", "")))
                return;

            this.Text = (
                Convert.ToDouble(System.Text.RegularExpressions.Regex.Replace(this.Text, "[^-0-9.]", ""))
                + Gradiation).ToString();
            this.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            Command?.Execute(null);
        }

        private void _TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((TextBox)sender).GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                this.GetBindingExpression(CommandProperty)?.UpdateSource();
                this.Command?.Execute(null);
            }
        }
    }
}
