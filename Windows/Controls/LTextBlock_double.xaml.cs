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
    /// UserControl1.xaml 的互動邏輯
    /// </summary>
    public partial class LTextBlock_double : UserControl
    {
        public LTextBlock_double()
        {
            InitializeComponent();

            this.Value.ValueChanged += LinkededValueChanged;
        }

        public Link<double> Value = new Link<double>();
        public String StringFormat = "N6";

        public event Func<Object, Object, bool> ValueChanged;

        public System.Windows.Media.Brush WarningColor = System.Windows.Media.Brushes.Yellow;
        protected System.Windows.Media.Brush ORGBackGroung;

        private void LinkededValueChanged()
        {
            try
            {
                if (CheckAccess())
                {
                    if (this.StringFormat == null)
                        this.TextBlock.Text = this.Value.V.ToString();
                    else
                        this.TextBlock.Text = this.Value.V.ToString(StringFormat);
                    ValueChanged(this, Value);
                }
                else
                    Dispatcher.Invoke(LinkededValueChanged);
            }
            catch
            {
            }
        }

        public void LinkTo(Source<double> source)
        {
            this.Value.LinkTo(source);
            this.LinkededValueChanged();
        }
    }
}
