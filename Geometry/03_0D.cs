using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using PTL.Definitions;
using PTL.OpenGL.Plot;
using CsGL.OpenGL;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class Vector : Entity, IHaveXYZ
    {
        public double[] Value = new double[3] { 0, 0, 0 };
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

        public static Vector ZeroVector { get { return new Vector(0, 0, 0); } }

        protected double openGLDisplaySize = 0.1;
        public double OpenGLDisplaySize
        {
            get { return openGLDisplaySize; }
            set { this.openGLDisplaySize = value; }
        }
        public Action<Vector> OpenGLDisplayFunction = (Vector p) =>
        {
            glColor4d(p.Color);
            GL.glPushMatrix();
            Translated(p);
            GL.glutSolidSphere(p.OpenGLDisplaySize, 20, 20);
            GL.glPopMatrix();
        };

        #region Constructor and Destructor
        public Vector(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector(double[] direction)
        {
            this.X = direction[0];
            this.Y = direction[1];
            this.Z = direction[2];
        }

        public Vector()
        {
        }

        public Vector(Vector p)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Z = p.Z;
        }
        #endregion

        #region Operation
        public static Vector operator -(Vector p1, Vector p2)
        {
            Vector p3 = new Vector();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

            return p3;
        }
        public static Vector operator +(Vector p1, Vector p2)
        {
            Vector p3 = new Vector();

            p3.X = p1.X + p2.X;
            p3.Y = p1.Y + p2.Y;
            p3.Z = p1.Z + p2.Z;

            return p3;
        }
        public static Vector operator *(double scale, Vector p1)
        {
            Vector po = new Vector();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static Vector operator *(Vector p1, double scale)
        {
            Vector po = new Vector();

            po.X = scale * p1.X;
            po.Y = scale * p1.Y;
            po.Z = scale * p1.Z;

            return po;
        }
        public static Vector operator /(Vector p1, double denominator)
        {
            Vector po = new Vector();

            po.X = p1.X / denominator;
            po.Y = p1.Y / denominator;
            po.Z = p1.Z / denominator;

            return po;
        }
        #endregion

        public override object Clone()
        {
            PointD aPoint = new PointD();
            aPoint.X = this.X;
            aPoint.Y = this.Y;
            aPoint.Z = this.Z;
            return aPoint;
        }

        public override string ToString()
        {
            return "{" + this.X + "," + this.Y + "," + this.Z + "}";
        }

        public string ToString(string Format)
        {
            return "{" + this.X.ToString(Format) + "," + this.Y.ToString(Format) + "," + this.Z.ToString(Format) + "}";
        }

        public override PointD[] Boundary
        {
            get { return null; }
        }

        public override void PlotInOpenGL()
        {
            if (this.Visible == true && this.OpenGLDisplayFunction != null)
            {
                this.OpenGLDisplayFunction(this);
            }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            Vector newPoint = TransformPoint(TransformMatrix, this);
            this.X = newPoint.X;
            this.Y = newPoint.Y;
            this.Z = newPoint.Z;
        }

        public double[] ToArray()
        {
            return new double[] { this.X, this.Y, this.Z };
        }

        public virtual PointD ToPointD()
        {
            return new PointD(this);
        }

        public netDxf.Vector3 ToDXFVertex3()
        {
            return new netDxf.Vector3(this.X, this.Y, this.Z);
        }
    }

    public class PointD : Vector, IHomogeneous
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
        #endregion

        public override PointD[] Boundary
        {
            get { return new PointD[] { this.Clone() as PointD, this.Clone() as PointD }; }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            PointD newPoint = TransformPoint(TransformMatrix, this);
            this.X = newPoint.X;
            this.Y = newPoint.Y;
            this.Z = newPoint.Z;
        }

        public virtual Vector ToVector()
        {
            return new Vector(this);
        }
    }
}
