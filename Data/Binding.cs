using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;
using System.Reflection;

namespace PTL.Data
{
    public class Binding
    {
        public INotifyPropertyChanged Target
        {
            get;
            protected set;
        }
        public INotifyPropertyChanged Source
        {
            get;
            protected set;
        }
        public string TargetPropertyName
        {
            get;
            protected set;
        }
        public string SourcePropertyName
        {
            get;
            protected set;
        }
        public PropertyChangedEventHandler TargetPropertyChangedEventHandler;
        public PropertyChangedEventHandler SourcePropertyChangedEventHandler;
        public IValueConverter Converter;
        public object Paremeters;
        public CultureInfo Culture;

        public static Binding Bind(INotifyPropertyChanged target, string targetPropertyName, INotifyPropertyChanged source, string sourcePropertyName, IValueConverter converter = null, object parameters = null, CultureInfo culture = null)
        {
            Binding newBinding = new Binding();
            newBinding.Target = target;
            newBinding.Source = source;
            newBinding.TargetPropertyName = targetPropertyName;
            newBinding.SourcePropertyName = sourcePropertyName;
            newBinding.Converter = converter;
            newBinding.Paremeters = parameters;
            newBinding.Culture = culture;

            newBinding.TargetPropertyChangedEventHandler = (o, e) =>
            {
                if (o == newBinding.Target && e.PropertyName == newBinding.TargetPropertyName)
                    newBinding.UpdateSource();
            };
            newBinding.SourcePropertyChangedEventHandler = (o, e) =>
            {
                if (o == newBinding.Source && e.PropertyName == newBinding.SourcePropertyName)
                    newBinding.UpdateTarget();
            };

            newBinding.Target.PropertyChanged += newBinding.TargetPropertyChangedEventHandler;
            newBinding.Source.PropertyChanged += newBinding.SourcePropertyChangedEventHandler;

            newBinding.UpdateTarget();

            return newBinding;
        }

        public void UpdateTarget()
        {
            PropertyInfo targetProperty = Target.GetType().GetProperty(TargetPropertyName);
            PropertyInfo sourceProperty = Source.GetType().GetProperty(SourcePropertyName);
            
            Object sourceValue = 
                Converter != null?
                Converter.Convert(
                    sourceProperty.GetValue(Source)
                    , targetProperty.PropertyType
                    , Paremeters
                    , Culture)
                : sourceProperty.GetValue(Source);

            targetProperty.SetValue(Target, sourceValue);
        }

        public void UpdateSource()
        {
            PropertyInfo targetProperty = Target.GetType().GetProperty(TargetPropertyName);
            PropertyInfo sourceProperty = Source.GetType().GetProperty(SourcePropertyName);

            Object targetValue =
                Converter != null ?
                Converter.Convert(
                    targetProperty.GetValue(Target)
                    , sourceProperty.PropertyType
                    , Paremeters
                    , Culture)
                : targetProperty.GetValue(Target);

            sourceProperty.SetValue(Source, targetValue);
        }

        public void Remove()
        {
            Target.PropertyChanged -= TargetPropertyChangedEventHandler;
            Source.PropertyChanged -= SourcePropertyChangedEventHandler;
        }
    }
}
