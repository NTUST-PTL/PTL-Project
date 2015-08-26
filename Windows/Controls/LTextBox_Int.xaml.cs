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
    public partial class LTextBox_Int : UserControl
    {
        public LTextBox_Int()
        {
            InitializeComponent();

            this.GotFocus += (object sender, System.Windows.RoutedEventArgs e) => this._TextBox.SelectAll();
            this.Value.ValueChanged += LinkededValueChanged;
            this._TextBox.TextChanged += this.TextChanged;
        }

        Link<int> Value = new Link<int>();
        public String StringFormat;

        public event Func<Object, Object, bool> ValueChanged;

        public System.Windows.Media.Brush WarningColor = System.Windows.Media.Brushes.Yellow;
        protected System.Windows.Media.Brush ORGBackGroung;

        private void LinkededValueChanged()
        {
            try
            {
                if (CheckAccess())
                {
                    this._TextBox.TextChanged -= this.TextChanged;
                    if (this.StringFormat == null)
                        this._TextBox.Text = this.Value.V.ToString();
                    else
                        this._TextBox.Text = this.Value.V.ToString(StringFormat);
                    this._TextBox.TextChanged += this.TextChanged;
                    ValueChanged(this, Value);
                }
                else
                    Dispatcher.Invoke(LinkededValueChanged);
            }
            catch
            {
            }
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.Value.V = Convert.ToInt32(this._TextBox.Text);
            }
            catch
            {
            }
        }

        public void LinkTo(Source<int> source)
        {
            this.Value.LinkTo(source);
            this.LinkededValueChanged();
        }
    }
    
}
