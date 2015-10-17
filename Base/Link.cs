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
        public Link(Source<T> source)
        {
            source.LinkWith(this);
        }

        public Source<T> Source;
        public Func<T> GetSourceValue;
        public Action<T> SetSourceValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public T V
        {
            get { return GetSourceValue(); }
            set { SetSourceValue(value); }
        }

        public void NoticeChange()
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs("V"));
        }

        public void LinkTo(Source<T> source)
        {
            LinkTo(true, source);
        }

        public void LinkTo(bool bilateral, Source<T> source)
        {
            if (source != null)
            {
                this.GetSourceValue = source.GetValue;
                this.SetSourceValue = source.SetValue;
                this.Source = source;
                if (bilateral)
                    source.LinkWith(false, this);
            }
        }

        public void Delink()
        {
            this.GetSourceValue = null;
            this.SetSourceValue = null;
            this.Source = null;
            if (this.Source != null)
                this.Source.DelinkWith(this);
        }

        public static implicit operator T(Link<T> link)
        {
            return link.V;
        }
    }
}
