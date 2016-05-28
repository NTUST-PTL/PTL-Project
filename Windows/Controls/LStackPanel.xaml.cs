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
using PTL.Geometry;

namespace PTL.Windows.Controls
{
    /// <summary>
    /// LinkableCombox.xaml 的互動邏輯
    /// </summary>
    public partial class LStackPanel : UserControl
    {
        public Link<HashSet<ICanPlotInOpenGL>> Value = new Link<HashSet<ICanPlotInOpenGL>>();

        public Action ItemStateChanged;

        public LStackPanel()
        {
            InitializeComponent();

            Value.PropertyChanged += (o, e) => SynchronizeItem();
        }

        public void SynchronizeItem()
        {
            if (CheckAccess())
            {
                this._StackPanel.Children.Clear();
                List<ICanPlotInOpenGL> ValueList = Value.Value.ToList();
                for (int i = 0; i < Value.Value.Count; i++)
                {
                    if (ValueList[i] is IHaveVisibility)
                    {
                        IHaveVisibility item = ValueList[i] as IHaveVisibility;
                        CheckBox cb = new CheckBox() { IsChecked = (item.Visible == true) };
                        cb.Checked += CheckBoc_IsCheckedChanged;
                        cb.Unchecked += CheckBoc_IsCheckedChanged;
                        this._StackPanel.Children.Add(cb);
                    }
                }
            }
            else
                Dispatcher.Invoke(SynchronizeItem);
        }

        public void SynchronizeValueToTarget()
        {
            if (CheckAccess())
            {
                int i = 0;
                List<IHaveVisibility> ValueList =
                    (from item in Value.Value.ToList()
                     where item is IHaveVisibility
                     select item as IHaveVisibility).ToList();
                foreach (CheckBox item in this._StackPanel.Children)
                {
                    if (item != null)
                    {
                        ValueList[i].Visible = item.IsChecked;
                        i++;
                    }
                }
            }
            else
            {
                Dispatcher.Invoke(SynchronizeValueToTarget);
            }
        }

        public void CheckBoc_IsCheckedChanged(object o, RoutedEventArgs e)
        {
            SynchronizeValueToTarget(); ItemStateChanged();
        }

        private void BT_Inverse_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAccess())
            {
                foreach (CheckBox item in this._StackPanel.Children)
                {
                    if (item != null)
                    {
                        item.Checked -= CheckBoc_IsCheckedChanged;
                        item.Unchecked -= CheckBoc_IsCheckedChanged;
                        if (item.IsChecked == true)
                            item.IsChecked = false;
                        else if (item.IsChecked == false)
                            item.IsChecked = true;
                        item.Checked += CheckBoc_IsCheckedChanged;
                        item.Unchecked += CheckBoc_IsCheckedChanged;
                    }
                }
                CheckBoc_IsCheckedChanged(null, null);
            }
            else
                Dispatcher.Invoke(SynchronizeItem);
        }
    }
}
