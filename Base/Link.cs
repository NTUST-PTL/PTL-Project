using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PTL.Base
{
    public class Link<T>
    {
        public Link()
        {

        }
        public Link(Source<T> source)
        {
            source.LinkWith(this);
        }

        public Source<T> Source;
        public Func<T> GetSource;
        public Action<T> SetSource;
        public Action ValueChanged;

        public T V
        {
            get { return GetSource(); }
            set { SetSource(value); }
        }

        public void LinkTo(Source<T> source)
        {
            LinkTo(true, source);
        }

        public void LinkTo(bool bilateral, Source<T> source)
        {
            if (source != null)
            {
                this.GetSource = source.GetSourceMethod;
                this.SetSource = source.SetSourceMethod;
                this.Source = source;
                if (bilateral)
                    source.LinkWith(false, this);
            }
        }

        public void Delink()
        {
            this.GetSource = null;
            this.SetSource = null;
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
