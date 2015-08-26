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
    public partial class BTextBox_Arrow : UserControl, IBindable
    {
        public BTextBox_Arrow()
        {
            InitializeComponent();
            this.TextBox.TextChanged += (object sender, TextChangedEventArgs e) => this.V = this.TextBox.Text;
            this.GotFocus += (object sender, System.Windows.RoutedEventArgs e) => this.TextBox.SelectAll();
            this.BindedValueChanged = _BindedValueChanged;
            this.V = 0.1; this.V = 0.0;
        }

        public double value;
        public object V
        {
            get
            {
                return this.value;
            }
            set
            {
                try
                {
                    if (this.value != Convert.ToDouble(value))
                    {
                        this.value = Convert.ToDouble(value);
                        this.TextBox.Text = this.value.ToString();
                        if (ValueChanged != null)
                        {
                            bool somthingWrong = false;
                            foreach (Func<Object, Object, bool> item in ValueChanged.GetInvocationList())
                                if (!item(this, this.value))
                                    somthingWrong = true;
                            if (somthingWrong)
                            {
                                if (this.Background != this.WarningColor)
                                    this.ORGBackGroung = this.Background;
                                this.Background = this.WarningColor;
                            }
                            else
                            {
                                if (this.Background == this.WarningColor)
                                    this.Background = this.ORGBackGroung;
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }
        public double _Gradiation = 0.1;
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
        public TextBox TextBox
        {
            get { return this._TextBox; }
            set {
                if (this._TextBox != value)
                {
                    this._TextBox = value;
                    this._TextBox.TextChanged += (object sender, TextChangedEventArgs e) => this.V = this.TextBox.Text;
                }
            }
        }

        public event Func<Object, Object, bool> ValueChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public System.Windows.Media.Brush WarningColor = System.Windows.Media.Brushes.Yellow;
        protected System.Windows.Media.Brush ORGBackGroung;

        private bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                if (CheckAccess())
                    this.V = value.ToString();
                else
                    Dispatcher.Invoke(() => { _BindedValueChanged(sender, value); });
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Button_minus_Click(object sender, RoutedEventArgs e)
        {
            this.V = this.value - Gradiation;
        }

        private void Button_add_Click(object sender, RoutedEventArgs e)
        {
            this.V = this.value + Gradiation;
        }
    }
}
