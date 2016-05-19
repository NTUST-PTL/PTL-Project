using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PTL.Base;

namespace PTL.Windows.Controls
{
    public class BTextBlock : TextBlock, IBindable
    {
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
                    this.Text = this.value;
                    ValueChanged?.Invoke(this, this.value);
                }
            }
        }


        public event Func<Object, Object, bool> ValueChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public bool _BindedValueChanged(object sender, object value)
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

        public BTextBlock()
            : base()
        {
            this.ValueChanged += (object sender, object e) =>
            {
                this.V = this.Text;
                return true;
            };
            this.BindedValueChanged = _BindedValueChanged;
        }
    }
}
