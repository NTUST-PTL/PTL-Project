using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PTL.Base
{
    public class Source<T>
    {
        public Source()
        {

        }
        public Source(T value)
        {
            this.V = value;
        }

        protected T _V;
        public virtual T V
        {
            get { return _V; }
            set {
                this._V = value;
                NoticeChange();
            }
        }

        HashSet<Link<T>> Observers = new HashSet<Link<T>>();
        public Action ValueChanged;

        public T GetSourceMethod()
        {
            return V;
        }

        public void SetSourceMethod(T value)
        {
            this.V = value;
        }

        public void NoticeChange()
        {
            foreach (var observer in Observers)
                if (observer.ValueChanged != null)
                    observer.ValueChanged();
            if (this.ValueChanged != null)
                this.ValueChanged();
        }

        public void LinkWith(params Link<T>[] links)
        {
            LinkWith(true, links);
        }

        public void LinkWith(bool bilateral, params Link<T>[] links)
        {
            foreach (var link in links)
            {
                if (bilateral)
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
