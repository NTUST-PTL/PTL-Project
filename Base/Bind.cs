using System;
using System.ComponentModel;
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
        public static void CreateBinding(IBindable source, IBindable link)
        {
            source.ValueChanged += link.BindedValueChanged;
            link.ValueChanged += source.BindedValueChanged;
        }

        public static void RemoveBinding(IBindable source, IBindable link)
        {
            source.ValueChanged -= link.BindedValueChanged;
            link.ValueChanged -= source.BindedValueChanged;
        }

        public static void SetBinding(INotifyPropertyChanged element1, String path1, INotifyPropertyChanged element2, String Path2)
        {
            if (element1 == null || path1 == null || element2 == null || Path2 == null)
                return;

            PropertyChangedEventHandler element1PropertyChanged =
                (o, e) =>
                {
                    if (e.PropertyName != path1)
                        return;
                    Type element1Type = o.GetType();
                    Type element2Type = element2.GetType();
                    element1Type.
                };

        }
    }
}
