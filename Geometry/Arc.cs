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
using static System.Math;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry
{
    public class Arc : LineArchitectureEntity, ICanBeWritedToDXFFile, IScriptFile, IToDXFEntity
    {
        //欄位，作為圓弧的實際定義
        public PointD Center;
        public double Radius, StartAng, EndAng;

        public PointD StartPoint
        {
            get { return Center + new PointD(Radius * Cos(StartAng), Radius * Sin(StartAng), 0); }
        }
        public PointD EndPoint
        {
            get { return Center + new PointD(Radius * Cos(EndAng), Radius * Sin(EndAng), 0); }
        }
        public PointD MidPoint
        {
            get { return Center + new PointD(Radius * Cos((StartAng + EndAng) / 2.0), Radius * Sin((StartAng + EndAng) / 2.0), 0); }
        }

        #region Constructor and Destructor
        public Arc(PointD tcenter, double tradius)
        {
            Center = (PointD)tcenter.Clone();
            Radius = tradius;
        }
        public Arc(PointD tcenter, double tradius, double tStartAng, double tEndAng, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            Center = (PointD)tcenter.Clone();
            Radius = tradius;
            StartAng = tStartAng;
            EndAng = tEndAng;
        }
        public Arc()
        {
        }
        #endregion

        #region DXFEntity
        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = externalCoordinateMatrix;
            if (this.CoordinateSystem != null)
                M = Dot(M, this.CoordinateSystem);

            XYZ4[] boundary;
            PointD startPoint;
            PointD endPoint;
            startPoint = Center + new PointD(Radius * Cos(StartAng), Radius * Sin(StartAng), 0);
            endPoint = Center + new PointD(Radius * Cos(EndAng), Radius * Sin(EndAng), 0);
            startPoint = Transport4(M, startPoint);
            endPoint = Transport4(M, endPoint);
            boundary = new XYZ4[2] { (PointD)startPoint.Clone(), (PointD)startPoint.Clone() };
            Compare_Boundary(boundary, startPoint);
            Compare_Boundary(boundary, endPoint);
            if (Abs(StartAng - EndAng) > PI / 4.0)
            {
                if (StartAng > EndAng)
                {
                    double Angle1;
                    if (StartAng > 0) { Angle1 = StartAng - (StartAng % (PI / 4.0)); }
                    else { Angle1 = StartAng - (-PI / 4.0 - StartAng % (PI / 4.0)); }
                    while (Angle1 < StartAng && Angle1 > EndAng)
                    {
                        Compare_Boundary(boundary, Center + new PointD(Radius * Cos(Angle1), Radius * Sin(Angle1), 0));
                        Angle1 -= PI / 4.0;
                    }
                }
                if (StartAng < EndAng)
                {
                    double Angle1;
                    if (EndAng > 0) { Angle1 = EndAng - (EndAng % (PI / 4.0)); }
                    else { Angle1 = EndAng - (-PI / 4.0 - EndAng % (PI / 4.0)); }
                    while (Angle1 < EndAng && Angle1 > StartAng)
                    {
                        Compare_Boundary(boundary, Center + new PointD(Radius * Cos(Angle1), Radius * Sin(Angle1), 0));
                        Angle1 -= PI / 4.0;
                    }
                }
            }
            return boundary;
        }
        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            sw.WriteLine("0");
            sw.WriteLine("ARC");
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
            if (this.StartAng < this.EndAng)
            {
                sw.WriteLine("50");
                sw.WriteLine(RadToDeg(this.StartAng).ToString());//起始角
                sw.WriteLine("51");
                sw.WriteLine(RadToDeg(this.EndAng).ToString());//終止角
            }
            else
            {
                sw.WriteLine("50");
                sw.WriteLine(RadToDeg(this.EndAng).ToString());//起始角
                sw.WriteLine("51");
                sw.WriteLine(RadToDeg(this.StartAng).ToString());//終止角
            }
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            sw.WriteLine("-COLOR T {0},{1},{2}", this.Color.R, this.Color.G, this.Color.B);
            sw.WriteLine("-LWEIGHT {0}", this.LineWidth / 8.0);
            sw.WriteLine("ARC");
            //sw.WriteLine(aArc.Center.X.ToString(demicalFormat) + "," + aArc.Center.Y.ToString(demicalFormat));
            sw.WriteLine(this.StartPoint.X.ToString() + "," + this.StartPoint.Y.ToString());
            sw.WriteLine(this.MidPoint.X.ToString() + "," + this.MidPoint.Y.ToString());
            sw.WriteLine(this.EndPoint.X.ToString() + "," + this.EndPoint.Y.ToString());
        }
        public override object Clone()
        {
            Arc aArc = new Arc();
            if (this.Name != null)
                aArc.Name = this.Name;
            aArc.Color = this._color;
            aArc.LineType = this.LineType;
            aArc.LineWidth = this.LineWidth;
            aArc.LineTypefactor = this.LineTypefactor;

            aArc.Center = (PointD)this.Center.Clone();
            aArc.Radius = this.Radius;
            aArc.StartAng = this.StartAng;
            aArc.EndAng = this.EndAng;
            return aArc;
        }
        #endregion

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

                PlotArc(this.Center, this.Radius, this.StartAng, this.EndAng);

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.Center.Transform(TransformMatrix);
            double dAngle = Atan2(TransformMatrix[1, 3], TransformMatrix[0, 3]);
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            netDxf.Entities.Arc dxfArc = new netDxf.Entities.Arc()
            {
                Center = this.Center.ToDXFVertex3(),
                Radius = this.Radius,
                Color = this._color.A != 0 ? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
                LineType = DXFLineTypeConverter.Convert(this.LineType)
            };

            if (this.StartAng < this.EndAng)
            {
                dxfArc.StartAngle = RadToDeg(this.StartAng);
                dxfArc.EndAngle = RadToDeg(this.EndAng);
            }
            else
            {
                dxfArc.StartAngle = RadToDeg(this.EndAng);
                dxfArc.EndAngle = RadToDeg(this.StartAng);
            }
            return dxfArc;
        }
    }
}
