﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Base.Unit;

namespace PTL.Base.DataType
{
    public interface INVU<T> : IBindable
    {
        String Name { get; set; }
        T Value { get; set; }
        IUnit Unit { get; set; }

        event EventHandler<String> NameChanged;
        event EventHandler<IUnit> UnitChanged;

        void ChangeUnit(IUnit newUnit);
    }

    public class IntNVU : INVU<int>, ICloneable
    {
        String name;
        int value;
        IUnit unit;

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    NameChanged?.Invoke(this, this.name);
                }
            }
        }
        /// <summary>
        /// Value
        /// </summary>
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                try
                {
                    if (this.value != value)
                    {
                        this.value = value;
                        ValueChanged?.Invoke(this, this.value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        public IUnit Unit
        {
            get
            {
                return this.unit;
            }
            set
            {
                if (this.unit != value)
                {
                    this.unit = value;
                    UnitChanged?.Invoke(this, this.unit);
                }
            }
        }

        public event EventHandler<string> NameChanged;
        public event Func<object, object, bool> ValueChanged;
        public event EventHandler<IUnit> UnitChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public IntNVU()
        {
            this.BindedValueChanged = this._BindedValueChanged;
        }

        private bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                this.Value = Convert.ToInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ChangeUnit(IUnit newUnit)
        {
            Object tv = this.value;
            if (this.Unit != null)
                this.Unit.ToDefaultUnit(ref tv);
            newUnit.FromDefaulUnit(ref tv);
            this.Value = Convert.ToInt32(tv);
            this.Unit = newUnit;
        }

        #region 隱含轉換為 其他型別
        public static implicit operator decimal (IntNVU v)
        {
            return Convert.ToDecimal(v.Value);
        }
        public static implicit operator double (IntNVU v)
        {
            return Convert.ToDouble(v.Value);
        }
        public static implicit operator float (IntNVU v)
        {
            return Convert.ToSingle(v.Value);
        }
        public static implicit operator long (IntNVU v)
        {
            return Convert.ToInt64(v.Value);
        }
        public static implicit operator int (IntNVU v)
        {
            return Convert.ToInt32(v.Value);
        }
        public static explicit operator short (IntNVU v)
        {
            return Convert.ToInt16(v.Value);
        }
        public static implicit operator string (IntNVU v)
        {
            return v.Value.ToString();
        }
        #endregion

        #region 隱含轉換為 IntNVU
        public static explicit operator IntNVU(decimal v)
        {
            return new IntNVU() { Value = Convert.ToInt32(v) };
        }
        public static explicit operator IntNVU(double v)
        {
            return new IntNVU() { Value = Convert.ToInt32(v) };
        }
        public static explicit operator IntNVU(float v)
        {
            return new IntNVU() { Value = Convert.ToInt32(v) };
        }
        public static explicit operator IntNVU(long v)
        {
            return new IntNVU() { Value = Convert.ToInt32(v) };
        }
        public static explicit operator IntNVU(int v)
        {
            return new IntNVU() { Value = v };
        }
        public static explicit operator IntNVU(short v)
        {
            return new IntNVU() { Value = v };
        }
        public static explicit operator IntNVU(string v)
        {
            return new IntNVU() { Value = Convert.ToInt32(v) };
        }
        #endregion

        #region ICloneable 成員

        public Object Clone()
        {
            return new IntNVU() { name = this.name, value = this.value, unit = this.unit };
        }

        #endregion
    }
    public class DoubleNVU : INVU<double>, ICloneable
    {
        String name;
        double value;
        IUnit unit;

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    NameChanged?.Invoke(this, this.name);
                }
            }
        }
        /// <summary>
        /// Value
        /// </summary>
        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    ValueChanged?.Invoke(this, this.value);
                }
            }
        }
        public IUnit Unit
        {
            get
            {
                return this.unit;
            }
            set
            {
                if (this.unit != value)
                {
                    this.unit = value;
                    UnitChanged?.Invoke(this, this.unit);
                }
            }
        }

        public event EventHandler<string> NameChanged;
        public event Func<object, object, bool> ValueChanged;
        public event EventHandler<IUnit> UnitChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public DoubleNVU()
        {
            this.BindedValueChanged = _BindedValueChanged;
        }

        private bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                this.Value = Convert.ToDouble(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ChangeUnit(IUnit newUnit)
        {
            Object tv = this.Value;
            if (this.Unit != null)
                this.Unit.ToDefaultUnit(ref tv);
            newUnit.FromDefaulUnit(ref tv);
            this.Value = Convert.ToDouble(tv);
            this.Unit = newUnit;
        }

        #region 隱含轉換為 其他型別
        public static implicit operator decimal (DoubleNVU v)
        {
            return Convert.ToDecimal(v.Value);
        }
        public static implicit operator double (DoubleNVU v)
        {
            return Convert.ToDouble(v.Value);
        }
        public static implicit operator float (DoubleNVU v)
        {
            return Convert.ToSingle(v.Value);
        }
        public static explicit operator long (DoubleNVU v)
        {
            return Convert.ToInt64(v.Value);
        }
        public static explicit operator int (DoubleNVU v)
        {
            return Convert.ToInt32(v.Value);
        }
        public static explicit operator short (DoubleNVU v)
        {
            return Convert.ToInt16(v.Value);
        }

        public static implicit operator string (DoubleNVU v)
        {
            return v.Value.ToString();
        }
        #endregion

        #region 轉換為 DoubleNVU
        public static explicit operator DoubleNVU(double v)
        {
            return new DoubleNVU() { Value = v };
        }
        public static explicit operator DoubleNVU(float v)
        {
            return new DoubleNVU() { Value = v };
        }
        public static explicit operator DoubleNVU(short v)
        {
            return new DoubleNVU() { Value = v };
        }
        public static explicit operator DoubleNVU(int v)
        {
            return new DoubleNVU() { Value = v };
        }
        public static explicit operator DoubleNVU(long v)
        {
            return new DoubleNVU() { Value = v };
        }
        public static explicit operator DoubleNVU(string v)
        {
            return new DoubleNVU() { Value = Convert.ToDouble(v) };
        }
        #endregion

        #region ICloneable 成員

        public Object Clone()
        {
            return new DoubleNVU() { name = this.name, value = this.value, unit = this.unit };
        }

        #endregion
    }
    public class StringNVU : INVU<string>, ICloneable
    {
        String name;
        String value;
        IUnit unit;

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    NameChanged?.Invoke(this, this.name);
                }
            }
        }
        /// <summary>
        /// Value
        /// </summary>
        public String Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    ValueChanged?.Invoke(this, this.value);
                }
            }
        }
        public IUnit Unit
        {
            get
            {
                return this.unit;
            }
            set
            {
                if (this.unit != value)
                {
                    this.unit = value;
                    UnitChanged?.Invoke(this, this.unit);
                }
            }
        }

        public event EventHandler<string> NameChanged;
        public event Func<object, object, bool> ValueChanged;
        public event EventHandler<IUnit> UnitChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public StringNVU()
        {
            BindedValueChanged = _BindedValueChanged;
        }

        private bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                this.Value = value.ToString();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public void ChangeUnit(IUnit newUnit)
        {
            Object tv = this.Value;
            if (this.Unit != null)
                this.Unit.ToDefaultUnit(ref tv);
            newUnit.FromDefaulUnit(ref tv);
            this.Value = tv.ToString();
            this.Unit = newUnit;
        }

        #region 隱含轉換為 其他型別
        public static explicit operator decimal (StringNVU v)
        {
            return Convert.ToDecimal(v.Value);
        }
        public static explicit operator double (StringNVU v)
        {
            return Convert.ToDouble(v.Value);
        }
        public static explicit operator float (StringNVU v)
        {
            return Convert.ToSingle(v.Value);
        }
        public static explicit operator long (StringNVU v)
        {
            return Convert.ToInt64(v.Value);
        }
        public static explicit operator int (StringNVU v)
        {
            return Convert.ToInt32(v.Value);
        }
        public static explicit operator short (StringNVU v)
        {
            return Convert.ToInt16(v.Value);
        }

        public static implicit operator string (StringNVU v)
        {
            return v.Value;
        }
        #endregion

        #region 隱含轉換為 DoubleNVU
        public static explicit operator StringNVU(double v)
        {
            return new StringNVU() { Value = v.ToString() };
        }
        public static explicit operator StringNVU(float v)
        {
            return new StringNVU() { Value = v.ToString() };
        }
        public static explicit operator StringNVU(short v)
        {
            return new StringNVU() { Value = v.ToString() };
        }
        public static explicit operator StringNVU(int v)
        {
            return new StringNVU() { Value = v.ToString() };
        }
        public static explicit operator StringNVU(long v)
        {
            return new StringNVU() { Value = v.ToString() };
        }
        public static explicit operator StringNVU(string v)
        {
            return new StringNVU() { Value = v };
        }
        #endregion

        #region ICloneable 成員
        public Object Clone()
        {
            return new StringNVU() { name = this.name, value = this.value, unit = this.unit };
        }
        #endregion

        public override string ToString()
        {
            return this.Value;
        }
    }
    public class BooleanNVU : INVU<Boolean>, ICloneable
    {
        String name;
        Boolean value;
        IUnit unit;

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    NameChanged?.Invoke(this, this.name);
                }
            }
        }
        /// <summary>
        /// Value
        /// </summary>
        public Boolean Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    ValueChanged?.Invoke(this, this.value);
                }
            }
        }
        public IUnit Unit
        {
            get
            {
                return this.unit;
            }
            set
            {
                if (this.unit != value)
                {
                    this.unit = value;
                    UnitChanged?.Invoke(this, this.unit);
                }
            }
        }

        public event EventHandler<string> NameChanged;
        public event Func<object, object, bool> ValueChanged;
        public event EventHandler<IUnit> UnitChanged;
        public Func<object, object, bool> BindedValueChanged { get; set; }

        public BooleanNVU()
        {
            this.BindedValueChanged = _BindedValueChanged;
        }

        private bool _BindedValueChanged(object sender, object value)
        {
            try
            {
                this.Value = (Boolean)value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public void ChangeUnit(IUnit newUnit)
        {
            Object tv = this.Value;
            if (this.Unit != null)
                this.Unit.ToDefaultUnit(ref tv);
            newUnit.FromDefaulUnit(ref tv);
            this.Value = (Boolean)tv;
            this.Unit = newUnit;
        }

        #region 隱含轉換為 其他型別
        public static implicit operator Boolean(BooleanNVU v)
        {
            return v.Value;
        }
        #endregion

        #region 隱含轉換為 DoubleNVU
        public static explicit operator BooleanNVU(Boolean v)
        {
            return new BooleanNVU() { Value = v };
        }
        #endregion

        #region ICloneable 成員
        public Object Clone()
        {
            return new BooleanNVU() { name = this.name, value = this.value, unit = this.unit };
        }
        #endregion

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
