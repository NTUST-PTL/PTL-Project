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
    public partial class LTextBox : UserControl
    {
        public LTextBox()
        {
            InitializeComponent();

            this.GotFocus += (object sender, System.Windows.RoutedEventArgs e) => this._TextBox.SelectAll();
            this.Value.ValueChanged += LinkededValueChanged;
            this._TextBox.TextChanged += this.TextChanged;
        }

        public Link<String> Value = new Link<String>();

        public event Func<Object, Object, bool> ValueChanged;

        public System.Windows.Media.Brush WarningColor = System.Windows.Media.Brushes.Yellow;
        protected System.Windows.Media.Brush ORGBackGroung;

        private void LinkededValueChanged()
        {
            if (CheckAccess())
            {
                this._TextBox.TextChanged -= this.TextChanged;
                this._TextBox.Text = this.Value.V;
                this._TextBox.TextChanged += this.TextChanged;
                if (ValueChanged != null)
                    ValueChanged(this, Value);
            }
            else
            {
                Dispatcher.Invoke(LinkededValueChanged);
            }
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Value.V =this._TextBox.Text;
        }

        public void LinkTo(Source<String> source)
        {
            this.Value.LinkTo(source);
            this.LinkededValueChanged();
        }
    }
    
}
