using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class PointD : Vector
    {
        public PointD(double x, double y, double z)
            : base(x, y, z)
        {
            
        }

        public PointD(double[] coordinate)
        {
            this.X = coordinate[0];
            this.Y = coordinate[1];
            this.Z = coordinate[2];
        }

        public PointD()
            :base()
        {
        }

        public PointD(Vector p)
            :base(p)
        {
        }

        #region Operation
        public static PointD operator -(Vector p1, PointD p2)
        {
            PointD p3 = new PointD();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static PointD operator -(PointD p1, Vector p2)
        {
            PointD p3 = new PointD();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static PointD operator -(PointD p1, PointD p2)
        {
            PointD p3 = new PointD();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static PointD operator +(PointD p1, Vector p2)
        {
            PointD p3 = new PointD();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static PointD operator +(Vector p1, PointD p2)
        {
            PointD p3 = new PointD();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static PointD operator +(PointD p1, PointD p2)
        {
            PointD p3 = new PointD();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static PointD operator *(double scale, PointD p1)
        {
            PointD po = new PointD();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static PointD operator *(PointD p1, double scale)
        {
            PointD po = new PointD();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static PointD operator /(PointD p1, double denominator)
        {
            PointD po = new PointD();

            po.X = p1.X / denominator;
            po.Y = p1.Y / denominator;
            po.Z = p1.Z / denominator;

            return po;
        }
        public static implicit operator XYZ4(PointD p)
        {
            return new XYZ4(p.Value);
        }
        public static implicit operator PointD(XYZ4 p)
        {
            return new PointD(p.Values);
        }
        public static implicit operator PointD(double[] p)
        {
            return new PointD(p);
        }
        public static implicit operator double[](PointD p)
        {
            return p.Value;
        }
        #endregion

        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = externalCoordinateMatrix;
            if (this.CoordinateSystem != null)
                M = MatrixDot(M, this.CoordinateSystem);
            return new XYZ4[] { Transport<XYZ4>(M, this), Transport<XYZ4>(M, this) };
        }

        public override void Transform(double[,] TransformMatrix)
        {
            PointD newPoint = Transport4(TransformMatrix, this);
            this.X = newPoint.X;
            this.Y = newPoint.Y;
            this.Z = newPoint.Z;
        }

        public virtual Vector ToVector()
        {
            return new Vector(this.Value);
        }
    }
}
