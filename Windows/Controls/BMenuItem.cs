using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PTL.Base;

namespace PTL.Windows.Controls
{
    public class BMenuItem : MenuItem, IBindable
    {
        public BMenuItem()
            : base()
        {
            this.BindedValueChanged = _BindedValueChanged;
        }

        public void SetItems(IEnumerable<String> itemsString)
        {
            foreach (var item in itemsString)
            {
                BMenuItem aMenuItemItem = new BMenuItem();
                aMenuItemItem.Header = item;
                aMenuItemItem.IsCheckable = true;
                aMenuItemItem.Checked += aMenuItemItem_Checked;
                this.Items.Add(aMenuItemItem);
            }
        }

        void aMenuItemItem_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.ValueChanged != null) this.ValueChanged(this, (sender as MenuItem).Header.ToString());
        }

        #region IBindable 成員

        public event Func<Object, Object, bool> ValueChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                bool selectSuccesful = false;
                foreach (var item in this.Items)
                {
                    if (item is MenuItem)
                    {
                        MenuItem aMenuItem = item as MenuItem;
                        if (aMenuItem.Header.ToString() == value.ToString())
                        {
                            aMenuItem.IsChecked = true;
                            selectSuccesful = true;
                        }
                        else
                            aMenuItem.IsChecked = false;
                    }
                    else
                        throw new NotImplementedException("不再適用範圍內的項目");
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
