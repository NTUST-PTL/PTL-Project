using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PTL.Base
{
    public class Source<T> : INotifyPropertyChanged
    {
        public Source()
        {

        }
        public Source(T value)
        {
            this.Value = value;
        }

        public T _Value;
        public virtual T Value
        {
            get { return _Value; }
            set
            {
                if (this._Value?.GetHashCode() != value?.GetHashCode())
                {
                    this._Value = value;
                    NotidfyPropertyChanged("V");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public T GetValue()
        {
            return Value;
        }

        public void SetValue(T value)
        {
            this.Value = value;
        }

        public void NotidfyPropertyChanged(String propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LinkWith(params Link<T>[] links)
        {
            foreach (var link in links)
            {
                link.GetSourceValue = () => Value;
                link.SetSourceValue = (v) => Value = v;
                this.PropertyChanged += (s, e) => link.NotifyPropertyChanged(nameof(link.Value));
            }
        }

        public void DelinkWith(params Link<T>[] links)
        {
            foreach (var link in links)
            {
                link.GetSourceValue = null;
                link.SetSourceValue = null;
            }
        }

        public static implicit operator T(Source<T> source)
        {
            return source.Value;
        }
    }
}
