using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using CsGL.OpenGL;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class RadialDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, IScriptFile, IToDXFEntity
    {
        public double Value { get { return Norm(Startpn - Endpn); } }
        public PointD Startpn = new PointD();
        public PointD Endpn = new PointD();
        public PointD TextReferP = new PointD();
        public Double LeaderLength;
        public string Text = "<>";
        public double ArrowSize = 10.0;
        Color color = Color.LawnGreen;
        public override Color Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                }
            }
        }

        #region Constructor and Destructor
        public RadialDimension(Circle aCircle, double refDirection, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = (PointD)aCircle.Center.Clone();
            Endpn = aCircle.Center + V1;
            TextReferP = (Startpn + Endpn) / 2;
            LeaderLength = Norm((this.Startpn - this.TextReferP)) - Value;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public RadialDimension(Circle aCircle, double refDirection, PointD tTextReferpn, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = (PointD)aCircle.Center.Clone();
            Endpn = aCircle.Center + V1;
            TextReferP = (PointD)tTextReferpn.Clone();
            LeaderLength = Norm((this.Startpn - this.TextReferP)) - Value;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public RadialDimension(Circle aCircle, double refDirection, PointD tTextReferpn, string tText, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = (PointD)aCircle.Center.Clone();
            Endpn = aCircle.Center + V1;
            TextReferP = (PointD)tTextReferpn.Clone();
            LeaderLength = Norm((this.Startpn - this.TextReferP)) - Value;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
            this.Text = tText;
        }
        public RadialDimension()
        {
        }
        #endregion

        #region Method
        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                if (this.CoordinateSystem != null)
                {
                    boundary = new XYZ4[2] { Transport4(this.CoordinateSystem, this.Startpn), Transport4(this.CoordinateSystem, this.Startpn) };
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Endpn));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, TextReferP));
                }
                else
                {
                    boundary = new XYZ4[2] { (PointD)this.Startpn.Clone(), (PointD)this.Startpn.Clone() };
                    Compare_Boundary(boundary, Endpn);
                    Compare_Boundary(boundary, TextReferP);
                }
                return boundary;
            }
        }
        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            sw.WriteLine("0");
            sw.WriteLine("DIMENSION");
            //sw.WriteLine("5");
            //sw.WriteLine("88");//
            sw.WriteLine("8");
            sw.WriteLine(this.Parent.Name);//圖層名稱
            sw.WriteLine("2");
            sw.WriteLine("*D0");//包含標註圖片組成圖元的圖塊名稱
            sw.WriteLine("70");
            sw.WriteLine("4");//標註種類
            sw.WriteLine("1");//使用者明確輸入的標註文字
            sw.WriteLine(" ");//抑制文字
            //sw.WriteLine(this.Value.ToString("N3"));//抑制文字
            sw.WriteLine("62");
            sw.WriteLine(Part.SystemColor2DXFColorIndex(this.Color));//顏色號碼
            sw.WriteLine("10");
            sw.WriteLine(this.Startpn.X.ToString());//定義點X座標
            sw.WriteLine("20");
            sw.WriteLine(this.Startpn.Y.ToString());//定義點Y座標
            sw.WriteLine("30");
            sw.WriteLine(this.Startpn.Z.ToString());//定義點Z座標
            sw.WriteLine("11");
            sw.WriteLine(this.TextReferP.X.ToString());//標註文字的中點X座標
            sw.WriteLine("21");
            sw.WriteLine(this.TextReferP.Y.ToString());//標註文字的中點Y座標
            sw.WriteLine("31");
            sw.WriteLine(this.TextReferP.Z.ToString());//標註文字的中點Z座標
            sw.WriteLine("15");
            sw.WriteLine(this.Endpn.X.ToString());//直徑標註、半徑標註及角度標註的定義點X座標
            sw.WriteLine("25");
            sw.WriteLine(this.Endpn.Y.ToString());//直徑標註、半徑標註及角度標註的定義點Y座標
            sw.WriteLine("35");
            sw.WriteLine(this.Endpn.Z.ToString());//直徑標註、半徑標註及角度標註的定義點Z座標
            sw.WriteLine("40");
            sw.WriteLine(this.LeaderLength.ToString());//Leader length for radius and diameter dimensions

            Text valueText = new Geometry.Text();
            valueText.Value = this.Value.ToString("N3");
            valueText.RefPoint = this.TextReferP;
            PointD v1 = this.Endpn - this.Startpn;
            valueText.Orietation = (int)RadToDeg(Atan2(v1.Y, v1.X));
            if (valueText.Orietation > PI / 2.0)
                valueText.Orietation -= PI;
            else if (valueText.Orietation < -PI / 2.0)
                valueText.Orietation += PI;
            valueText.Color = this.Color;
            valueText.Parent = new Layer(this.Parent.Name);
            valueText.TextHieght = this.ArrowSize / 8;
            valueText.WriteToFileInDxfFormat(sw);
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            throw new NotImplementedException();
        }
        public override object Clone()
        {
            RadialDimension aAligD = new RadialDimension();
            if (this.Name != null)
                aAligD.Name = this.Name;
            aAligD.Startpn = (PointD)this.Startpn.Clone();
            aAligD.Endpn = (PointD)this.Endpn.Clone();
            aAligD.TextReferP = (PointD)this.TextReferP.Clone();
            aAligD.Color = this.color;
            aAligD.LeaderLength = this.LeaderLength;

            aAligD.Text = this.Text;
            aAligD.ArrowSize = this.ArrowSize;
            return aAligD;
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

                PointD v1 = this.Endpn - this.Startpn;

                double dist = Round(Norm(this.Endpn - this.Startpn), 0.001);
                double ExtenDist1 = 0.05 * this.ArrowSize;
                double ExtenDist2 = 0.05 * this.ArrowSize;

                //Plot Text
                WriteString(dist.ToString(), this.TextReferP, this.Color);

                //Dimension Line
                PointD p1;
                PointD p2;
                p1 = this.Startpn;
                p2 = this.Startpn + v1 * ((Norm(v1) + this.LeaderLength) / Norm(v1));
                Line tLine = new Line(p1, p2, this.Color, 1.0f);
                tLine.PlotInOpenGL();

                //Plot Arrow
                double theta = RadToDeg(Atan2(v1.Y, v1.X));

                //Arrow
                p1 = new PointD(0, 0, 0);
                p2 = new PointD(0.15, -0.03, 0) * this.ArrowSize;
                PointD p3 = new PointD(0.15, 0.03, 0) * this.ArrowSize;

                PointD p4 = this.Endpn;
                if (Norm(Endpn - Startpn) > Norm(TextReferP - Startpn))
                    PlotTriangle(p1, p2, p3, p4, theta + 180, this.Color);
                else
                    PlotTriangle(p1, p2, p3, p4, theta, this.Color);

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.Startpn.Transform(TransformMatrix);
            this.Endpn.Transform(TransformMatrix);
            this.TextReferP.Transform(TransformMatrix);
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            return new netDxf.Entities.RadialDimension(
                this.Startpn.ToDXFVertex3(),
                this.Endpn.ToDXFVertex3(),
                this.LeaderLength)
            {
                Color = this._color.A != 0 ? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
            };
        }
    }
}
