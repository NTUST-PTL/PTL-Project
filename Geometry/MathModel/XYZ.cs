﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PTL.Geometry.MathModel
{
    public class XYZ : PTL.Mathematics.Math2, ICloneable
    {
        public new double[] Value = new double[3] { 0, 0, 0 };
        public double X
        {
            get
            {
                return this.Value[0];
            }
            set
            {
                this.Value[0] = value;
            }
        }
        public double Y
        {
            get
            {
                return this.Value[1];
            }
            set
            {
                this.Value[1] = value;
            }
        }
        public double Z
        {
            get
            {
                return this.Value[2];
            }
            set
            {
                this.Value[2] = value;
            }
        }
        public double this[int index]
        {
            get
            {
                return this.Value[index];
            }
            set
            {
                this.Value[index] = value;
            }
        }

        public static XYZ ZeroVector { get { return new XYZ(0, 0, 0); } }

        #region Constructor and Destructor
        public XYZ(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public XYZ(double[] direction)
        {
            this.X = direction[0];
            this.Y = direction[1];
            this.Z = direction[2];
        }

        public XYZ()
        {
        }

        public XYZ(XYZ p)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Z = p.Z;
        }
        #endregion

        #region Operation
        public static XYZ operator -(XYZ p1, XYZ p2)
        {
            XYZ p3 = new XYZ();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static XYZ operator +(XYZ p1, XYZ p2)
        {
            XYZ p3 = new XYZ();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static XYZ operator *(double scale, XYZ p1)
        {
            XYZ po = new XYZ();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static XYZ operator *(XYZ p1, double scale)
        {
            XYZ po = new XYZ();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static XYZ operator /(XYZ p1, double denominator)
        {
            XYZ po = new XYZ();

            po.X = p1.X / denominator;
            po.Y = p1.Y / denominator;
            po.Z = p1.Z / denominator;

            return po;
        }
        public static implicit operator double[](XYZ p1)
        {
            return p1.Value;
        }
        public static implicit operator XYZ(double[] p1)
        {
            return new XYZ(p1);
        }
        #endregion

        public object Clone()
        {
            XYZ aCoordinate = new XYZ();
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

        public void Transform3(double[,] TransformMatrix)
        {
            this.Value = TransCoordinate3(TransformMatrix, this.Value);
        }

        public void Transform4(double[,] TransformMatrix)
        {
            this.Value = TransCoordinate4(TransformMatrix, this.Value);
        }

        public double[] ToArray()
        {
            return this.Value;
        }
    }
}