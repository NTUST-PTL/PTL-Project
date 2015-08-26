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
    /// BindableTextBox.xaml 的互動邏輯
    /// </summary>
    public partial class BTextBox : UserControl, IBindable
    {
        public BTextBox()
        {
            InitializeComponent();
            InitializeTextBoxEvent();
            this.GotFocus += (object sender, System.Windows.RoutedEventArgs e) => this._TextBox.SelectAll();
            this.BindedValueChanged = _BindedValueChanged;
        }

        private void InitializeTextBoxEvent()
        {
            this._TextBox.TextChanged += (object sender, TextChangedEventArgs e) => this.V = this._TextBox.Text;
            //if (this.TextChanged != null)
            //    this._TextBox.TextChanged += this.TextChanged;
        }
        public string value = "";
        public object V
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value != value.ToString())
                {
                    this.value = value.ToString();
                    this._TextBox.Text = this.value;
                    if (ValueChanged != null)
                    {
                        bool somthingWrong = false;
                        foreach (Func<Object, Object, bool> item in ValueChanged.GetInvocationList())
                            if (!item(this, this.value))
                                somthingWrong = true;
                        if (somthingWrong)
                        {
                            if (this._TextBox.Background != this.WarningColor)
                            {
                                this.ORGBackGroung = this._TextBox.Background;
                                this._TextBox.Background = this.WarningColor;
                            }
                        }
                        else
                        {
                            if (this._TextBox.Background == this.WarningColor)
                                this._TextBox.Background = this.ORGBackGroung;
                        }
                    }
                }
            }
        }

        public event Func<Object, Object, bool> ValueChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }
        //public event TextChangedEventHandler TextChanged;

        public System.Windows.Media.Brush WarningColor = System.Windows.Media.Brushes.Yellow;
        protected System.Windows.Media.Brush ORGBackGroung;

        private bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                this.V = value.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
