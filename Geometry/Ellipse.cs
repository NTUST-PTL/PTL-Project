using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using CsGL.OpenGL;
using PTL.Definitions;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class Ellipse : LineArchitectureEntity, ICanBeWritedToDXFFile, IScriptFile
    {
        //欄位，作為橢圓的實際定義
        private PointD fCenter;
        private Vector fEndPointOfMajorAxis;//Relative To Center
        private Vector fNormal;
        private double fRatio;
        private double fStartAngle = 0, fEndAngle = 2 * PI;

        //屬性，對應欄位
        public PointD Center
        {
            get { return fCenter; }
            set { this.fCenter = value; }
        }
        public Vector EndPointOfMajorAxis
        {
            get { return this.fEndPointOfMajorAxis; }
            set { this.fEndPointOfMajorAxis = value; }
        }
        public Vector Normal
        {
            get { return this.fNormal; }
            private set { this.fNormal = value; }
        }
        public double Ratio
        {
            get { return this.fRatio; }
            private set { this.fRatio = value; }
        }
        public double StartAngle
        { get { return this.fStartAngle; } private set { this.fStartAngle = value; } }
        public double EndAngle
        { get { return this.fEndAngle; } private set { this.fEndAngle = value; } }

        //屬性，作為橢圓外在的特徵
        public Vector MajorDirection
        {
            get { return Normalize(fEndPointOfMajorAxis); }
            set
            {
                double Ra = Norm(fEndPointOfMajorAxis);
                this.fEndPointOfMajorAxis = Ra * (Vector)Normalize(value);
            }
        }
        public Vector MinorDirection
        {
            get { return Normalize(Cross(fNormal, fEndPointOfMajorAxis)); }
            set
            {
                this.fNormal = Normalize(Cross(fEndPointOfMajorAxis, value));
            }
        }
        public double Ra
        { get { return Norm(fEndPointOfMajorAxis); } set { this.fEndPointOfMajorAxis = value * (Vector)Normalize(fEndPointOfMajorAxis); } }
        public double Rb
        { get { return fRatio * Norm(fEndPointOfMajorAxis); } set { this.fRatio = value / Norm(fEndPointOfMajorAxis); } }


        #region Constructor
        public Ellipse(PointD center, Vector majorDirection, Vector minorDirection, Double ra, double rb)
        {
            this.fCenter = center;
            this.fEndPointOfMajorAxis = (new Vector(Normalize(majorDirection))) * Ra;
            this.fNormal = Cross(majorDirection, minorDirection);
            this.fRatio = rb / ra;
        }
        public Ellipse(PointD center, Vector majorDirection, Vector minorDirection, Double ra, double rb, Color color, LineType lineType, float width)
        {
            this.fCenter = center;
            this.fEndPointOfMajorAxis = (Vector)Normalize(majorDirection) * ra;
            this.fNormal = Cross(majorDirection, minorDirection);
            this.fRatio = rb / ra;
            this.Color = color;
            this.LineType = lineType;
            this.LineWidth = width;
        }
        public Ellipse(PointD center, Vector endPointOfMajorAxis, Vector normal, double ratio, double startAngle, double endAngle)
        {
            this.fCenter = center;
            this.fEndPointOfMajorAxis = endPointOfMajorAxis;
            this.fNormal = normal;
            this.fRatio = ratio;
            this.fStartAngle = startAngle;
            this.fEndAngle = endAngle;
        }
        public Ellipse(PointD center, Vector endPointOfMajorAxis, Vector normal, double ratio)
        {
            this.fCenter = center;
            this.fEndPointOfMajorAxis = endPointOfMajorAxis;
            this.fNormal = normal;
            this.fRatio = ratio;
        }
        public Ellipse()
        {

        }
        #endregion

        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = externalCoordinateMatrix;
            if (this.CoordinateSystem != null)
                M = MatrixDot(M, this.CoordinateSystem);

            XYZ4[] boundary;
            boundary = new XYZ4[2] { Transport4(M, Center + fEndPointOfMajorAxis), Transport4(M, Center + fEndPointOfMajorAxis) };
            Compare_Boundary(boundary, Transport4(M, Center - fEndPointOfMajorAxis));
            Compare_Boundary(boundary, Transport4(M, Center + MinorDirection * Rb));
            Compare_Boundary(boundary, Transport4(M, Center - MinorDirection * Rb));
            return boundary;
        }

        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            throw new NotImplementedException();
        }

        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            Ellipse aEllipse = new Ellipse(this.fCenter, this.fEndPointOfMajorAxis, this.fNormal, this.fRatio, this.fStartAngle, this.fEndAngle);
            aEllipse.Color = this._color;
            aEllipse.LineType = this.LineType;
            aEllipse.LineWidth = this.LineWidth;
            aEllipse.LineTypefactor = this.LineTypefactor;
            return aEllipse;
        }

        public override void PlotInOpenGL()
        {
            if (this.Visible == true)
            {
                if (this.CoordinateSystem != null)
                {
                    GL.glPushMatrix();
                    GL.glMatrixMode(GL.GL_MODELVIEW);
                    MultMatrixd(this.CoordinateSystem);
                }
                //Line Style
                glLineWidth(this.LineWidth);
                SetLineType(this.LineType, this.LineTypefactor);
                glColor3d(this.Color);

                int nump = 21;
                double step = 2 * PI / (nump - 1);

                PointD center = this.Center;
                Vector e1 = (Vector)this.MajorDirection.Clone();
                Vector e2 = (Vector)this.MinorDirection.Clone();
                double ra = this.Ra;
                double rb = this.Rb;

                GL.glBegin(GL.GL_LINE_STRIP);
                for (int k = 0; k < nump; k++)
                {
                    double theta = step * k;
                    PointD pnt = this.Center + ra * Cos(theta) * e1 + rb * Sin(theta) * e2;
                    Vertex3d(pnt);
                }
                GL.glEnd();

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.fCenter.Transform(TransformMatrix);
            this.fEndPointOfMajorAxis.Transform(TransformMatrix);
            this.fNormal.Transform(TransformMatrix);
        }
    }
}
