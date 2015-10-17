using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Base.Unit
{
    public interface IUnit
    {
        void ToDefaultUnit(ref object value);
        void FromDefaulUnit(ref object value);
    }

    public class Rad : IUnit //Default Unit Type
    {
        public void ToDefaultUnit(ref object value)
        {
            return;
        }
        public void FromDefaulUnit(ref object value)
        {
            return;
        }
        public override string ToString()
        {
            return "Rad";
        }
    }
    public class Deg : IUnit
    {
        public void ToDefaultUnit(ref object value)
        {
            value = Convert.ToDouble(value) / 180.0 * System.Math.PI;
            return;
        }
        public void FromDefaulUnit(ref object value)
        {
            value = Convert.ToDouble(value) / System.Math.PI * 180.0;
            return;
        }
        public override string ToString()
        {
            return "Deg";
        }
    }
    public class mm : IUnit //Default Unit Type
    {
        public void ToDefaultUnit(ref object value)
        {
            return;
        }
        public void FromDefaulUnit(ref object value)
        {
            return;
        }
        public override string ToString()
        {
            return "mm";
        }
    }
    public class Meter : IUnit
    {
        public void ToDefaultUnit(ref object value)
        {
            value = Convert.ToDouble(value) * 1000.0;
            return;
        }
        public void FromDefaulUnit(ref object value)
        {
            value = Convert.ToDouble(value) / 1000.0;
            return;
        }
        public override string ToString()
        {
            return "Meter";
        }
    }
    public class Inch : IUnit
    {
        public void ToDefaultUnit(ref object value)
        {
            value = Convert.ToDouble(value) * 25.4;
            return;
        }
        public void FromDefaulUnit(ref object value)
        {
            value = Convert.ToDouble(value) / 25.4;
            return;
        }
        public override string ToString()
        {
            return "Inch";
        }
    }
    public class Module : IUnit//Default Unit Type
    {
        public void ToDefaultUnit(ref object value)
        {
            return;
        }
        public void FromDefaulUnit(ref object value)
        {
            return;
        }
        public override string ToString()
        {
            return "Module";
        }
    }
    public class DP : IUnit
    {
        public void ToDefaultUnit(ref object value)
        {
            value = 25.4 / Convert.ToDouble(value);
            return;
        }
        public void FromDefaulUnit(ref object value)
        {
            value = 25.4 / Convert.ToDouble(value);
            return;
        }
        public override string ToString()
        {
            return "DP";
        }
    }
}
