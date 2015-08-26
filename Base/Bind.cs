using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PTL.Base
{
    public interface IBindable
    {
        event Func<Object, Object, bool> ValueChanged;
        [JsonIgnore] 
        Func<Object, Object, bool> BindedValueChanged { get; set; }
    }

    public static class Bind
    {
        public static void CreateBinding(IBindable Bindable1, IBindable Bindable2)
        {
            Bindable1.ValueChanged += Bindable2.BindedValueChanged;
            Bindable2.ValueChanged += Bindable1.BindedValueChanged;
        }

        public static void RemoveBinding(IBindable Bindable1, IBindable Bindable2)
        {
            Bindable1.ValueChanged -= Bindable2.BindedValueChanged;
            Bindable2.ValueChanged -= Bindable1.BindedValueChanged;
        }
    }
}
