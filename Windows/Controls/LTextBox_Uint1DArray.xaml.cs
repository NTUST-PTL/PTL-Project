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
    /// LinkableTextBox_Uint1DArray.xaml 的互動邏輯
    /// </summary>
    public partial class LTextBox_UintHashSet : UserControl
    {
        public LTextBox_UintHashSet()
        {
            InitializeComponent();

            this.GotFocus += (object sender, System.Windows.RoutedEventArgs e) => this._TextBox.SelectAll();
            this.Value.ValueChanged += LinkededValueChanged;
            this._TextBox.TextChanged += this.TextChanged;
        }

        Link<HashSet<uint>> Value = new Link<HashSet<uint>>();

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
                    if (this.Value.V != null)
                    {
                        string str = "";
                        foreach (var item in this.Value.V)
                        {
                            if (str == "")
                                str += item.ToString();
                            else
                                str += ", " + item.ToString();
                        }
                        this._TextBox.Text = str;
                    }
                    this._TextBox.TextChanged += this.TextChanged;
                    ValueChanged(this, Value);
                }
                else
                {
                    Dispatcher.Invoke(LinkededValueChanged);
                }
            }
            catch
            {
            }
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string[] items = this._TextBox.Text.Split(',');
                HashSet<uint> sets = new HashSet<uint>();
                foreach (var item in items)
                {
                    sets.Add(Convert.ToUInt32(item));
                }
                this.Value.V = sets;
            }
            catch
            {
            }
        }

        public void LinkTo(Source<HashSet<uint>> source)
        {
            this.Value.LinkTo(source);
            this.LinkededValueChanged();
        }
    }
}
