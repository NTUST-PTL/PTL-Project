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
            this.V = value;
        }

        public T _V;
        public virtual T V
        {
            get { return _V; }
            set
            {
                if (this._V?.GetHashCode() != value?.GetHashCode())
                {
                    this._V = value;
                    NoticeChange("V");
                }
            }
        }

        HashSet<Link<T>> Observers = new HashSet<Link<T>>();
        public event PropertyChangedEventHandler PropertyChanged;

        public T GetValue()
        {
            return V;
        }

        public void SetValue(T value)
        {
            this.V = value;
        }

        public void NoticeChange(String propertyName)
        {
            foreach (var observer in Observers)
                observer.NoticeChange();
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LinkWith(params Link<T>[] links)
        {
            LinkWith(true, links);
        }

        public void LinkWith(bool TwoWay, params Link<T>[] links)
        {
            foreach (var link in links)
            {
                if (TwoWay)
                    link.LinkTo(false, this);
                this.Observers.Add(link);
            }
        }

        public void DelinkWith(params Link<T>[] links)
        {
            foreach (var link in links)
            {
                if (link.Source == this)
                {
                    link.Delink();
                    this.Observers.Remove(link);
                }
            }
        }

        public static implicit operator T(Source<T> source)
        {
            return source.V;
        }
    }
}
