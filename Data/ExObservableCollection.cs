﻿using System;
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

        public void AddRange(params T[] datas)
        {
            AddRange(datas as IEnumerable<T>);
        }

        public void RemoveRange(params T[] datas)
        {
            RemoveRange(datas as IEnumerable<T>);
        }

        public void ReplaceAll(params T[] datas)
        {
            ReplaceAll(datas as IEnumerable<T>);
        }

        public void AddRange(IEnumerable<T> datas)
        {
            foreach (var item in datas)
            {
                if (item != null)
                    Items.Add(item);
            }
            CollectionChanged?.Invoke(
                this,
                new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, datas));
        }

        public void RemoveRange(IEnumerable<T> datas)
        {
            foreach (var item in datas)
            {
                if (item != null)
                    Items.Remove(item);
            }
            CollectionChanged?.Invoke(
                this,
                new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, datas));
        }

        public void ReplaceAll(IEnumerable<T> datas)
        {
            Items.Clear();
            foreach (var item in datas)
            {
                if (item != null)
                    Items.Add(item);
            }
            CollectionChanged?.Invoke(
                this,
                new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, datas));
        }
    }
}
