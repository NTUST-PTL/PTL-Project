using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using PTL.Extensions.ReflectionExtensions;

namespace PTL.Windows.VisualExtensions
{
    public static class VisualTreeExtention
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;
                    foreach (var childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static List<T> FindVisualChildren2<T>(this object depObj, Predicate<object> ReachTerminate = null) where T : DependencyObject
        {
            List<T> targetFounded = new List<T>();
            if (depObj != null)
            {
                IEnumerable childrens = (IEnumerable)depObj.GetValueByPath("Children");
                if (childrens != null)
                {
                    foreach (var child in childrens)
                    {
                        if (child != null && child is T)
                            targetFounded.Add((T)child);
                        if (ReachTerminate != null)
                        {
                            if (!ReachTerminate(child))
                            {
                                var result = FindVisualChildren2<T>(child, ReachTerminate);
                                if (result.Count != 0)
                                    targetFounded.AddRange(result);
                            }
                        }
                        else
                        {
                            var result = FindVisualChildren2<T>(child);
                            if (result.Count != 0)
                                targetFounded.AddRange(result);
                        }
                    }
                    return targetFounded;
                }

                var content = depObj.GetValueByPath("Content");
                if (content != null)
                {
                    if (content is T)
                        targetFounded.Add((T)content);
                    if (ReachTerminate != null)
                    {
                        if (!ReachTerminate(content))
                        {
                            var result = FindVisualChildren2<T>(content, ReachTerminate);
                            if (result.Count != 0)
                                targetFounded.AddRange(result);
                        }
                    }
                    else
                    {
                        var result = FindVisualChildren2<T>(content);
                        if (result.Count != 0)
                            targetFounded.AddRange(result);
                    }
                    return targetFounded;
                }


                IEnumerable Items = (IEnumerable)depObj.GetValueByPath("Items");
                if (Items != null)
                {
                    foreach (var child in Items)
                    {
                        if (child != null && child is T)
                            targetFounded.Add((T)child);
                        if (ReachTerminate != null)
                        {
                            if (!ReachTerminate(child))
                            {
                                var result = FindVisualChildren2<T>(child, ReachTerminate);
                                if (result.Count != 0)
                                    targetFounded.AddRange(result);
                            }
                        }
                        else
                        {
                            var result = FindVisualChildren2<T>(child);
                            if (result.Count != 0)
                                targetFounded.AddRange(result);
                        }
                    }
                }
            }
            return targetFounded;
        }
    }
}
