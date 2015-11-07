using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using PTL.ReflectionExtensions;

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
                try
                {
                    IEnumerable childrens = (IEnumerable)depObj.GetValueByPath("Children");
                    foreach (var child in childrens)
                    {
                        if (child != null && child is T)
                            targetFounded.Add((T)child);
                        if (ReachTerminate != null)
                        {
                            if (!ReachTerminate(child))
                                targetFounded.AddRange(FindVisualChildren2<T>(child, ReachTerminate));
                        }
                        else
                            targetFounded.AddRange(FindVisualChildren2<T>(child));
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        var content = depObj.GetValueByPath("Content");
                        if (content is T)
                            targetFounded.Add((T)content);
                        if (ReachTerminate != null)
                        {
                            if (!ReachTerminate(content))
                                targetFounded.AddRange(FindVisualChildren2<T>(content, ReachTerminate));
                        }
                        else
                            targetFounded.AddRange(FindVisualChildren2<T>(content));
                    }
                    catch
                    {
                        try
                        {
                            IEnumerable childrens = (IEnumerable)depObj.GetValueByPath("Items");
                            foreach (var child in childrens)
                            {
                                if (child != null && child is T)
                                    targetFounded.Add((T)child);
                                if (ReachTerminate != null)
                                {
                                    if (!ReachTerminate(child))
                                        targetFounded.AddRange(FindVisualChildren2<T>(child, ReachTerminate));
                                }
                                else
                                    targetFounded.AddRange(FindVisualChildren2<T>(child));
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return targetFounded;
        }
    }
}
