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
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Geometry
{
    public class PolyLine : LineArchitectureEntity, ICanBeWritedToDXFFile, IScriptFile, IToDXFEntity
    {
        public List<XYZ4> Points = new List<XYZ4>();

        #region Constructor and Destructor
        public PolyLine(Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;
        }
        public PolyLine(int PointNum)
        {
            for (int i = 0; i < PointNum; i++)
                Points.Add(new PointD());
        }
        public PolyLine(PointD[] tpoints)
        {
            int nump = tpoints.Length;
            for (int i = 0; i < nump; i++)
                Points.Add((PointD)tpoints[i].Clone());
        }
        public PolyLine(MathModel.XYZ4[] tpoints)
        {
            int nump = tpoints.Length;
            for (int i = 0; i < nump; i++)
                Points.Add(new PointD(tpoints[i]));
        }
        public PolyLine(PointD[] tpoints, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            int nump = tpoints.Length;
            for (int i = 0; i < nump; i++)
                Points.Add((PointD)tpoints[i].Clone());
        }
        public PolyLine()
        {
        }
        #endregion

        public void AddPoint(XYZ4 tpoint)
        {
            Points.Add((XYZ4)tpoint.Clone());
        }

        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = externalCoordinateMatrix;
            if (this.CoordinateSystem != null)
                M = Dot(M, this.CoordinateSystem);

            XYZ4[] boundary = null;
            if (Points.Count > 0)
            {
                boundary = new XYZ4[2] { Transport4(M, Points[0]), Transport4(M, Points[0]) };
                foreach (PointD p in this.Points)
                    Compare_Boundary(boundary, Transport4(M, p));
            }
            return boundary;
        }
        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            sw.WriteLine("0");
            sw.WriteLine("POLYLINE");
            sw.WriteLine("8");
            sw.WriteLine(this.Parent.Name);//圖層名稱
            sw.WriteLine("66");//图元跟随
            sw.WriteLine("1");
            sw.WriteLine("62");
            sw.WriteLine(Part.SystemColor2DXFColorIndex(this.Color));//顏色號碼
            sw.WriteLine("10");
            sw.WriteLine("0");
            sw.WriteLine("20");
            sw.WriteLine("0");
            sw.WriteLine("70");
            sw.WriteLine("2");//聚合線旗標；
            for (int i = 0; i < this.Points.Count; i++)
            {
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("8");
                sw.WriteLine(this.Parent.Name);//圖層名稱
                sw.WriteLine("10");
                sw.WriteLine(this.Points[i].X.ToString());//第一點X座標
                sw.WriteLine("20");
                sw.WriteLine(this.Points[i].Y.ToString());//第一點Y座標
            }
            sw.WriteLine("0");
            sw.WriteLine("SEQEND");
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            sw.WriteLine("-COLOR T {0},{1},{2}", this.Color.R, this.Color.G, this.Color.B);
            sw.WriteLine("-LWEIGHT {0}", this.LineWidth / 8.0);
            sw.WriteLine("PLINE");
            foreach (PointD p in this.Points)
            {
                sw.WriteLine(p.X.ToString() + "," + p.Y.ToString());
            }
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
                glColor3d(this.Color);

                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                for (int k = 0; k < this.Points.Count; k++)
                    Vertex3d(this.Points[k]);
                GL.glEnd();
                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override object Clone()
        {
            PolyLine aPolyLine = new PolyLine();
            if (this.Name != null)
                aPolyLine.Name = this.Name;
            aPolyLine.Color = this._color;
            aPolyLine.LineType = this.LineType;
            aPolyLine.LineWidth = this.LineWidth;
            aPolyLine.LineTypefactor = this.LineTypefactor;

            int nump = this.Points.Count;
            aPolyLine.Points = new List<XYZ4>();
            for (int i = 0; i < nump; i++)
                aPolyLine.Points.Add(this.Points[i].Clone() as XYZ4);
            return aPolyLine;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.Points = Transport(TransformMatrix, this.Points.ToArray()).ToList();
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            List<netDxf.Entities.PolylineVertex> ps = new List<netDxf.Entities.PolylineVertex>();
            foreach (var p in Points)
                ps.Add(new netDxf.Entities.PolylineVertex(((PointD)p).ToDXFVertex3()));
            netDxf.Entities.Polyline pl = new netDxf.Entities.Polyline()
            {
                Vertexes = ps,
                Color = this._color.A != 0 ? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
                LineType = DXFLineTypeConverter.Convert(this.LineType)
            };

            return pl;
        }
    }
}
