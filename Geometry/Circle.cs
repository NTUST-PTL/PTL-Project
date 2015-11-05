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
    public class Circle : LineArchitectureEntity, ICanBeWritedToDXFFile, IScriptFile, IToDXFEntity
    {
        public PointD Center;
        public double Radius;

        #region Constructor and Destructor
        public Circle(PointD tcenter, double tradius)
        {
            Center = (PointD)tcenter.Clone();
            Radius = tradius;
        }
        public Circle(PointD tcenter, double tradius, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            this.Center = (PointD)tcenter.Clone();
            this.Radius = tradius;
        }
        public Circle()
        {
        }
        #endregion

        #region DXFEntity
        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                if (this.CoordinateSystem != null)
                {
                    boundary = new XYZ4[2] { Transport4(this.CoordinateSystem, Center + new PointD(Radius, 0, 0)), Transport4(this.CoordinateSystem, Center + new PointD(Radius, 0, 0)) };
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Center + new PointD(-Radius, 0, 0)));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Center + new PointD(0, Radius, 0)));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Center + new PointD(0, -Radius, 0)));
                }
                else
                {
                    boundary = new XYZ4[2] { Center + new PointD(Radius, 0, 0), Center + new PointD(Radius, 0, 0) };
                    Compare_Boundary(boundary, Center + new PointD(-Radius, 0, 0));
                    Compare_Boundary(boundary, Center + new PointD(0, Radius, 0));
                    Compare_Boundary(boundary, Center + new PointD(0, -Radius, 0));
                }
                return boundary;
            }
        }
        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            sw.WriteLine("0");
            sw.WriteLine("CIRCLE");
            sw.WriteLine("8");
            sw.WriteLine(this.Parent.Name);//圖層名稱
            sw.WriteLine("62");
            sw.WriteLine(Part.SystemColor2DXFColorIndex(this.Color));//顏色號碼
            sw.WriteLine("10");
            sw.WriteLine(this.Center.X.ToString());//中心X座標
            sw.WriteLine("20");
            sw.WriteLine(this.Center.Y.ToString());//中心Y座標
            sw.WriteLine("30");
            sw.WriteLine(this.Center.Z.ToString());//中心Z座標
            sw.WriteLine("40");
            sw.WriteLine(this.Radius.ToString());//半徑
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            sw.WriteLine("-COLOR T {0},{1},{2}", this.Color.R, this.Color.G, this.Color.B);
            sw.WriteLine("-LWEIGHT {0}", this.LineWidth / 8.0);
            sw.WriteLine("CIRCLE");
            sw.WriteLine(this.Center.X.ToString() + "," + this.Center.Y.ToString());
            sw.WriteLine(this.Radius.ToString());
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

                int nump = 360;
                double step = 2.0 * PI / nump;
                double thetaz;
                PointD pnt;

                GL.glBegin(GL.GL_LINE_LOOP);
                for (int i = 0; i < nump; i++)
                {
                    thetaz = step * i;
                    pnt = Center + new PointD(Radius * Cos(thetaz), Radius * Sin(thetaz), 0.0);
                    Vertex3d(pnt);
                }
                GL.glEnd();

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }
        public override object Clone()
        {
            Circle aCircle = new Circle();
            if (this.Name != null)
                aCircle.Name = this.Name;
            aCircle.Color = this._color;
            aCircle.LineType = this.LineType;
            aCircle.LineWidth = this.LineWidth;
            aCircle.LineTypefactor = this.LineTypefactor;

            aCircle.Center = (PointD)this.Center.Clone();
            aCircle.Radius = this.Radius;
            return aCircle;
        }
        #endregion

        public override void Transform(double[,] TransformMatrix)
        {
            this.Center.Transform(TransformMatrix);
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            return new netDxf.Entities.Circle()
            {
                Center = this.Center.ToDXFVertex3(),
                Radius = this.Radius,
                Color = this._color.A != 0 ? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
                LineType = DXFLineTypeConverter.Convert(this.LineType)
            };
        }
    }
}
