using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PTL.Data
{
    public class ExObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        public void AddRange( params T[] datas)
        {
            foreach (var item in datas)
            {
                Items.Add(item);
            }
            CollectionChanged(
                this, 
                new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, datas));
        }

        public void RemoveRange(params T[] datas)
        {
            foreach (var item in datas)
            {
                Items.Remove(item);
            }
            CollectionChanged(
                this,
                new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, datas));
        }
    }
}
