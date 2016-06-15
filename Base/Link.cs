using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PTL.Base
{
    public class Link<T> : INotifyPropertyChanged
    {
        public Link()
        {

        }
        public Link(Func<T> getSourceValue, Action<T> setSourceValue)
        {
            GetSourceValue = getSourceValue;
            SetSourceValue = setSourceValue;
        }

        public Func<T> GetSourceValue;
        public Action<T> SetSourceValue;

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public T Value
        {
            get { return GetSourceValue(); }
            set {
                if (Value != null && !Value.Equals(value))
                {
                    SetSourceValue(value);
                    NotifyPropertyChanged(nameof(Value));
                }
                
            }
        }

        public void LinkTo(Source<T> source)
        {
            GetSourceValue = source.GetValue;
            SetSourceValue = source.SetValue;
        }

        public static implicit operator T(Link<T> link)
        {
            return link.Value;
        }
    }
}
