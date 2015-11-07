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
    public class Line : LineArchitectureEntity, ICanBeWritedToDXFFile, IScriptFile, IToDXFEntity
    {
        public PointD p1;
        public PointD p2;

        #region Constructor and Destructor
        public Line(PointD tstartpnt, PointD tendpnt)
        {
            this.p1 = (PointD)tstartpnt.Clone();
            this.p2 = (PointD)tendpnt.Clone();
        }
        public Line(PointD tstartpnt, PointD tendpnt, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            this.p1 = (PointD)tstartpnt.Clone();
            this.p2 = (PointD)tendpnt.Clone();
        }
        public Line(PointD tstartpnt, PointD tendpnt, Color tcolor, float twidth)
        {
            this.Color = tcolor;
            this.LineWidth = twidth;

            this.p1 = (PointD)tstartpnt.Clone();
            this.p2 = (PointD)tendpnt.Clone();
        }
        public Line(Line tline, Color tcolor, LineType ttype, float twidth)
        {
            if (tline.Name != null)
                Name = tline.Name;
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            this.p1 = (PointD)tline.p1.Clone();
            this.p2 = (PointD)tline.p2.Clone();
        }
        public Line()
        {
        }
        #endregion

        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = externalCoordinateMatrix;
            if (this.CoordinateSystem != null)
                M = MatrixDot(M, this.CoordinateSystem);

            XYZ4[] boundary;
            boundary = new XYZ4[2] { Transport<XYZ4>(M, this.p1), Transport<XYZ4>(M, this.p1) };
            Compare_Boundary(boundary, Transport<XYZ4>(M, this.p2));
            return boundary;
        }
        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            sw.WriteLine("0");
            sw.WriteLine("LINE");
            sw.WriteLine("8");
            sw.WriteLine(this.Parent.Name);//圖層名稱
            sw.WriteLine("62");
            sw.WriteLine(Part.SystemColor2DXFColorIndex(this.Color));//顏色號碼
            sw.WriteLine("10");
            sw.WriteLine(this.p1.X.ToString());//第一點X座標
            sw.WriteLine("20");
            sw.WriteLine(this.p1.Y.ToString());//第一點Y座標
            sw.WriteLine("30");
            sw.WriteLine(this.p1.Z.ToString());//第一點Z座標
            sw.WriteLine("11");
            sw.WriteLine(this.p2.X.ToString());//第二點X座標
            sw.WriteLine("21");
            sw.WriteLine(this.p2.Y.ToString());//第二點Y座標
            sw.WriteLine("31");
            sw.WriteLine(this.p2.Z.ToString());//第二點Z座標
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            sw.WriteLine("-COLOR T {0},{1},{2}", this.Color.R, this.Color.G, this.Color.B);
            sw.WriteLine("-LWEIGHT {0}", this.LineWidth / 8.0);
            sw.WriteLine("Line");
            sw.WriteLine(this.p1.X.ToString() + "," + this.p1.Y.ToString());
            sw.WriteLine(this.p2.X.ToString() + "," + this.p2.Y.ToString());
            sw.WriteLine("");
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
                SetLineType(this.LineType, LineTypefactor);
                glColor4d(this.Color);

                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                Vertex3d(this.p1);
                Vertex3d(this.p2);
                GL.glEnd();
                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override object Clone()
        {
            Line aLine = new Line();
            if (this.Name != null)
                aLine.Name = this.Name;
            aLine.Color = this._color;
            aLine.LineType = this.LineType;
            aLine.LineWidth = this.LineWidth;
            aLine.LineTypefactor = this.LineTypefactor;

            aLine.p1 = (PointD)this.p1.Clone();
            aLine.p2 = (PointD)this.p2.Clone();
            return aLine;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.p1 = Transport4(TransformMatrix, this.p1);
            this.p2 = Transport4(TransformMatrix, this.p2);
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            return new netDxf.Entities.Line()
            {
                StartPoint = this.p1.ToDXFVertex3(),
                EndPoint = this.p2.ToDXFVertex3(),
                Color = this._color.A != 0 ? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
                LineType = DXFLineTypeConverter.Convert(this.LineType)
            };
        }
    }
}
