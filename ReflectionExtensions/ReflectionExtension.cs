using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PTL.ReflectionExtensions
{
    public class ReflectionExtension
    {
        public static object GetMemberByPath(this object obj, String path)
        {
            Type currentType = o.GetType();

            foreach (var elementName in path.Split('.'))
            {
                MemberInfo elementInfo = currentType.GetMember(elementName).First();
                switch (elementInfo.MemberType)
                {
                    case MemberTypes.Constructor:
                        break;
                    case MemberTypes.Event:
                        break;
                    case MemberTypes.Field:
                        break;
                    case MemberTypes.Method:
                        break;
                    case MemberTypes.Property:
                        break;
                    case MemberTypes.TypeInfo:
                        break;
                    case MemberTypes.Custom:
                        break;
                    case MemberTypes.NestedType:
                        break;
                    case MemberTypes.All:
                        break;
                    default:
                        break;
                }
                if (elementInfo.MemberType == MemberTypes.Constructor )
                {

                } 
            }
        }
    }
}
