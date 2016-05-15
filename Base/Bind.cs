using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTL.Extensions.ReflectionExtensions;

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

        public static void SetBinding(
            INotifyPropertyChanged element1, 
            String path1, 
            INotifyPropertyChanged element2, 
            String path2, 
            Func<object, object> ConvertValeToType2 = null,
            Func<object, object> ConvertValeToType1 = null)
        {
            if (element1 == null || path1 == null || element2 == null || path2 == null)
                return;

            element1.PropertyChanged +=
                (o, e) =>
                {
                    if (e.PropertyName != path1.Split('.').Last())
                        return;
                    if (ConvertValeToType2 == null)
                        element2.SetValueByPath(path2, o.GetValueByPath(path1));
                    else
                        element2.SetValueByPath(path2, ConvertValeToType2(o.GetValueByPath(path1)));
                };
            element2.PropertyChanged +=
                (obj, e) =>
                {
                    if (e.PropertyName != path2.Split('.').Last())
                        return;
                    if (ConvertValeToType1 == null)
                        element1.SetValueByPath(path1, obj.GetValueByPath(path1));
                    else
                        element1.SetValueByPath(path1, ConvertValeToType1(obj.GetValueByPath(path1)));
                };
        }
    }
}
