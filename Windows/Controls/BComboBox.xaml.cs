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
    /// BComboBox.xaml 的互動邏輯
    /// </summary>
    public partial class BComboBox : UserControl, IBindable
    {
        public BComboBox()
        {
            InitializeComponent();
            this._ComboBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                if (this.ValueChanged != null)
                    this.ValueChanged(this, (this._ComboBox.SelectedItem as ComboBoxItem).Content.ToString());
            };
            this.BindedValueChanged = _BindedValueChanged;
        }

        public void SetItems(IEnumerable<String> itemsString)
        {
            foreach (var item in itemsString)
            {
                ComboBoxItem aComboBoxItem = new ComboBoxItem();
                aComboBoxItem.Content = item;
                this._ComboBox.Items.Add(aComboBoxItem);
            }
        }

        #region IBindable 成員

        public event Func<Object, Object, bool> ValueChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                bool selectSuccesful = false;
                foreach (var item in this._ComboBox.Items)
                {
                    ComboBoxItem aComboBoxItem = item as ComboBoxItem;
                    if (aComboBoxItem.Content.ToString().ToLower() == value.ToString().ToLower())
                    {
                        this._ComboBox.SelectedItem = aComboBoxItem;
                        selectSuccesful = true;
                        if (this.ValueChanged != null) { this.ValueChanged(this, aComboBoxItem.Content.ToString()); }
                    }
                }
                return selectSuccesful;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
