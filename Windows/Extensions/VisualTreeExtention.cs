using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PTL.Extensions.ReflectionExtensions;

namespace PTL.Windows.Extensions
{
    public static class VisualTreeExtention
    {
        /// <summary>
        /// Find Visual Children by VisualTreeHelper.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                if (VisualTreeHelper.GetChildrenCount(depObj) > 0)
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
                else
                {
                    var content = (depObj as ContentControl)?.Content;
                    if (content != null && content is T)
                    {
                        yield return (T)content;
                    }
                    if (content != null && content is DependencyObject)
                    {
                        foreach (var childOfContent in FindVisualChildren<T>((DependencyObject)content))
                        {
                            yield return childOfContent;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find Visual Children by reflection.
        /// Searching path includes Children, ,Child, Content, and Items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <param name="ReachTerminate"></param>
        /// <returns></returns>
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

                {
                    var child = depObj.GetValueByPath("Child");
                    if (child != null)
                    {
                        if (child is T)
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
                        return targetFounded;
                    }
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

        /// <summary>
        /// Find Visual Children by reflection.
        /// Searching with provided paths.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paths"></param>
        /// <param name="depObj"></param>
        /// <param name="ReachTerminate"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren2<T>(this object obj, string[] paths)
        {
            if (obj != null)
            {
                Type objType = obj.GetType();
                foreach (var path in paths)
                {
                    var property = objType.GetProperty(path);
                    if (property == null)
                        continue;

                    var value = property.GetValue(obj);
                    if (value is T)
                        yield return (T)value;
                    foreach (var item in FindVisualChildren2<T>(value, paths))
                    {
                        yield return item;
                    }

                    if (value.GetType().GetInterface(typeof(IEnumerable<>).FullName) != null)
                    {
                        foreach (var child in value as IEnumerable)
                        {
                            if (child is T)
                                yield return (T)child;
                            foreach (var item in FindVisualChildren2<T>(child, paths))
                            {
                                yield return item;
                            }
                        }
                    }
                }
            }
        }
    }
}
