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
    public partial class LTextBox_double_Arrow : UserControl
    {
        public LTextBox_double_Arrow()
        {
            InitializeComponent();
            this.GotFocus += (sender, e) => this._TextBox.SelectAll();
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

        private void Button_minus_Click(object sender, RoutedEventArgs e)
        {
            this._TextBox.Text = (Convert.ToDouble(this._TextBox.Text) - Gradiation).ToString();
        }

        private void Button_add_Click(object sender, RoutedEventArgs e)
        {
            this._TextBox.Text = (Convert.ToDouble(this._TextBox.Text) + Gradiation).ToString();
        }
    }
}
