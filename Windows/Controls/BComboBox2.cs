using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PTL.Base;

namespace PTL.Windows.Controls
{
    public class BComboBox2 : ComboBox, IBindable
    {

        public BComboBox2()
            : base()
        {
            this.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                if (this.ValueChanged != null)
                    this.ValueChanged(this, (this.SelectedItem as ComboBoxItem).Content.ToString());
            };
            this.BindedValueChanged = _BindedValueChanged;
        }

        public void SetItems(IEnumerable<String> itemsString)
        {
            foreach (var item in itemsString)
            {
                ComboBoxItem aComboBoxItem = new ComboBoxItem();
                aComboBoxItem.Content = item;
                this.Items.Add(aComboBoxItem);
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
                foreach (var item in Items)
                {
                    ComboBoxItem aComboBoxItem = item as ComboBoxItem;
                    if (aComboBoxItem.Content.ToString().ToLower() == value.ToString().ToLower())
                    {
                        this.SelectedItem = aComboBoxItem;
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
