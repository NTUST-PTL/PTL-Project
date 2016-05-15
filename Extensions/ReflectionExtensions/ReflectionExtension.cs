using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Extensions.ReflectionExtensions
{
    public static class ReflectionExtension
    {
        public static object GetValueByPath(this Type type, object obj, String path, bool ignoreError = true)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (type != obj.GetType())
                throw new ArgumentException("type is not type of obj");
            if (path == null)
                throw new ArgumentNullException("path");

            String[] sectors = path.Split('.');
            for (int i = 0; i < sectors.Length; i++)
            {
                String elementName = sectors[i];
                MemberInfo[] memberInfos = type.GetMember(elementName);
                MemberInfo memberInfo = memberInfos.Count() > 0 ? memberInfos.First() : null;
                if (memberInfo == null)
                {
                    if (ignoreError)
                        return null;
                    else
                        throw new NullReferenceException("Object : " + obj + "Element Name : " + elementName);
                }
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Constructor:
                        throw new InvalidOperationException(elementName + " is a Constructor");
                    case MemberTypes.Event:
                        throw new InvalidOperationException(elementName + " is a Event");
                    case MemberTypes.Field:
                        obj = ((FieldInfo)memberInfo).GetValue(obj);
                        break;
                    case MemberTypes.Method:
                        throw new InvalidOperationException(elementName + " is a Constructor");
                    case MemberTypes.Property:
                        obj = ((PropertyInfo)memberInfo).GetValue(obj);
                        break;
                    case MemberTypes.TypeInfo:
                        throw new InvalidOperationException(elementName + " is a TypeInfo");
                    case MemberTypes.Custom:
                        throw new InvalidOperationException(elementName + " is Custom???");
                    case MemberTypes.NestedType:
                        throw new InvalidOperationException(elementName + " is  NestedType");
                    case MemberTypes.All:
                        throw new NotSupportedException("Unsure member type");
                    default:
                        throw new NotSupportedException("Unkown member type");
                }
                if (i == sectors.Length - 1)
                {
                    return obj;
                }
                else
                {
                    if (obj == null)
                    {
                        if (ignoreError)
                            return null;
                        else
                            throw new NullReferenceException("Object : " + obj + "Element Name : " + elementName);
                    }
                    type = obj.GetType();
                }
                
            }
            return obj;
        }

        public static object GetValueByPath(this object obj, String path)
        {
            return GetValueByPath(obj.GetType(), obj, path);
        }

        public static void SetValueByPath(this Type type, object obj, String path, object value, bool ignoreError = true)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (type != obj.GetType())
                throw new ArgumentException("type is not type of obj");
            if (path == null)
                throw new ArgumentNullException("path");

            String[] sectors = path.Split('.');
            for (int i = 0; i < sectors.Length; i++)
            {
                String elementName = sectors[i];
                MemberInfo[] memberInfos = type.GetMember(elementName);
                MemberInfo memberInfo = memberInfos.Count() > 0 ? memberInfos.First() : null;
                if (memberInfo == null)
                {
                    if (ignoreError)
                        return;
                    else
                        throw new NullReferenceException();
                }
                if (i != sectors.Length - 1)
                {
                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Constructor:
                            throw new InvalidOperationException(elementName + " is a Constructor");
                        case MemberTypes.Event:
                            throw new InvalidOperationException(elementName + " is a Event");
                        case MemberTypes.Field:
                            obj = ((FieldInfo)memberInfo).GetValue(obj);
                            break;
                        case MemberTypes.Method:
                            throw new InvalidOperationException(elementName + " is a Constructor");
                        case MemberTypes.Property:
                            obj = ((PropertyInfo)memberInfo).GetValue(obj);
                            break;
                        case MemberTypes.TypeInfo:
                            throw new InvalidOperationException(elementName + " is a TypeInfo");
                        case MemberTypes.Custom:
                            throw new InvalidOperationException(elementName + " is Custom???");
                        case MemberTypes.NestedType:
                            throw new InvalidOperationException(elementName + " is  NestedType");
                        case MemberTypes.All:
                            throw new NotSupportedException("Unsure member type");
                        default:
                            throw new NotSupportedException("Unkown member type");
                    }
                    if (obj == null)
                    {
                        if (ignoreError)
                            return;
                        else
                            throw new NullReferenceException();
                    }
                    type = obj.GetType();
                }
                else
                {
                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Constructor:
                            throw new InvalidOperationException(elementName + " is a Constructor");
                        case MemberTypes.Event:
                            throw new InvalidOperationException(elementName + " is a Event");
                        case MemberTypes.Field:
                            ((FieldInfo)memberInfo).SetValue(obj, value);
                            break;
                        case MemberTypes.Method:
                            throw new InvalidOperationException(elementName + " is a Constructor");
                        case MemberTypes.Property:
                            ((PropertyInfo)memberInfo).SetValue(obj, value);
                            break;
                        case MemberTypes.TypeInfo:
                            throw new InvalidOperationException(elementName + " is a TypeInfo");
                        case MemberTypes.Custom:
                            throw new InvalidOperationException(elementName + " is Custom???");
                        case MemberTypes.NestedType:
                            throw new InvalidOperationException(elementName + " is  NestedType");
                        case MemberTypes.All:
                            throw new NotSupportedException("Unsure member type");
                        default:
                            throw new NotSupportedException("Unkown member type");
                    }
                }

            }
        }

        public static void SetValueByPath(this object obj, String path, object value)
        {
            SetValueByPath(obj.GetType(), obj, path, value);
        }
    }
}
