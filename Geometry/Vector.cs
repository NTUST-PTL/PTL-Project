using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected double openGLDisplaySize = 10;
        public double OpenGLDisplaySize
        {
            get { return openGLDisplaySize; }
            set { this.openGLDisplaySize = value; }
        }
        public Action<Vector> OpenGLDisplayFunction = PlotAsPoint;

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
        public static implicit operator XYZ3(Vector v)
        {
            return new XYZ3(v.Value);
        }
        public static implicit operator Vector(XYZ3 v)
        {
            return new Vector(v.Values);
        }
        public static implicit operator Vector(double[] p)
        {
            return new Vector(p);
        }
        public static implicit operator double[] (Vector p)
        {
            return p.Value;
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

        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            return null;
        }

        public static void PlotAsPoint(Vector v)
        {
            glColor4d(v.Color);
            GL.glPointSize(Convert.ToSingle(v.openGLDisplaySize));
            GL.glBegin(GL.GL_POINTS);
            GL.glVertex3d(v.X, v.Y, v.Z);
            GL.glEnd();
        }

        public static void PlotAsShpere(Vector v)
        {
            glColor4d(v.Color);
            GL.glPushMatrix();
            Translated(v);
            GL.glutSolidSphere(v.OpenGLDisplaySize, 20, 20);
            GL.glPopMatrix();
        }

        public static void PlotAsCube(Vector v)
        {
            glColor4d(v.Color);
            GL.glPushMatrix();
            Translated(v);
            GL.glutSolidCube(v.OpenGLDisplaySize);
            GL.glPopMatrix();
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
            Vector newPoint = Transport3(TransformMatrix, this);
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
}
