using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry.MathModel
{
    public class XYZ4 : PTL.Mathematics.ProtectedPTLM, IXYZ
    {
        protected double[] values;
        public double[] Values
        {
            get { return values; }
            set { values = value; }
        }

        [Newtonsoft.Json.JsonIgnore]
        public double X
        {
            get
            {
                return this.Values[0];
            }
            set
            {
                this.Values[0] = value;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public double Y
        {
            get
            {
                return this.Values[1];
            }
            set
            {
                this.Values[1] = value;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public double Z
        {
            get
            {
                return this.Values[2];
            }
            set
            {
                this.Values[2] = value;
            }
        }

        public double this[int index]
        {
            get
            {
                return this.Values[index];
            }
            set
            {
                this.Values[index] = value;
            }
        }

        public bool IsHomogeneous
        {
            get
            {
                return true;
            }
        }

        #region Constructor and Destructor
        public XYZ4(double x, double y, double z)
        {
            this.Values = new double[3] { x, y, z };
        }

        public XYZ4(double[] XYZ)
        {
            this.values = XYZ;
            //this.X = XYZ[0];
            //this.Y = XYZ[1];
            //this.Z = XYZ[2];
        }

        public XYZ4()
        {
            this.Values = new double[3] { 0, 0, 0 };
        }

        public object New(double[] XYZ)
        {
            return new XYZ4(XYZ);
        }
        #endregion

        #region Operation
        public static XYZ4 operator -(XYZ4 p1, XYZ4 p2)
        {
            XYZ4 p3 = new XYZ4();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static XYZ4 operator -(XYZ4 p1, XYZ3 p2)
        {
            XYZ4 p3 = new XYZ4();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static XYZ4 operator -(XYZ3 p1, XYZ4 p2)
        {
            XYZ4 p3 = new XYZ4();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static XYZ4 operator +(XYZ4 p1, XYZ4 p2)
        {
            XYZ4 p3 = new XYZ4();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static XYZ4 operator +(XYZ4 p1, XYZ3 p2)
        {
            XYZ4 p3 = new XYZ4();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static XYZ4 operator +(XYZ3 p1, XYZ4 p2)
        {
            XYZ4 p3 = new XYZ4();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static XYZ4 operator *(double scale, XYZ4 p1)
        {
            XYZ4 po = new XYZ4();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static XYZ4 operator *(XYZ4 p1, double scale)
        {
            XYZ4 po = new XYZ4();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static XYZ4 operator /(XYZ4 p1, double denominator)
        {
            XYZ4 po = new XYZ4();

            po.X = p1.X / denominator;
            po.Y = p1.Y / denominator;
            po.Z = p1.Z / denominator;

            return po;
        }
        public static implicit operator double[] (XYZ4 p1)
        {
            return p1.Values;
        }
        public static implicit operator XYZ4(double[] p1)
        {
            return new XYZ4(p1);
        }
        public static implicit operator XYZ3(XYZ4 p1)
        {
            return new XYZ3(p1.values);
        }
        #endregion

        public void Transform(double[,] M)
        {
            Values = Transport4(M, Values);
        }

        public virtual object Clone()
        {
            XYZ4 aCoordinate = new XYZ4();
            aCoordinate.X = this.X;
            aCoordinate.Y = this.Y;
            aCoordinate.Z = this.Z;
            return aCoordinate;
        }

        public override string ToString()
        {
            return "{" + this.X + "," + this.Y + "," + this.Z + "}";
        }

        public string ToString(string Format)
        {
            return "{" + this.X.ToString(Format) + "," + this.Y.ToString(Format) + "," + this.Z.ToString(Format) + "}";
        }
    }
}
