using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using PTL.OpenGL.Definitions;
using PTL.OpenGL.Plot;
using CsGL.OpenGL;

namespace PTL.Geometry
{
    public interface IHasXYZ
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }

    public class PointD : PTL.Math, ICloneable, IHasXYZ
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        #region Constructor and Destructor
        public PointD(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public PointD()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public PointD(PointD p)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Z = p.Z;
        }
             
        #endregion

        #region Operation
        public static PointD operator -(PointD p1, PointD p2)
        {
            PointD p3 = new PointD();

            p3.X = p1.X - p2.X;
            p3.Y = p1.Y - p2.Y;
            p3.Z = p1.Z - p2.Z;

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

            po.X =  p1.X / denominator;
            po.Y =  p1.Y / denominator;
            po.Z =  p1.Z / denominator;

            return po;
        }
        #endregion

        public virtual object Clone ()
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
    }

    public interface IHaveColor
    {
        Color Color { get; set; }
    }
    public interface IHaveBoundary 
    {
        PointD[] Boundary { get; }
    }
    public interface IHavePoint : IHaveBoundary
    {
        Dictionary<String, PointD> PointsDictionary { get; }
    }
    public interface IHaveVector
    {
        Dictionary<String, PointD> VectorDictionary { get; }
    }
    public interface IHaveAngle
    {
        Dictionary<String, Double> AngleDictionary { get; }
    }
    public interface IIsLineArchitecture
    {
        LineType LineType { get; set; }
        float LineWidth { get; set; }
    }
    public interface ICanPlotInOpenGL
    {
        void PlotInOpenGL();
    }

    public abstract class Entity : PlotSub, ICloneable, IHavePoint, ICanPlotInOpenGL
    {
        public String Name { get; set; }
        private EntityCollection fParent;
        public EntityCollection Parent
        {
            get
            {
                return fParent;
            }
            set
            {
                if (value != null)
                {
                    if (this.fParent != value)
                    {
                        if (this.fParent != null)
                            this.fParent.Entities.Remove(this.Name);
                        if (this.Name != null)
                        {
                            if (!value.Entities.ContainsKey(this.Name))
                                value.Entities.Add(this.Name, this);
                            else
                            {
                                int i = 1;
                                while (value.Entities.ContainsKey(this.Name + "-" + i))
                                    i++;
                                this.Name = this.Name + "-" + i;
                                value.Entities.Add(this.Name, this);
                            }
                        }
                        else
                        {
                            this.Name = this.GetType().Name.ToString() + value.Entities.Count;
                            value.Entities.Add(this.Name, this);
                        }
                        this.fParent = value;
                    }
                }
                else
                {
                    this.fParent.Entities.Remove(this.Name);
                    this.fParent = value;
                }
            }
        }
        public abstract PointD[] Boundary { get; }
        public abstract Dictionary<String, PointD> PointsDictionary { get; }

        #region ICloneable 成員
        public abstract object Clone();
        #endregion

        #region ICanPlotInOpenGL 成員
        public abstract void PlotInOpenGL();
        #endregion
    }
    public abstract class LineArchitecture : Entity, IHaveColor, IIsLineArchitecture
    {
        private Color fColor = Color.Black;
        private LineType fLineType = LineType.Solid;
        private float fLineWidth = 0.8f;

        public Color Color
        {
            get { return this.fColor; }
            set { this.fColor = value; }
        }
        public LineType LineType
        {
            get { return this.fLineType; }
            set { this.fLineType = value; }
        }
        public float LineWidth
        {
            get { return this.fLineWidth; }
            set { this.fLineWidth = value; }
        }
    }

    public class Line : LineArchitecture, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public PointD p1;
        public PointD p2;

        #region Constructor and Destructor
        public Line(PointD tstartpnt, PointD tendpnt)
        {
            this.p1 = tstartpnt.Clone() as PointD;
            this.p2 = tendpnt.Clone() as PointD;
        }
        public Line(PointD tstartpnt, PointD tendpnt, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            this.p1 = tstartpnt.Clone() as PointD;
            this.p2 = tendpnt.Clone() as PointD;
        }
        public Line(PointD tstartpnt, PointD tendpnt, Color tcolor, float twidth)
        {
            this.Color = tcolor;
            this.LineWidth = twidth;

            this.p1 = tstartpnt.Clone() as PointD;
            this.p2 = tendpnt.Clone() as PointD;
        }
        public Line(Line tline, Color tcolor, LineType ttype, float twidth)
        {
            if (tline.Name != null)
                Name = tline.Name;
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            this.p1 = tline.p1.Clone() as PointD;
            this.p2 = tline.p2.Clone() as PointD;
        }
        public Line()
        {
        }
        #endregion

        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.p1.Clone() as PointD, this.p1.Clone() as PointD };
                Compare_Boundary(boundary, p2);
                return boundary;
            }
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("p1", p1);
                pointsDic.Add("p2", p2);
                return pointsDic;
            }
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
            //Line Style
            glLineWidth(this.LineWidth);
            SetLineType(this.LineType);
            glColor3d(this.Color);

            //Draw Line
            GL.glBegin(GL.GL_LINE_STRIP);
            Vertex3d(this.p1);
            Vertex3d(this.p2);
            GL.glEnd();
        }

        public override object Clone()
        {
            Line aLine = new Line();
            if (this.Name != null)
                aLine.Name = this.Name;
            aLine.Color = this.Color;
            aLine.LineType = this.LineType;
            aLine.LineWidth = this.LineWidth;

            aLine.p1 = this.p1.Clone() as PointD;
            aLine.p2 = this.p2.Clone() as PointD;
            return aLine;
        }
    }
    public class PolyLine : LineArchitecture, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public PointD[] points;

        #region Constructor and Destructor
        public PolyLine(Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;
        }
        public PolyLine(int nump)
        {
            points = new PointD[nump];
            for (int i = 0; i < nump; i++)
                points[i] = new PointD();
        }
        public PolyLine(PointD[] tpoints)
        {
            int nump = tpoints.Length;
            points = new PointD[nump];
            for (int i = 0; i < nump; i++)
                points[i] = tpoints[i].Clone() as PointD;
        }
        public PolyLine(PointD[] tpoints, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            int nump = tpoints.Length;
            points = new PointD[nump];
            for (int i = 0; i < nump; i++)
                points[i] = tpoints[i].Clone() as PointD;
        }
        public PolyLine()
        {
        }
        #endregion

        public void AddPoint(PointD tpoint)
        {
            if (points == null)
                points = new PointD[1];
            else
                Array.Resize(ref points, points.Length + 1);

            points[points.Length - 1] = tpoint.Clone() as PointD;
        }

        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.points[0].Clone() as PointD, this.points[0].Clone() as PointD };
                foreach (PointD p in this.points)
                    Compare_Boundary(boundary, p);
                return boundary;
            }
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                int i = 0;
                foreach (PointD p in points)
                {
                    i++;
                    pointsDic.Add("point" + i, p);
                }
                return pointsDic;
            }
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
            for (int i = 0; i < this.points.Length; i++)
            {
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("8");
                sw.WriteLine(this.Parent.Name);//圖層名稱
                sw.WriteLine("10");
                sw.WriteLine(this.points[i].X.ToString());//第一點X座標
                sw.WriteLine("20");
                sw.WriteLine(this.points[i].Y.ToString());//第一點Y座標
            }
            sw.WriteLine("0");
            sw.WriteLine("SEQEND");
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            sw.WriteLine("-COLOR T {0},{1},{2}", this.Color.R, this.Color.G, this.Color.B);
            sw.WriteLine("-LWEIGHT {0}", this.LineWidth / 8.0);
            sw.WriteLine("PLINE");
            foreach (PointD p in this.points)
            {
                sw.WriteLine(p.X.ToString() + "," + p.Y.ToString());
            }
            sw.WriteLine("");
        }
        public override void PlotInOpenGL()
        {
            //Line Style
            glLineWidth(this.LineWidth);
            SetLineType(this.LineType);
            glColor3d(this.Color);

            //Draw Line
            GL.glBegin(GL.GL_LINE_STRIP);
            for (int k = 0; k < this.points.Length; k++)
                Vertex3d(this.points[k]);
            GL.glEnd();
        }

        public override object Clone()
        {
            PolyLine aPolyLine = new PolyLine();
            if (this.Name != null)
                aPolyLine.Name = this.Name;
            aPolyLine.Color = this.Color;
            aPolyLine.LineType = this.LineType;
            aPolyLine.LineWidth = this.LineWidth;

            int nump = this.points.Length;
            aPolyLine.points = new PointD[nump];
            for (int i = 0; i < nump; i++)
                aPolyLine.points[i] = this.points[i].Clone() as PointD;
            return aPolyLine;
        }
    }
    public class Circle : LineArchitecture, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public PointD Center;
        public double Radius;

        #region Constructor and Destructor
        public Circle(PointD tcenter, double tradius)
        {
            Center = tcenter.Clone() as PointD;
            Radius = tradius;
        }
        public Circle(PointD tcenter, double tradius, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            this.Center = tcenter.Clone() as PointD;
            this.Radius = tradius;
        }
        public Circle()
        {
        }
        #endregion

        #region DXFEntity
        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.Center.Clone() as PointD, this.Center.Clone() as PointD };
                Compare_Boundary(boundary, Center + new PointD(Radius, 0, 0));
                Compare_Boundary(boundary, Center + new PointD(-Radius, 0, 0));
                Compare_Boundary(boundary, Center + new PointD(0, Radius, 0));
                Compare_Boundary(boundary, Center + new PointD(0, -Radius, 0));
                return boundary;
            }
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("Center", Center);
                return pointsDic;
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
            //Line Style
            glLineWidth(this.LineWidth);
            SetLineType(this.LineType);
            glColor3d(this.Color);

            PlotSub.PlotCircle(this.Center, this.Radius);
        }
        public override object Clone()
        {
            Circle aCircle = new Circle();
            if (this.Name != null)
                aCircle.Name = this.Name;
            aCircle.Color = this.Color;
            aCircle.LineType = this.LineType;
            aCircle.LineWidth = this.LineWidth;

            aCircle.Center = this.Center.Clone() as PointD;
            aCircle.Radius = this.Radius;
            return aCircle;
        }
        #endregion
    }
    public class Arc : LineArchitecture, IHaveAngle, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
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
            Center = tcenter.Clone() as PointD;
            Radius = tradius;
        }
        public Arc(PointD tcenter, double tradius, double tStartAng, double tEndAng, Color tcolor, LineType ttype, float twidth)
        {
            this.Color = tcolor;
            this.LineType = ttype;
            this.LineWidth = twidth;

            Center = tcenter.Clone() as PointD;
            Radius = tradius;
            StartAng = tStartAng;
            EndAng = tEndAng;
        }
        public Arc()
        {
        }
        #endregion

        #region DXFEntity
        public override PointD[] Boundary
        {
            get
            {
                PointD startPoint = Center + new PointD(Radius * Cos(StartAng), Radius * Sin(StartAng), 0);
                PointD endPoint = Center + new PointD(Radius * Cos(EndAng), Radius * Sin(EndAng), 0);
                PointD[] boundary = new PointD[2] { startPoint.Clone() as PointD, startPoint.Clone() as PointD };
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
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("Center", Center);
                return pointsDic;
            }
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
            aArc.Color = this.Color;
            aArc.LineType = this.LineType;
            aArc.LineWidth = this.LineWidth;

            aArc.Center = this.Center.Clone() as PointD;
            aArc.Radius = this.Radius;
            aArc.StartAng = this.StartAng;
            aArc.EndAng = this.EndAng;
            return aArc;
        }
        #endregion

        public Dictionary<string, double> AngleDictionary
        {
            get
            {
                Dictionary<String, double> angleDic = new Dictionary<String, double>();
                angleDic.Add("StartAngle", StartAng);
                angleDic.Add("EndAng", EndAng);
                return angleDic;
            }
        }

        public override void PlotInOpenGL()
        {
            //Line Style
            glLineWidth(this.LineWidth);
            SetLineType(this.LineType);
            glColor3d(this.Color);

            PlotArc(this.Center, this.Radius, this.StartAng, this.EndAng);
        }
    }
    public class Ellipse : LineArchitecture, IHaveVector, IHaveAngle, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        //欄位，作為橢圓的實際定義
        private PointD fCenter;
        private PointD fEndPointOfMajorAxis;//Relative To Center
        private PointD fNormal;
        private double fRatio;
        private double fStartAngle = 0, fEndAngle = 2 * PI;

        //屬性，對應欄位
        public PointD Center
        {
            get { return fCenter; }
            set { this.fCenter = value; }
        }
        public PointD EndPointOfMajorAxis
        {
            get { return this.fEndPointOfMajorAxis; }
            set { this.fEndPointOfMajorAxis = value; }
        }
        public PointD Normal
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
        public PointD MajorDirection
        {
            get { return Normalize(fEndPointOfMajorAxis); }
            set
            {
                double Ra = GetLength(fEndPointOfMajorAxis);
                this.fEndPointOfMajorAxis = Ra * Normalize(value);
            }
        }
        public PointD MinorDirection
        {
            get { return Normalize(Cross(fNormal, fEndPointOfMajorAxis)); }
            set
            {
                this.fNormal = Normalize(Cross(fEndPointOfMajorAxis, value));
            }
        }
        public double Ra
        { get { return GetLength(fEndPointOfMajorAxis); } set { this.fEndPointOfMajorAxis = value * Normalize(fEndPointOfMajorAxis); } }
        public double Rb
        { get { return fRatio * GetLength(fEndPointOfMajorAxis); } set { this.fRatio = value / GetLength(fEndPointOfMajorAxis); } }


        #region Constructor
        public Ellipse(PointD center, PointD majorDirection, PointD minorDirection, Double ra, double rb)
        {
            this.fCenter = center;
            this.fEndPointOfMajorAxis = Normalize(majorDirection) * Ra;
            this.fNormal = Cross(majorDirection, minorDirection);
            this.fRatio = rb / ra;
        }
        public Ellipse(PointD center, PointD majorDirection, PointD minorDirection, Double ra, double rb, Color color, LineType lineType, float width)
        {
            this.fCenter = center;
            this.fEndPointOfMajorAxis = Normalize(majorDirection) * ra;
            this.fNormal = Cross(majorDirection, minorDirection);
            this.fRatio = rb / ra;
            this.Color = color;
            this.LineType = lineType;
            this.LineWidth = width;
        }
        public Ellipse(PointD center, PointD endPointOfMajorAxis, PointD normal, double ratio, double startAngle, double endAngle)
        {
            this.fCenter = center;
            this.fEndPointOfMajorAxis = endPointOfMajorAxis;
            this.fNormal = normal;
            this.fRatio = ratio;
            this.fStartAngle = startAngle;
            this.fEndAngle = endAngle;
        }
        public Ellipse(PointD center, PointD endPointOfMajorAxis, PointD normal, double ratio)
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

        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.Center.Clone() as PointD, this.Center.Clone() as PointD };
                Compare_Boundary(boundary, Center + fEndPointOfMajorAxis);
                Compare_Boundary(boundary, Center - fEndPointOfMajorAxis);
                Compare_Boundary(boundary, Center + MinorDirection * Rb);
                Compare_Boundary(boundary, Center - MinorDirection * Rb);
                return boundary;
            }
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
            aEllipse.Color = this.Color;
            aEllipse.LineType = this.LineType;
            aEllipse.LineWidth = this.LineWidth;
            return aEllipse;
        }

        public override Dictionary<string, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("Center", this.fCenter);
                return pointsDic;
            }
        }

        public Dictionary<string, PointD> VectorDictionary
        {
            get
            {
                Dictionary<String, PointD> Dic = new Dictionary<String, PointD>();
                Dic.Add("EndPointOfMajorAxis", this.fEndPointOfMajorAxis);
                Dic.Add("Normal", this.Normal);
                return Dic;
            }
        }

        public Dictionary<string, double> AngleDictionary
        {
            get
            {
                Dictionary<String, double> Dic = new Dictionary<String, double>();
                Dic.Add("StartAngle", this.fStartAngle);
                Dic.Add("EndAngle", this.fEndAngle);
                return Dic;
            }
        }

        public override void PlotInOpenGL()
        {
            //Line Style
            glLineWidth(this.LineWidth);
            SetLineType(this.LineType);
            glColor3d(this.Color);

            int nump = 21;
            double step = 2 * PI / (nump - 1);

            PointD center = this.Center;
            PointD e1 = this.MajorDirection.Clone() as PointD;
            PointD e2 = this.MinorDirection.Clone() as PointD;
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
        }
    }
    public class Text : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public string Value = "Input Text";//Text

        public string FontStyle = "Times New Roman";//Text Style Name
        public double TextHieght = 22;
        public uint FontWeight = 500;
        public uint Italic = 0;
        public uint Underline = 0;

        private Color fColor = Color.Black;
        public Color Color
        {
            get { return this.fColor; }
            set { this.fColor = value; }
        }

        public PointD RefPoint = new PointD();//First alignment point
        public string JustType = "Center";//Justification Type
        public double Dist = 10;
        public int Orietation = 0; //Text rotation


        #region Constructor and Destructor
        public Text(string value, PointD point, Color color, string justtype, string style, int rotateang, uint fontWeight)
        {
            Value = value;
            RefPoint = point.Clone() as PointD;
            Color = color;
            JustType = justtype;
            FontStyle = style;
            Orietation = rotateang;
            FontWeight = fontWeight;
        }
        public Text(string value, PointD point, Color color, String style, double textHieght)
        {
            this.Value = value;
            this.RefPoint = point.Clone() as PointD;
            this.Color = color;
            this.FontStyle = style;
            this.TextHieght = textHieght;
        }
        public Text(string value, PointD point, Color color)
        {
            this.Value = value;
            this.RefPoint = point.Clone() as PointD;
            this.Color = color;
        }
        public Text(string value, PointD point)
        {
            Value = value;
            RefPoint = point.Clone() as PointD;
        }
        public Text()
        {
        }
        #endregion

        #region DXFEntity
        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.RefPoint.Clone() as PointD, this.RefPoint.Clone() as PointD };
                return boundary;
            }
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("RefPoint", RefPoint);
                return pointsDic;
            }
        }
        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            sw.WriteLine("0");
            sw.WriteLine("TEXT");
            sw.WriteLine("8");
            sw.WriteLine(this.Parent.Name);//圖層名稱
            sw.WriteLine("62");
            sw.WriteLine(Part.SystemColor2DXFColorIndex(this.Color));//顏色號碼
            sw.WriteLine("10");
            sw.WriteLine(this.RefPoint.X.ToString());//定義點X座標
            sw.WriteLine("20");
            sw.WriteLine(this.RefPoint.Y.ToString());//定義點Y座標
            sw.WriteLine("30");
            sw.WriteLine(this.RefPoint.Z.ToString());//定義點Z座標
            sw.WriteLine("40");
            sw.WriteLine(this.TextHieght.ToString());//文字高度
            sw.WriteLine("50");
            sw.WriteLine(this.Orietation.ToString());//文字旋轉
            sw.WriteLine("1");
            //對中文進行處理
            String chineseString = "";
            String finalString = "";
            for (int i = 0; i < this.Value.Length; i++)
            {
                if (System.Text.Encoding.Default.GetByteCount(this.Value[i].ToString()) > 1 && i != this.Value.Length - 1)
                    chineseString += this.Value[i];
                else
                {
                    if (chineseString != "")
                    {
                        finalString += "{\\fPMingLiU|b0|i0|c136|p18;" + chineseString + "}" + this.Value[i];
                        chineseString = "";
                    }
                    else
                        finalString += this.Value[i];
                }
            }
            sw.WriteLine(finalString);//字串本身
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            throw new NotImplementedException();
        }
        public override object Clone()
        {
            Text aText = new Text();
            if (this.Name != null)
                aText.Name = this.Name;
            aText.Value = this.Value;
            aText.TextHieght = this.TextHieght;
            aText.RefPoint = this.RefPoint.Clone() as PointD;
            aText.JustType = this.JustType;
            aText.FontStyle = this.FontStyle;
            aText.Orietation = this.Orietation;
            aText.FontWeight = this.FontWeight;
            aText.Color = this.Color;
            return aText;
        }
        #endregion

        public override void PlotInOpenGL()
        {
            WriteString(this.Value, this.RefPoint, this.Color);
        }
    }
    public class AlignedDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public double Value { get { return GetLength(Startpn - Endpn); } }
        public PointD Startpn = new PointD();
        public PointD Endpn = new PointD();
        public PointD HieghtReferP = new PointD();
        public PointD TextReferP = new PointD();
        private Color fColor = Color.LawnGreen;
        public Color Color
        {
            get { return this.fColor; }
            set { this.fColor = value; }
        }
        public string Text = "<>";
        public double ArrowSize = 10.0;

        #region Constructor and Destructor
        public AlignedDimension(PointD tStartpn, PointD tEndpn, PointD tTextReferpn, string tText, double tArrowSize)
        {
            Startpn = tStartpn.Clone() as PointD;
            Endpn = tEndpn.Clone() as PointD;

            TextReferP = tTextReferpn.Clone() as PointD;
            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            double refheight = Dot(v1, tTextReferpn - Endpn);
            HieghtReferP = Endpn + v1 * refheight;

            this.Text = tText;
            ArrowSize = tArrowSize;
        }
        public AlignedDimension(PointD tStartpn, PointD tEndpn, PointD tTextReferpn, double tArrowSize)
        {
            Startpn = tStartpn.Clone() as PointD;
            Endpn = tEndpn.Clone() as PointD;
            TextReferP = tTextReferpn.Clone() as PointD;

            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            double refheight = Abs(Dot(v1, tTextReferpn - Endpn));
            HieghtReferP = Endpn + v1 * refheight;

            ArrowSize = tArrowSize;
        }
        public AlignedDimension(PointD tStartpn, PointD tEndpn, double refheight, double tArrowSize)
        {
            Startpn = tStartpn.Clone() as PointD;
            Endpn = tEndpn.Clone() as PointD;

            ArrowSize = tArrowSize;

            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            TextReferP = (Startpn + Endpn) / 2 + v1 * refheight;
            HieghtReferP = Endpn + v1 * refheight;
        }
        public AlignedDimension(PointD tStartpn, PointD tEndpn)
        {
            Startpn = tStartpn.Clone() as PointD;
            Endpn = tEndpn.Clone() as PointD;

            PointD v1 = Normalize(Cross(new PointD(0, 0, 1), Endpn - Startpn));
            TextReferP = (Startpn + Endpn) / 2 + v1 * ArrowSize * 0.5;
            HieghtReferP = Endpn + v1 * ArrowSize * 0.5;
        }
        public AlignedDimension()
        {
        }
        #endregion

        #region DXFEntity
        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.Startpn.Clone() as PointD, this.Startpn.Clone() as PointD };
                Compare_Boundary(boundary, Endpn);
                Compare_Boundary(boundary, HieghtReferP);
                Compare_Boundary(boundary, TextReferP);
                Compare_Boundary(boundary, HieghtReferP + (Startpn - Endpn));
                return boundary;
            }
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("Startpn", Startpn);
                pointsDic.Add("Endpn", Endpn);
                pointsDic.Add("HieghtReferP", HieghtReferP);
                pointsDic.Add("TextReferP", TextReferP);
                return pointsDic;
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
            aAligD.Startpn = this.Startpn.Clone() as PointD;
            aAligD.Endpn = this.Endpn.Clone() as PointD;
            aAligD.TextReferP = this.TextReferP.Clone() as PointD;
            aAligD.HieghtReferP = this.HieghtReferP.Clone() as PointD;
            aAligD.Color = this.Color;

            aAligD.Text = this.Text;
            aAligD.ArrowSize = this.ArrowSize;
            return aAligD;
        }
        #endregion

        public override void PlotInOpenGL()
        {
            PointD v1 = this.Endpn - this.Startpn;
            PointD v2 = this.HieghtReferP - this.Endpn;

            double dist = Round(GetLength(this.Endpn - this.Startpn), 0.001);
            double RefDist = GetLength(this.HieghtReferP - this.Endpn);
            double ExtenDist1 = 0.05 * this.ArrowSize;
            double ExtenDist2 = 0.05 * this.ArrowSize;

            //Plot Text
            WriteString(dist.ToString(), this.TextReferP, this.Color);

            //Dimension Line
            PointD p1;
            PointD p2;
            if (Dot(this.TextReferP - this.Startpn, v1) > GetLength(v1) * GetLength(v1))
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
            tLine.PlotInOpenGL();

            //Extension Line
            p1 = this.Startpn + ExtenDist1 * v2 / GetLength(v2);
            p2 = this.Startpn + v2 + ExtenDist2 * v2 / GetLength(v2);
            tLine = new Line(p1, p2, this.Color, 1.0f);
            tLine.PlotInOpenGL();


            p1 = this.Endpn + ExtenDist1 * v2 / GetLength(v2);
            p2 = this.Endpn + v2 + ExtenDist2 * v2 / GetLength(v2);
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
        }
    }
    public class RadialDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public double Value { get { return GetLength(Startpn - Endpn); } }
        public PointD Startpn = new PointD();
        public PointD Endpn = new PointD();
        public PointD TextReferP = new PointD();
        public Double LeaderLength;
        private Color fColor = Color.LawnGreen;
        public Color Color
        {
            get { return this.fColor; }
            set { this.fColor = value; }
        }
        public string Text = "<>";
        public double ArrowSize = 10.0;

        #region Constructor and Destructor
        public RadialDimension(Circle aCircle, double refDirection, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center.Clone() as PointD;
            Endpn = aCircle.Center + V1;
            TextReferP = (Startpn + Endpn) / 2;
            LeaderLength = GetLength((this.Startpn - this.TextReferP)) - Value;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public RadialDimension(Circle aCircle, double refDirection, PointD tTextReferpn, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center.Clone() as PointD;
            Endpn = aCircle.Center + V1;
            TextReferP = tTextReferpn.Clone() as PointD;
            LeaderLength = GetLength((this.Startpn - this.TextReferP)) - Value;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public RadialDimension(Circle aCircle, double refDirection, PointD tTextReferpn, string tText, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center.Clone() as PointD;
            Endpn = aCircle.Center + V1;
            TextReferP = tTextReferpn.Clone() as PointD;
            LeaderLength = GetLength((this.Startpn - this.TextReferP)) - Value;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
            this.Text = tText;
        }
        public RadialDimension()
        {
        }
        #endregion

        #region Method
        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.Startpn.Clone() as PointD, this.Startpn.Clone() as PointD };
                Compare_Boundary(boundary, Endpn);
                Compare_Boundary(boundary, TextReferP);
                return boundary;
            }
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("Startpn", Startpn);
                pointsDic.Add("Endpn", Endpn);
                pointsDic.Add("TextReferP", TextReferP);
                return pointsDic;
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
            sw.WriteLine(this.Startpn.X.ToString());//直徑標註、半徑標註及角度標註的定義點X座標
            sw.WriteLine("25");
            sw.WriteLine(this.Startpn.Y.ToString());//直徑標註、半徑標註及角度標註的定義點Y座標
            sw.WriteLine("35");
            sw.WriteLine(this.Startpn.Z.ToString());//直徑標註、半徑標註及角度標註的定義點Z座標
            sw.WriteLine("40");
            sw.WriteLine(this.LeaderLength.ToString());//Leader length for radius and diameter dimensions

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
            throw new NotImplementedException();
        }
        public override object Clone()
        {
            RadialDimension aAligD = new RadialDimension();
            if (this.Name != null)
                aAligD.Name = this.Name;
            aAligD.Startpn = this.Startpn.Clone() as PointD;
            aAligD.Endpn = this.Endpn.Clone() as PointD;
            aAligD.TextReferP = this.TextReferP.Clone() as PointD;
            aAligD.Color = this.Color;
            aAligD.LeaderLength = this.LeaderLength;

            aAligD.Text = this.Text;
            aAligD.ArrowSize = this.ArrowSize;
            return aAligD;
        }
        #endregion

        public override void PlotInOpenGL()
        {
            PointD v1 = this.Endpn - this.Startpn;

            double dist = Round(GetLength(this.Endpn - this.Startpn), 0.001);
            double ExtenDist1 = 0.05 * this.ArrowSize;
            double ExtenDist2 = 0.05 * this.ArrowSize;

            //Plot Text
            WriteString(dist.ToString(), this.TextReferP, this.Color);

            //Dimension Line
            PointD p1;
            PointD p2;
            p1 = this.Startpn;
            p2 = this.Startpn + v1 * ((GetLength(v1) + this.LeaderLength) / GetLength(v1));
            Line tLine = new Line(p1, p2, this.Color, 1.0f);
            tLine.PlotInOpenGL();

            //Plot Arrow
            double theta = RadToDeg(Atan2(v1.Y, v1.X));

            //Arrow
            p1 = new PointD(0, 0, 0);
            p2 = new PointD(0.15, -0.03, 0) * this.ArrowSize;
            PointD p3 = new PointD(0.15, 0.03, 0) * this.ArrowSize;

            PointD p4 = this.Endpn;
            PlotTriangle(p1, p2, p3, p4, theta + 180, this.Color);
        }
    }
    public class DiametricDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public double Value { get { return GetLength(Startpn - Endpn); } }
        public PointD Startpn = new PointD();
        public PointD Endpn = new PointD();
        public PointD TextReferP = new PointD();
        public Double LeaderLength;
        private Color fColor = Color.LawnGreen;
        public Color Color
        {
            get { return this.fColor; }
            set { this.fColor = value; }
        }
        public string Text = "<>";
        public double ArrowSize = 10.0;

        #region Constructor and Destructor
        public DiametricDimension(Circle aCircle, double refDirection, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center - V1;
            Endpn = aCircle.Center + V1;
            TextReferP = (Startpn + Endpn) / 2;
            LeaderLength = GetLength(((this.Startpn + this.Endpn) / 2.0 - this.TextReferP)) - Value / 2.0;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public DiametricDimension(Circle aCircle, double refDirection, PointD tTextReferpn, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center - V1;
            Endpn = aCircle.Center + V1;
            TextReferP = tTextReferpn.Clone() as PointD;
            LeaderLength = GetLength(((this.Startpn + this.Endpn) / 2.0 - this.TextReferP)) - Value / 2.0;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public DiametricDimension(Circle aCircle, double refDirection, PointD tTextReferpn, string tText, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center - V1;
            Endpn = aCircle.Center + V1;
            TextReferP = tTextReferpn.Clone() as PointD;
            LeaderLength = GetLength(((this.Startpn + this.Endpn) / 2.0 - this.TextReferP)) - Value / 2.0;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
            this.Text = tText;
        }
        public DiametricDimension()
        {
        }
        #endregion

        #region Method
        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { this.Startpn.Clone() as PointD, this.Startpn.Clone() as PointD };
                Compare_Boundary(boundary, Endpn);
                Compare_Boundary(boundary, TextReferP);
                return boundary;
            }
        }
        public override Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                pointsDic.Add("Startpn", Startpn);
                pointsDic.Add("Endpn", Endpn);
                pointsDic.Add("TextReferP", TextReferP);
                return pointsDic;
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
            sw.WriteLine("3");//標註種類
            sw.WriteLine("1");//使用者明確輸入的標註文字
            sw.WriteLine(" ");//抑制文字
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
            sw.WriteLine(this.Startpn.X.ToString());//直徑標註、半徑標註及角度標註的定義點X座標
            sw.WriteLine("25");
            sw.WriteLine(this.Startpn.Y.ToString());//直徑標註、半徑標註及角度標註的定義點Y座標
            sw.WriteLine("35");
            sw.WriteLine(this.Startpn.Z.ToString());//直徑標註、半徑標註及角度標註的定義點Z座標
            sw.WriteLine("40");
            sw.WriteLine(this.LeaderLength.ToString());//Leader length for radius and diameter dimensions

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
            throw new NotImplementedException();
        }
        public override object Clone()
        {
            DiametricDimension aAligD = new DiametricDimension();
            if (this.Name != null)
                aAligD.Name = this.Name;
            aAligD.Startpn = this.Startpn.Clone() as PointD;
            aAligD.Endpn = this.Endpn.Clone() as PointD;
            aAligD.TextReferP = this.TextReferP.Clone() as PointD;
            aAligD.Color = this.Color;

            aAligD.Text = this.Text;
            aAligD.ArrowSize = this.ArrowSize;
            return aAligD;
        }
        #endregion



        public override void PlotInOpenGL()
        {
            PointD v1 = this.Endpn - this.Startpn;

            double dist = Round(GetLength(this.Endpn - this.Startpn), 0.001);
            double ExtenDist1 = 0.05 * this.ArrowSize;
            double ExtenDist2 = 0.05 * this.ArrowSize;

            //Plot Text
            WriteString(dist.ToString(), this.TextReferP, this.Color);

            //Dimension Line
            PointD p1;
            PointD p2;
            if (Dot(v1, this.TextReferP - (this.Startpn + this.Endpn) / 2.0) >= 0)
            {
                p1 = this.Startpn;
                p2 = this.Startpn + v1 * ((GetLength(v1) + this.LeaderLength) / GetLength(v1));
            }
            else
            {
                p1 = this.Endpn;
                p2 = this.Endpn - v1 * ((GetLength(v1) + this.LeaderLength) / GetLength(v1));
            }

            Line tLine = new Line(p1, p2, this.Color, 1.0f);
            tLine.PlotInOpenGL();

            //Plot Arrow
            double theta = RadToDeg(Atan2(v1.Y, v1.X));

            //Arrow
            p1 = new PointD(0, 0, 0);
            p2 = new PointD(0.15, -0.03, 0) * this.ArrowSize;
            PointD p3 = new PointD(0.15, 0.03, 0) * this.ArrowSize;

            PointD p4 = this.Startpn;
            PlotTriangle(p1, p2, p3, p4, theta, this.Color);
            p4 = this.Endpn;
            PlotTriangle(p1, p2, p3, p4, theta + 180, this.Color);
        }
    }

    public abstract class EntityCollection : PlotSub, ICloneable, ICanPlotInOpenGL, IHaveBoundary
    {
        public String Name { get; set; }
        public Dictionary<String, Entity> Entities;
        public virtual PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { new PointD(), new PointD() };
                if (Entities.Count != 0)
                {
                    Entity[] entities = Entities.Values.ToArray();
                    boundary = entities[0].Boundary;
                    PointD[] eboundary;
                    foreach (Entity aEntity in entities)
                    {
                        eboundary = aEntity.Boundary;
                        Compare_Boundary(boundary, eboundary[0]);
                        Compare_Boundary(boundary, eboundary[1]);
                    }
                }
                return boundary;
            }
        }
        public virtual Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                foreach (Entity aEntity in Entities.Values)
                {
                    Dictionary<String, PointD> apointsDic = aEntity.PointsDictionary;
                    String[] keys = apointsDic.Keys.ToArray();
                    PointD[] points = apointsDic.Values.ToArray();
                    for (int i = 0; i < apointsDic.Count; i++)
                    {
                        pointsDic.Add(aEntity.Name + "-" + keys[i], points[i]);
                    }
                }
                return pointsDic;
            }
        }

        public EntityCollection()
        {
            this.Name = "EntityCollection";
            this.Entities = new Dictionary<String, Entity>();
        }

        public abstract void AddEntity(Entity aEntity);
        public abstract void RemoveEntity(Entity aEntity);

        #region ICloneable 成員
        public abstract object Clone();
        #endregion

        #region ICanPlotInOpenGL 成員

        public virtual void PlotInOpenGL()
        {
            Entity[] entities = this.Entities.Values.ToArray();
            foreach (Entity aEntity in entities)
            {
                if (aEntity is ICanPlotInOpenGL)
                    (aEntity as ICanPlotInOpenGL).PlotInOpenGL();
            }
        }

        #endregion
    }
    public class Layer : EntityCollection, ICanBeWritedToDXFFile
    {
        public Boolean Visible { get; set; }

        public Layer(String Name)
            : base()
        {
            this.Name = Name;
            this.Visible = true;
        }
        public Layer()
            : base()
        {
            this.Name = "EntityCollection";
            this.Visible = true;
        }

        #region IDXFMember 成員

        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            foreach (ICanBeWritedToDXFFile item in Entities.Values)
                item.WriteToFileInDxfFormat(sw);
        }

        #endregion

        public override void AddEntity(Entity aEntity)
        {
            aEntity.Parent = this;
        }

        public override void RemoveEntity(Entity aEntity)
        {
            if (this.Entities.ContainsValue(aEntity))
            {
                aEntity.Parent = null;
            }
        }

        public override object Clone()
        {
            Layer aDXFLayer = new Layer();
            if (this.Name != null)
                aDXFLayer.Name = this.Name;
            aDXFLayer.Visible = this.Visible;
            Entity[] entities = this.Entities.Values.ToArray();
            for (int i = 0; i < entities.Length; i++)
                aDXFLayer.AddEntity(entities[i].Clone() as Entity);
            return aDXFLayer;
        }

        public override void PlotInOpenGL()
        {
            if (this.Visible)
                base.PlotInOpenGL();
        }
    }
    public interface ILayerCollection
    {
        void AddLayer(Layer aLayer);
        void RemoveLayer(Layer aLayer);
    }
    
    public class Part : PlotSub,IHaveBoundary, IDXF, ICloneable, ILayerCollection, ICanPlotInOpenGL
    {
        public String Name { get; set; }
        public Dictionary<String, Layer> Layers;
        public PointD[] Boundary
        {
            get
            {
                PointD[] boundary = new PointD[2] { new PointD(), new PointD() };
                if (Layers.Count != 0)
                {
                    Layer[] layers = Layers.Values.ToArray();
                    boundary = layers[0].Boundary;
                    PointD[] lboundary;
                    foreach (Layer aLayer in layers)
                    {
                        lboundary = aLayer.Boundary;
                        Compare_Boundary(boundary, lboundary[0]);
                        Compare_Boundary(boundary, lboundary[1]);
                    }
                }
                return boundary;
            }
        }
        public Dictionary<String, PointD> PointsDictionary
        {
            get
            {
                Dictionary<String, PointD> pointsDic = new Dictionary<String, PointD>();
                foreach (Layer aLayer in Layers.Values)
                {
                    Dictionary<String, PointD> apointsDic = aLayer.PointsDictionary;
                    String[] keys = apointsDic.Keys.ToArray();
                    PointD[] points = apointsDic.Values.ToArray();
                    for (int i = 0; i < apointsDic.Count; i++)
                    {
                        pointsDic.Add(aLayer.Name + "-" + keys[i], points[i]);
                    }
                }
                return pointsDic;
            }
        }

        #region SystemColor_DXFColor
        public static Dictionary<Color, int> SystemColor_DXFColor =
            new Dictionary<Color, int>()
            {{Color.White, 7},
             {Color.Red, 1},
             {Color.Orange, 30},
             {Color.DarkOrange, 32},
             {Color.LawnGreen, 3},
             {Color.SkyBlue, 130},
             {Color.DeepSkyBlue, 140},
             {Color.Blue, 5},
             {Color.AntiqueWhite, 7}};
        #endregion SystemColor_DXFColor

        public Part(String Name)
        {
            this.Name = Name;
            Layers = new Dictionary<String, Layer>();
        }
        public Part()
        {
            Layers = new Dictionary<String, Layer>();
        }

        public void AddLayer(Layer aLayer)
        {
            if (aLayer.Name != null)
            {
                if (!this.Layers.ContainsKey(aLayer.Name))
                    this.Layers.Add(aLayer.Name, aLayer);
                else
                {
                    int i = 1;
                    while (this.Layers.ContainsKey(aLayer.Name + "-" + i))
                        i++;
                    aLayer.Name = aLayer.Name + "-" + i;
                    this.Layers.Add(aLayer.Name, aLayer);
                }
            }
            else
            {
                aLayer.Name = aLayer.GetType().Name.ToString() + this.Layers.Count;
                this.Layers.Add(aLayer.Name, aLayer);
            }
        }

        public object Clone()
        {
            Part aDXF = new Part();
            if (this.Name != null)
                aDXF.Name = this.Name;
            Layer[] layers = this.Layers.Values.ToArray();
            for (int i = 0; i < layers.Length; i++)
                aDXF.AddLayer(layers[i].Clone() as Layer);
            return aDXF;
        }

        public static int SystemColor2DXFColorIndex(Color color)
        {
            int colorIndex = 256;
            if (SystemColor_DXFColor.ContainsKey(color))
                colorIndex = SystemColor_DXFColor[color];//顏色號碼
            return colorIndex;
        }

        #region IDXF 成員
        public void Save2DxfFile(String filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            StreamWriter sw = fileInfo.CreateText();
            sw.Close();
            sw = new StreamWriter(filename, true, Encoding.Default);

            sw.WriteLine("0");
            sw.WriteLine("SECTION");
            sw.WriteLine("2");
            sw.WriteLine("ENTITIES");
            Layer[] dxfLayers = this.Layers.Values.ToArray();
            foreach (Layer aLayer in dxfLayers)
                aLayer.WriteToFileInDxfFormat(sw);
            sw.WriteLine("0");
            sw.WriteLine("ENDSEC");
            //檔案結尾標記
            sw.WriteLine("0");
            sw.WriteLine("EOF");

            sw.Flush();
            sw.Close();
        }
        #endregion

        #region ILayerCollection 成員


        public void RemoveLayer(Layer aLayer)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICanPlotInOpenGL 成員

        public void PlotInOpenGL()
        {
            Layer[] Layers = this.Layers.Values.ToArray();
            foreach (Layer aLayer in Layers)
            {
                aLayer.PlotInOpenGL();
            }
        }

        #endregion
    }
}
