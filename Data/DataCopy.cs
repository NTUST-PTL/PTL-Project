using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Data
{
    public class DataCopy
    {
        public static void CopyFieldByReflection(object target, object source)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach (var sourceField in sourceType.GetFields())
            {
                var targetField = targetType.GetField(sourceField.Name);
                if (targetField != null
                    && !(targetField.GetCustomAttributes(typeof(Ignore), true).Cast<Ignore>().Count() > 0))
                    targetField?.SetValue(target, sourceField.GetValue(source));
            }
        }

        public static void CopyPropertyByReflection(object target, object source)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach (var sourceProperty in sourceType.GetProperties())
            {
                var targetProperty = targetType.GetProperty(sourceProperty.Name);
                if (targetProperty != null
                    && !(targetProperty.GetCustomAttributes(typeof(Ignore), true).Cast<Ignore>().Count() > 0))
                    targetProperty?.SetValue(target, sourceProperty.GetValue(source));
            }
        }

        public static void CopyByReflection(object target, object source)
        {
            CopyFieldByReflection(target, source);
            CopyPropertyByReflection(target, source);
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class Ignore : Attribute
        {

        }
    }
}
