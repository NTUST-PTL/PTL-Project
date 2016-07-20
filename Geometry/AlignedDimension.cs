using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using CsGL.OpenGL;
using PTL.Geometry.MathModel;
using static System.Math;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry
{
    public class AlignedDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, IScriptFile, IToDXFEntity
    {
        public double Value { get { return Norm(Startpn - Endpn); } }
        public PointD Startpn = new PointD();
        public PointD Endpn = new PointD();
        public PointD HieghtReferP = new PointD();
        public PointD TextReferP = new PointD();

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
        public AlignedDimension(PointD tStartpn, PointD tEndpn, PointD tTextReferpn, string tText, double tArrowSize)
        {
            Startpn = (PointD)tStartpn.Clone();
            Endpn = (PointD)tEndpn.Clone();

            TextReferP = (PointD)tTextReferpn.Clone();
            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            double refheight = Dot(v1, tTextReferpn - Endpn);
            HieghtReferP = Endpn + v1 * refheight;

            this.Text = tText;
            ArrowSize = tArrowSize;
        }
        public AlignedDimension(PointD tStartpn, PointD tEndpn, PointD tTextReferpn, double tArrowSize)
        {
            Startpn = (PointD)tStartpn.Clone();
            Endpn = (PointD)tEndpn.Clone();
            TextReferP = (PointD)tTextReferpn.Clone();

            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            double refheight = Abs(Dot(v1, tTextReferpn - Endpn));
            HieghtReferP = Endpn + v1 * refheight;

            ArrowSize = tArrowSize;
        }
        public AlignedDimension(PointD tStartpn, PointD tEndpn, double refheight, double tArrowSize)
        {
            Startpn = (PointD)tStartpn.Clone();
            Endpn = (PointD)tEndpn.Clone();

            ArrowSize = tArrowSize;

            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            TextReferP = (Startpn + Endpn) / 2 + v1 * refheight;
            HieghtReferP = Endpn + v1 * refheight;
        }
        public AlignedDimension(PointD tStartpn, PointD tEndpn)
        {
            Startpn = (PointD)tStartpn.Clone();
            Endpn = (PointD)tEndpn.Clone();

            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            TextReferP = (Startpn + Endpn) / 2 + v1 * ArrowSize * 0.5;
            HieghtReferP = Endpn + v1 * ArrowSize * 0.5;
        }
        public AlignedDimension()
        {
        }
        #endregion

        #region DXFEntity
        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = Dot(externalCoordinateMatrix, this.CoordinateSystem);

            XYZ4[] boundary;
            boundary = new XYZ4[2] { Transport4(M, this.Startpn), Transport4(M, this.Startpn) };
            Compare_Boundary(boundary, Transport4(M, Endpn));
            Compare_Boundary(boundary, Transport4(M, HieghtReferP));
            Compare_Boundary(boundary, Transport4(M, TextReferP));
            Compare_Boundary(boundary, Transport4(M, HieghtReferP + (Startpn - Endpn)));

            return boundary;
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
            sw.WriteLine("1");
            sw.WriteLine("1");//使用者明確輸入的標註文字
            sw.WriteLine(" ");//抑制文字
            sw.WriteLine("62");
            sw.WriteLine(Part.SystemColor2DXFColorIndex(this.Color));//顏色號碼
            sw.WriteLine("10");
            sw.WriteLine(this.HieghtReferP.X.ToString());//定義點X座標
            sw.WriteLine("20");
            sw.WriteLine(this.HieghtReferP.Y.ToString());//定義點Y座標
            sw.WriteLine("30");
            sw.WriteLine(this.HieghtReferP.Z.ToString());//定義點Z座標
            sw.WriteLine("11");
            sw.WriteLine(this.TextReferP.X.ToString());//標註文字的中點X座標
            sw.WriteLine("21");
            sw.WriteLine(this.TextReferP.Y.ToString());//標註文字的中點Y座標
            sw.WriteLine("31");
            sw.WriteLine(this.TextReferP.Z.ToString());//標註文字的中點Z座標
            sw.WriteLine("13");
            sw.WriteLine(this.Startpn.X.ToString());//線性標註與角度標註的定義點X座標
            sw.WriteLine("23");
            sw.WriteLine(this.Startpn.Y.ToString());//線性標註與角度標註的定義點Y座標
            sw.WriteLine("33");
            sw.WriteLine(this.Startpn.Z.ToString());//線性標註與角度標註的定義點Z座標
            sw.WriteLine("14");
            sw.WriteLine(this.Endpn.X.ToString());//線性標註與角度標註的定義點X座標
            sw.WriteLine("24");
            sw.WriteLine(this.Endpn.Y.ToString());//線性標註與角度標註的定義點Y座標
            sw.WriteLine("34");
            sw.WriteLine(this.Endpn.Z.ToString());//線性標註與角度標註的定義點Z座標

            Text valueText = new Geometry.Text();
            valueText.Value = this.Value.ToString("################.###");
            valueText.RefPoint = this.TextReferP;
            PointD v1 = this.Endpn - this.Startpn;
            valueText.Orietation = (int)RadToDeg(Atan2(v1.Y, v1.X));
            valueText.Color = this.Color;
            valueText.Parent = new Layer(this.Parent.Name);
            valueText.TextHieght = this.ArrowSize / 8;
            valueText.WriteToFileInDxfFormat(sw);
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            sw.WriteLine("-COLOR T {0},{1},{2}", this.Color.R, this.Color.G, this.Color.B);
            sw.WriteLine("-LWEIGHT {0}", 1.0 / 8.0);
            sw.WriteLine("DIMALIGNED");
            sw.WriteLine(this.Startpn.X.ToString() + "," + this.Startpn.Y.ToString());
            sw.WriteLine(this.Endpn.X.ToString() + "," + this.Endpn.Y.ToString());
            sw.WriteLine(this.TextReferP.X.ToString() + "," + this.TextReferP.Y.ToString());
        }
        public override object Clone()
        {
            AlignedDimension aAligD = new AlignedDimension();
            if (this.Name != null)
                aAligD.Name = this.Name;
            aAligD.Startpn = (PointD)this.Startpn.Clone();
            aAligD.Endpn = (PointD)this.Endpn.Clone();
            aAligD.TextReferP = (PointD)this.TextReferP.Clone();
            aAligD.HieghtReferP = (PointD)this.HieghtReferP.Clone();
            aAligD.Color = this.color;

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
                PointD v2 = this.HieghtReferP - this.Endpn;

                double dist = Round(Norm(this.Endpn - this.Startpn), 0.001);
                double RefDist = Norm(this.HieghtReferP - this.Endpn);
                double ExtenDist1 = 0.05 * this.ArrowSize;
                double ExtenDist2 = 0.05 * this.ArrowSize;

                //Plot Text
                WriteString(dist.ToString(), this.TextReferP, this.Color);

                //Dimension Line
                PointD p1;
                PointD p2;
                if (Dot(this.TextReferP - this.Startpn, v1) > Norm(v1) * Norm(v1))
                {
                    p1 = this.Startpn + v2;
                    p2 = this.TextReferP;
                }
                else if (Dot(this.TextReferP - this.Startpn, v1) < 0)
                {
                    p1 = this.TextReferP;
                    p2 = this.Endpn + v2;
                }
                else
                {
                    p1 = this.Startpn + v2;
                    p2 = this.Endpn + v2;
                }
                Line tLine = new Line(p1, p2, this.Color, 1.0f);
                tLine.Parent = new Layer();
                tLine.PlotInOpenGL();

                //Extension Line
                p1 = this.Startpn + ExtenDist1 * v2 / Norm(v2);
                p2 = this.Startpn + v2 + ExtenDist2 * v2 / Norm(v2);
                tLine = new Line(p1, p2, this.Color, 1.0f);
                tLine.PlotInOpenGL();


                p1 = this.Endpn + ExtenDist1 * v2 / Norm(v2);
                p2 = this.Endpn + v2 + ExtenDist2 * v2 / Norm(v2);
                tLine = new Line(p1, p2, this.Color, 1.0f);
                tLine.PlotInOpenGL();

                //量測點
                GL.glPushMatrix();
                Translated(this.Startpn);
                GL.glutSolidSphere(this.ArrowSize / 200.0, 20, 2);
                GL.glPopMatrix();
                GL.glPushMatrix();
                Translated(this.Endpn);
                GL.glutSolidSphere(this.ArrowSize / 200.0, 20, 2);
                GL.glPopMatrix();

                //Plot Arrow
                double theta = RadToDeg(Atan2(v1.Y, v1.X));

                //Arrow
                p1 = new PointD(0, 0, 0);
                p2 = new PointD(0.15, -0.03, 0) * this.ArrowSize;
                PointD p3 = new PointD(0.15, 0.03, 0) * this.ArrowSize;

                PointD p4 = this.Startpn + v2;
                if (this.Value < (p2.X - p1.X) * 5.0)
                {
                    PlotTriangle(p1, p2, p3, p4, theta + 180, this.Color);

                    p4 = this.Endpn + v2;
                    PlotTriangle(p1, p2, p3, p4, theta, this.Color);
                }
                else
                {
                    PlotTriangle(p1, p2, p3, p4, theta, this.Color);

                    p4 = this.Endpn + v2;
                    PlotTriangle(p1, p2, p3, p4, theta + 180, this.Color);
                }

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
            this.HieghtReferP.Transform(TransformMatrix);
            this.TextReferP.Transform(TransformMatrix);
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            //netDxf.Tables.DimensionStyle StandardDimStyle = new netDxf.Tables.DimensionStyle("Standard");
            //StandardDimStyle.DIMATFIT = 3;

            //return new netDxf.Entities.AlignedDimension(
            //    this.Startpn.ToDXFVertex3(),
            //    this.Endpn.ToDXFVertex3(),
            //    Norm(this.HieghtReferP - this.Endpn),
            //    StandardDimStyle);

            return new netDxf.Entities.AlignedDimension()
            {
                FirstReferencePoint = this.Startpn.ToDXFVertex3(),
                SecondReferencePoint = this.Endpn.ToDXFVertex3(),
                Offset = Norm(this.HieghtReferP - this.Endpn),
                Color = this._color.A != 0 ? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
            };
        }
    }
}
