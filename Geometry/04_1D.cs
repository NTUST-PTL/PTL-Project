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
    public class Line : LineArchitectureEntity, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile, IToDXFEntity
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

        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                boundary = new XYZ4[2] { (PointD)this.p1.Clone(), (PointD)this.p1.Clone() };
                Compare_Boundary(boundary, p2);
                if (this.CoordinateSystem != null)
                    boundary = Transport(this.CoordinateSystem, boundary).ToArray();
                return boundary;
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
                SetLineType(this.LineType);
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
    public class PolyLine : LineArchitectureEntity, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile, IToDXFEntity
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

        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary = null;
                if (Points.Count > 0)
                {
                    if (this.CoordinateSystem != null)
                    {
                        boundary = new XYZ4[2] { Transport4(this.CoordinateSystem, Points[0]), Transport4(this.CoordinateSystem, Points[0]) };
                        foreach (PointD p in this.Points)
                            Compare_Boundary(boundary, Transport4(this.CoordinateSystem, p));
                    }
                    else
                    {
                        boundary = new XYZ4[2] { this.Points[0].Clone() as XYZ4, this.Points[0].Clone() as XYZ4 };
                        foreach (PointD p in this.Points)
                            Compare_Boundary(boundary, p);
                    }
                }
                return boundary;
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
                SetLineType(this.LineType);
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
    public class ParaLine : PolyLine
    {
        protected Func<double, PointD> function;
        protected Func<double, PointD> transformedFunction;
        public Func<double, PointD> Function
        {
            get { return this.function; }
            set {
                this.function = null;
                this.function = value;
            }
        }
        protected List<double> tt;
        public List<double> TT
        {
            get { return tt; }
            set{
                if (this.tt != value)
	            {
                    this.tt = value;
	            }
            }
        }

        public virtual void RenderGeomatry()
        {
            if (this.CoordinateSystem != null)
                this.transformedFunction = (double t) =>
                {
                    PointD p = this.function(t);
                    p.Transform(this.CoordinateSystem);
                    return p;
                };
            else
                this.transformedFunction = this.function;
            this.Points = new List<XYZ4>();
            foreach (var t in this.TT)
                this.Points.Add(transformedFunction(t));
        }

        public override object Clone()
        {
            ParaLine aParaLine = new ParaLine() {
                Color = this._color,
                LineType = this.LineType,
                LineWidth = this.LineWidth,
                Function = this.Function};
            aParaLine.TT = new List<double>();
            foreach (var t in this.TT)
                aParaLine.TT.Add(t);
            aParaLine.Points = new List<XYZ4>();
            for (int i = 0; i < Points.Count; i++)
                aParaLine.Points.Add(this.Points[i].Clone() as XYZ4);
            return aParaLine;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            Func<double, PointD> function2;
            if (TransformMatrix != null)
                function2 = (double t) =>
                {
                    PointD p = this.function(t);
                    p.Transform(this.CoordinateSystem);
                    return p;
                };
            else
                function2 = this.function;
            this.Function = function2;
        }
    }
    public class Circle : LineArchitectureEntity, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile, IToDXFEntity
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
                SetLineType(this.LineType);
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
    public class Arc : LineArchitectureEntity, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile, IToDXFEntity
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
        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                PointD startPoint;
                PointD endPoint;
                startPoint = Center + new PointD(Radius * Cos(StartAng), Radius * Sin(StartAng), 0);
                endPoint = Center + new PointD(Radius * Cos(EndAng), Radius * Sin(EndAng), 0);
                if (this.CoordinateSystem != null)
                {
                    startPoint = Transport4(this.CoordinateSystem, startPoint);
                    endPoint = Transport4(this.CoordinateSystem, endPoint);
                }
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
                SetLineType(this.LineType);
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
                Color = this._color.A != 0? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
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
    public class Ellipse : LineArchitectureEntity, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
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
            this.fEndPointOfMajorAxis = (Vector)Normalize(majorDirection) * Ra;
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

        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                if (this.CoordinateSystem != null)
                {
                    boundary = new XYZ4[2] { Transport4(this.CoordinateSystem, Center + fEndPointOfMajorAxis), Transport4(this.CoordinateSystem, Center + fEndPointOfMajorAxis) };
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Center - fEndPointOfMajorAxis));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Center + MinorDirection * Rb));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Center - MinorDirection * Rb));
                }
                else
                {
                    boundary = new XYZ4[2] { Center + fEndPointOfMajorAxis, Center + fEndPointOfMajorAxis };
                    Compare_Boundary(boundary, Center - fEndPointOfMajorAxis);
                    Compare_Boundary(boundary, Center + MinorDirection * Rb);
                    Compare_Boundary(boundary, Center - MinorDirection * Rb);
                }
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
            aEllipse.Color = this._color;
            aEllipse.LineType = this.LineType;
            aEllipse.LineWidth = this.LineWidth;
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
                SetLineType(this.LineType);
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
    public class Text : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile
    {
        public string Value = "Input Text";//Text

        public string FontStyle = "Times New Roman";//Text Style Name
        public double TextHieght = 22;
        public uint FontWeight = 500;
        public uint Italic = 0;
        public uint Underline = 0;

        public PointD RefPoint = new PointD();//First alignment point
        public string JustType = "Center";//Justification Type
        public double Dist = 10;
        public double Orietation = 0; //Text rotation


        #region Constructor and Destructor
        public Text(string value, PointD point, Color color, string justtype, string style, int rotateang, uint fontWeight)
        {
            Value = value;
            RefPoint = (PointD)point.Clone();
            Color = color;
            JustType = justtype;
            FontStyle = style;
            Orietation = rotateang;
            FontWeight = fontWeight;
        }
        public Text(string value, PointD point, Color color, String style, double textHieght)
        {
            this.Value = value;
            this.RefPoint = (PointD)point.Clone();
            this.Color = color;
            this.FontStyle = style;
            this.TextHieght = textHieght;
        }
        public Text(string value, PointD point, Color color)
        {
            this.Value = value;
            this.RefPoint = (PointD)point.Clone();
            this.Color = color;
        }
        public Text(string value, PointD point)
        {
            Value = value;
            RefPoint = (PointD)point.Clone();
        }
        public Text()
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
                    boundary = new XYZ4[2] { Transport4(this.CoordinateSystem, this.RefPoint), Transport4(this.CoordinateSystem, this.RefPoint) };  
                else
                    boundary = new XYZ4[2] { (PointD)this.RefPoint.Clone(), (PointD)this.RefPoint.Clone() };
                return boundary;
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
            aText.RefPoint = (PointD)this.RefPoint.Clone();
            aText.JustType = this.JustType;
            aText.FontStyle = this.FontStyle;
            aText.Orietation = this.Orietation;
            aText.FontWeight = this.FontWeight;
            aText.Color = this._color;
            return aText;
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

                WriteString(this.Value, this.RefPoint, this.Color);

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.RefPoint.Transform(TransformMatrix);
        }
    }
    public class AlignedDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile, IToDXFEntity
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
        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                if (this.CoordinateSystem != null)
                {
                    boundary = new XYZ4[2] {Transport4(this.CoordinateSystem, this.Startpn), Transport4(this.CoordinateSystem, this.Startpn) };
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, Endpn));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, HieghtReferP));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, TextReferP));
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, HieghtReferP + (Startpn - Endpn)));
                }
                else
                {
                    boundary = new XYZ4[2] { (PointD)this.Startpn.Clone(), (PointD)this.Startpn.Clone() };
                    Compare_Boundary(boundary, Endpn);
                    Compare_Boundary(boundary, HieghtReferP);
                    Compare_Boundary(boundary, TextReferP);
                    Compare_Boundary(boundary, HieghtReferP + (Startpn - Endpn));
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
                Color = this._color.A != 0? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
            };
        }
    }
    public class RadialDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile, IToDXFEntity
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
                    boundary = new XYZ4[2] {Transport4(this.CoordinateSystem, this.Startpn), Transport4(this.CoordinateSystem, this.Startpn) };
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
                Color = this._color.A != 0? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
            };
        }
    }
    public class DiametricDimension : Entity, IHaveColor, ICanBeWritedToDXFFile, ICanBeWritedToScriptFile, IToDXFEntity
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
        public DiametricDimension(Circle aCircle, double refDirection, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center - V1;
            Endpn = aCircle.Center + V1;
            TextReferP = (Startpn + Endpn) / 2;
            LeaderLength = Norm((this.TextReferP - (this.Startpn + this.Endpn) / 2.0)) - Value / 2.0;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public DiametricDimension(Circle aCircle, double refDirection, PointD tTextReferpn, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center - V1;
            Endpn = aCircle.Center + V1;
            TextReferP = (PointD)tTextReferpn.Clone();
            LeaderLength = Norm((this.TextReferP - (this.Startpn + this.Endpn) / 2.0)) - Value / 2.0;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
        }
        public DiametricDimension(Circle aCircle, double refDirection, PointD tTextReferpn, string tText, double tArrowSize)
        {
            double angle = DegToRad(refDirection);
            PointD V1 = (new PointD(Cos(angle), Sin(angle), 0)) * aCircle.Radius;
            Startpn = aCircle.Center - V1;
            Endpn = aCircle.Center + V1;
            TextReferP = (PointD)tTextReferpn.Clone();
            LeaderLength = Norm((this.TextReferP - (this.Startpn + this.Endpn) / 2.0)) - Value / 2.0;
            if (LeaderLength < 0) LeaderLength = 0;
            ArrowSize = tArrowSize;
            this.Text = tText;
        }
        public DiametricDimension()
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
                    boundary = new XYZ4[2] {Transport4(this.CoordinateSystem, this.Startpn), Transport4(this.CoordinateSystem, this.Startpn) };
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
            sw.WriteLine(this.Endpn.X.ToString());//直徑標註、半徑標註及角度標註的定義點X座標
            sw.WriteLine("25");
            sw.WriteLine(this.Endpn.Y.ToString());//直徑標註、半徑標註及角度標註的定義點Y座標
            sw.WriteLine("35");
            sw.WriteLine(this.Endpn.Z.ToString());//直徑標註、半徑標註及角度標註的定義點Z座標
            sw.WriteLine("40");
            //sw.WriteLine("10");//Leader length for radius and diameter dimensions
            string a = this.LeaderLength.ToString();
            sw.WriteLine(this.LeaderLength.ToString());//Leader length for radius and diameter dimensions

            Text valueText = new Geometry.Text();
            valueText.Value = this.Value.ToString("N3");
            valueText.RefPoint = this.TextReferP;
            PointD v1 = this.Endpn - this.Startpn;
            valueText.Orietation = (int)RadToDeg(Atan2(v1.Y, v1.X));
            if (valueText.Orietation > 90)
                valueText.Orietation -= 180;
            else if (valueText.Orietation < -90)
                valueText.Orietation += 180;
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
                if (Dot(v1, this.TextReferP - (this.Startpn + this.Endpn) / 2.0) >= 0)
                {
                    p1 = this.Startpn;
                    p2 = this.Startpn + v1 * ((Norm(v1) + this.LeaderLength) / Norm(v1));
                }
                else
                {
                    p1 = this.Endpn;
                    p2 = this.Endpn - v1 * ((Norm(v1) + this.LeaderLength) / Norm(v1));
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
                if (Norm(Endpn - Startpn) / 2.0 > Norm(TextReferP - (Endpn + Startpn) / 2.0))
                {
                    PlotTriangle(p1, p2, p3, p4, theta, this.Color);
                    p4 = this.Endpn;
                    PlotTriangle(p1, p2, p3, p4, theta + 180, this.Color);
                }
                else
                {
                    PlotTriangle(p1, p2, p3, p4, theta + 180, this.Color);
                    p4 = this.Endpn;
                    PlotTriangle(p1, p2, p3, p4, theta, this.Color);
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
            this.TextReferP.Transform(TransformMatrix);
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            return new netDxf.Entities.DiametricDimension(
                ((this.Startpn + this.Endpn) / 2.0).ToDXFVertex3(),
                this.Endpn.ToDXFVertex3(),
                this.LeaderLength)
            {
                Color = this._color.A != 0? new netDxf.AciColor(this.Color) : netDxf.AciColor.ByLayer,
            };
        }
    }
}
