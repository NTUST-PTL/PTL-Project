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

namespace PTL.Geometry
{
    public class Triangle : SurfaceEntity
    {
        public PointD P1, P2, P3;
        public Vector N1, N2, N3;

        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary;
                if (this.CoordinateSystem != null)
                {
                    boundary = new PointD[2] { MatrixDot4(this.CoordinateSystem, P1), MatrixDot4(this.CoordinateSystem, P1) };
                    Compare_Boundary(boundary, MatrixDot4(this.CoordinateSystem, P2));
                    Compare_Boundary(boundary, MatrixDot4(this.CoordinateSystem, P3));
                }
                else
                {
                    boundary = new PointD[2] { (P1.Clone()) as PointD, (P1.Clone()) as PointD };
                    Compare_Boundary(boundary, P2);
                    Compare_Boundary(boundary, P3);
                }
                return boundary;
            }
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
                //顏色
                GL.glColor4d(this.Color.R / 255.0, this.Color.G / 255.0, this.Color.B / 255.0, this.Color.A / 255.0);

                //面
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceOnly || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    GL.glBegin(GL.GL_TRIANGLES); //三角面起點
                    if (N1 != null)
                        GL.glNormal3d(N1.X,
                                      N1.Y,
                                      N1.Z);//頂點法向量
                    GL.glVertex3d(P1.X,
                                  P1.Y,
                                  P1.Z);
                    if (N2 != null)
                        GL.glNormal3d(N2.X,
                                      N2.Y,
                                      N2.Z);//頂點法向量
                    GL.glVertex3d(P2.X,
                                  P2.Y,
                                  P2.Z);
                    if (N3 != null)
                        GL.glNormal3d(N3.X,
                                      N3.Y,
                                      N3.Z);//頂點法向量
                    GL.glVertex3d(P3.X,
                                  P3.Y,
                                  P3.Z);
                    GL.glEnd(); //三角面終點
                }
                //邊線
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.EdgeOnly || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    GL.glBegin(GL.GL_LINE_LOOP);
                    GL.glVertex3d(P1.X,
                                  P1.Y,
                                  P1.Z);
                    GL.glVertex3d(P2.X,
                                  P2.Y,
                                  P2.Z);
                    GL.glVertex3d(P3.X,
                                  P3.Y,
                                  P3.Z);
                    GL.glEnd();
                }

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override object Clone()
        {
            Triangle newTriangle = new Triangle()
            {
                Color = this._color,
                CoordinateSystem = this.CoordinateSystem,
                Visible = this.Visible
            };
            if (this.P1 != null)
                newTriangle.P1 = this.P1.Clone() as PointD;
            if (this.P2 != null)
                newTriangle.P2 = this.P2.Clone() as PointD;
            if (this.P3 != null)
                newTriangle.P3 = this.P3.Clone() as PointD;
            if (this.N1 != null)
                newTriangle.N1 = this.N1.Clone() as PointD;
            if (this.N2 != null)
                newTriangle.N2 = this.N2.Clone() as PointD;
            if (this.N3 != null)
                newTriangle.N3 = this.N3.Clone() as PointD;

            return newTriangle;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.P1.Transform(TransformMatrix);
            this.P2.Transform(TransformMatrix);
            this.P3.Transform(TransformMatrix);
            if (this.N1 != null) this.N1.Transform(TransformMatrix);
            if (this.N2 != null) this.N2.Transform(TransformMatrix);
            if (this.N3 != null) this.N3.Transform(TransformMatrix);
        }
    }
    public class Cylinder : SurfaceEntity
    {
        public PointD axisStart;
        public PointD axisEnd;
        public double radius;
        public int sliceNumber = 60;

        public PointD AxisStart
        {
            get { return axisStart; }
            set {
                if (this.axisStart != value)
                {
                    this.axisStart = value;
                }
            }
        }
        public PointD AxisEnd
        {
            get { return axisEnd; }
            set
            {
                if (this.axisEnd != value)
                {
                    this.axisEnd = value;
                }
            }
        }
        public double Radius
        {
            get { return radius; }
            set
            {
                if (this.radius != value)
                {
                    this.radius = value;
                }
            }
        }
        public int SliceNumber
        {
            get { return sliceNumber; }
            set {
                if (this.sliceNumber != value)
                {
                    this.sliceNumber = value;
                }
            }
        }

        TopoFace face;

        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary;

                PointD p1 = TransformPoint(this.CoordinateSystem, this.AxisStart);
                PointD p2 = TransformPoint(this.CoordinateSystem, this.AxisEnd);
                Vector N1 = Cross(Cross(new Vector(0, 0, 1), p2 - p1), p2 - p1);
                Vector N2 = Cross(N1, p2 - p1);
                List<PointD> pp = new List<PointD>();
                pp.Add(p1 + N1);
                pp.Add(p1 - N1);
                pp.Add(p1 + N2);
                pp.Add(p1 - N2);
                pp.Add(p2 + N1);
                pp.Add(p2 - N1);
                pp.Add(p2 + N2);
                pp.Add(p2 - N2);

                boundary = new PointD[2] { MatrixDot4(this.CoordinateSystem, pp[0]), MatrixDot4(this.CoordinateSystem, pp[0]) };
                for (int i = 1; i < pp.Count; i++)
                    Compare_Boundary(boundary, MatrixDot4(this.CoordinateSystem, pp[i]));
                
                return boundary;
            }
        }

        public void RenderGeometry()
        {
            if (this.axisStart != null && axisEnd != null && this.radius != 0 && this.sliceNumber != 0)
            {
                this.face = new TopoFace() { Points = new PointD[2, sliceNumber], Normals = new Vector[2, sliceNumber]};

                Vector axisDirection = AxisEnd - AxisStart;
                Vector N1 = Cross(new Vector(0, 0, 1), axisDirection);
                if (Norm(N1) == 0)
                    N1 = Cross(new Vector(0, 1, 0), axisDirection);
                N1 = Normalize(N1);

                double dTheta = PI * 2.0 / (sliceNumber - 1);
                double[,] M = IdentityMatrix(4);

                for (int i = 0; i < sliceNumber; i++)
                {
                    M = RotateMatrix(axisDirection, dTheta * i);
                    Vector n = RotatePoint(M, N1);
                    Vector r = n * this.Radius;

                    this.face.Normals[0, i] = n;
                    this.face.Normals[1, i] = n;
                    this.face.Points[0, i] = AxisStart + r;
                    this.face.Points[1, i] = AxisEnd + r;
                }
            }
        }

        public override void PlotInOpenGL()
        {
            if (this.Visible == true && this.face != null)
            {
                if (this.CoordinateSystem != null)
                {
                    GL.glPushMatrix();
                    GL.glMatrixMode(GL.GL_MODELVIEW);
                    MultMatrixd(this.CoordinateSystem);
                }

                //面
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceOnly || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    if (this.face.Color != this.Color)
                      this.face.Color = this.Color;
                    this.face.PlotInOpenGL();
                }
                //邊線
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.EdgeOnly || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    
                }

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override object Clone()
        {
            return new Cylinder()
            {
                AxisStart = this.AxisStart,
                AxisEnd = this.AxisEnd,
                Radius = this.Radius,
                sliceNumber = this.sliceNumber,
                Color = this._color
            };
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.AxisStart.Transform(TransformMatrix);
            this.AxisEnd.Transform(TransformMatrix);
        }
    }
    public class TopoFace : SurfaceEntity,IToDXFEntity , IToDXFEntities
    {
        public PointD[,] Points;
        public Vector[,] Normals;
        public int Dim1Length { get { return Points.GetLength(0); } }
        public int Dim2Length { get { return Points.GetLength(1); } }
        public override PointD[] Boundary
        {
            get
            {
                PointD[] boundary;
                if (this.CoordinateSystem != null)
                {
                    boundary = new PointD[2] { MatrixDot4(this.CoordinateSystem, this.Points[0, 0]), MatrixDot4(this.CoordinateSystem, this.Points[0, 0]) };
                    foreach (PointD p in this.Points)
                        Compare_Boundary(boundary, MatrixDot4(this.CoordinateSystem, p));
                }
                else
                {
                    boundary = new PointD[2] { this.Points[0, 0].Clone() as PointD, this.Points[0, 0].Clone() as PointD };
                    foreach (PointD p in this.Points)
                        Compare_Boundary(boundary, p);
                }
                return boundary;
            }
        }

        public TopoFace()
        {
        }

        public void SovleNormalVector()
        {
            if (Points != null && Points.Length >= 4)
            {
                Normals = new Vector[Points.GetLength(0), Points.GetLength(1)];
                double a = 0, b = 0, c = 0, m = 0, n = 0, l = 0;
                for (int i = 0; i < Points.GetLength(0); i++)
                {
                    for (int j = 0; j < Points.GetLength(1); j++)
                    {
                        if (i == Points.GetLength(0) - 1)
                        {
                            if (j == Points.GetLength(1) - 1)
                            {
                                a = Points[i, j].X - Points[i - 1, j].X;
                                b = Points[i, j].Y - Points[i - 1, j].Y;
                                c = Points[i, j].Z - Points[i - 1, j].Z;
                                m = Points[i, j].X - Points[i, j - 1].X;
                                n = Points[i, j].Y - Points[i, j - 1].Y;
                                l = Points[i, j].Z - Points[i, j - 1].Z;
                            }
                            else
                            {
                                a = Points[i, j].X - Points[i - 1, j].X;
                                b = Points[i, j].Y - Points[i - 1, j].Y;
                                c = Points[i, j].Z - Points[i - 1, j].Z;
                                m = Points[i, j + 1].X - Points[i, j].X;
                                n = Points[i, j + 1].Y - Points[i, j].Y;
                                l = Points[i, j + 1].Z - Points[i, j].Z;
                            }
                        }
                        else
                        {
                            if (j == Points.GetLength(1) - 1)
                            {
                                a = Points[i + 1, j].X - Points[i, j].X;
                                b = Points[i + 1, j].Y - Points[i, j].Y;
                                c = Points[i + 1, j].Z - Points[i, j].Z;
                                m = Points[i, j].X - Points[i, j - 1].X;
                                n = Points[i, j].Y - Points[i, j - 1].Y;
                                l = Points[i, j].Z - Points[i, j - 1].Z;
                            }
                            else
                            {
                                a = Points[i + 1, j].X - Points[i, j].X;
                                b = Points[i + 1, j].Y - Points[i, j].Y;
                                c = Points[i + 1, j].Z - Points[i, j].Z;
                                m = Points[i, j + 1].X - Points[i, j].X;
                                n = Points[i, j + 1].Y - Points[i, j].Y;
                                l = Points[i, j + 1].Z - Points[i, j].Z;
                            }
                        }
                        Normals[i, j] = Cross(new PointD(a, b, c), new PointD(m, n, l));
                    }
                }
            }

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

                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceOnly || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    this.PlotFace();
                }
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.EdgeOnly || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    this.PlotLineStructure();
                }

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        protected void PlotLineStructure()
        {
            GL.glDisable(GL.GL_CULL_FACE);//關閉表面剔除選項
            GL.glColor4d(this.Color.R / 255.0, this.Color.G / 255.0, this.Color.B / 255.0, this.Color.A / 255.0); //顏色
            for (int i = 0; i < (Points.GetLength(0)); i++)
            {
                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                for (int j = 0; j < (Points.GetLength(1)); j++)
                {
                    GL.glVertex3d(Points[i, j].X,
                                  Points[i, j].Y,
                                  Points[i, j].Z);
                }
                GL.glEnd();
            }
            for (int j = 0; j < (Points.GetLength(1)); j++)
            {
                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                for (int i = 0; i < (Points.GetLength(0)); i++)
                {
                    GL.glVertex3d(Points[i, j].X,
                                  Points[i, j].Y,
                                  Points[i, j].Z);
                }
                GL.glEnd();
            }
        }

        protected void PlotFace()
        {
            GL.glDisable(GL.GL_CULL_FACE);//關閉表面剔除選項

            //畫法向量
            //glColor4d(System.Drawing.Color.LawnGreen);
            //for (int i = 0; i < Points.GetLength(0); i++)
            //{
            //    for (int j = 0; j < Points.GetLength(1); j++)
            //    {
            //        PointD p1 = Points[i, j];
            //        PointD p2 = Points[i, j] + Normals[i, j];
            //        GL.glBegin(GL.GL_LINE_STRIP);
            //        GL.glVertex3d(p1.X, p1.Y, p1.Z);
            //        GL.glVertex3d(p2.X, p2.Y, p2.Z);
            //        GL.glEnd();
            //    }
            //}

            GL.glColor4d(this.Color.R / 255.0, this.Color.G / 255.0, this.Color.B / 255.0, this.Color.A / 255.0); //顏色
            for (int i = 0; i < (Points.GetLength(0) - 1); i++)
            {
                GL.glBegin(GL.GL_TRIANGLE_STRIP); //三角面起點
                for (int j = 0; j < Points.GetLength(1); j++)
                {
                    if (Normals[i, j] != null)
                        GL.glNormal3d(Normals[i, j].X,
                                      Normals[i, j].Y,
                                      Normals[i, j].Z);//頂點法向量
                    GL.glVertex3d(Points[i, j].X,
                                  Points[i, j].Y,
                                  Points[i, j].Z);
                    if (Normals[i + 1, j] != null)
                        GL.glNormal3d(Normals[i + 1, j].X,
                                      Normals[i + 1, j].Y,
                                      Normals[i + 1, j].Z);//頂點法向量
                    GL.glVertex3d(Points[i + 1, j].X,
                                  Points[i + 1, j].Y,
                                  Points[i + 1, j].Z);
                }
                GL.glEnd();
            }

            
            
        }

        public netDxf.Entities.EntityObject ToDXFEntity()
        {
            Console.WriteLine("Function \"netDxf.Entities.EntityObject TopoFace.ToDXFEntity()\" not implemented!");
            return null;
        }

        public List<netDxf.Entities.EntityObject> ToDXFEntities()
        {
            List<netDxf.Entities.EntityObject> entityList = new List<netDxf.Entities.EntityObject>();

            //橫線
            for (int i = 0; i < this.Dim1Length; i++)
            {
                List<netDxf.Entities.SplineVertex> controlPoints = new List<netDxf.Entities.SplineVertex>();
                for (int j = 0; j < this.Dim2Length; j++)
                    controlPoints.Add(
                        new netDxf.Entities.SplineVertex(
                            this.Points[i, j].X,
                            this.Points[i, j].Y,
                            this.Points[i, j].Z
                            ));
                netDxf.Entities.Spline spline = new netDxf.Entities.Spline(controlPoints);
                spline.Color = new netDxf.AciColor(this.Color);
                entityList.Add(spline);
            }

            //縱線
            for (int j = 0; j < this.Dim2Length; j++)
            {
                List<netDxf.Entities.SplineVertex> controlPoints = new List<netDxf.Entities.SplineVertex>();
                for (int i = 0; i < this.Dim1Length; i++)
                    controlPoints.Add(
                        new netDxf.Entities.SplineVertex(
                            this.Points[i, j].X,
                            this.Points[i, j].Y,
                            this.Points[i, j].Z
                            ));
                netDxf.Entities.Spline spline = new netDxf.Entities.Spline(controlPoints);
                spline.Color = new netDxf.AciColor(this.Color);
                entityList.Add(spline);
            }

            //法向線
            for (int i = 0; i < this.Dim1Length; i++)
            {
                for (int j = 0; j < this.Dim2Length; j++)
                {
                    netDxf.Entities.Line line =
                        new netDxf.Entities.Line(
                            new netDxf.Vector3(this.Points[i, j].ToArray()),
                            new netDxf.Vector3((this.Points[i, j] + this.Normals[i, j]*0.5).ToArray()));
                    line.Color = netDxf.AciColor.Red;
                    entityList.Add(line);
                }
            }

            return entityList;
        }

        public override object Clone()
        {
            TopoFace newTopoFace = new TopoFace() { Color = this._color, SurfaceDisplayOption = this.SurfaceDisplayOption };
            if (this.CoordinateSystem != null)
                newTopoFace.CoordinateSystem = this.CoordinateSystem;
            if (this.Points != null)
            {
                newTopoFace.Points = new PointD[this.Points.GetLength(0), this.Points.GetLength(1)];
                for (int i = 0; i < this.Points.GetLength(0); i++)
                {
                    for (int j = 0; j < this.Points.GetLength(1); j++)
                    {
                        if (this.Points[i, j] != null)
                            newTopoFace.Points[i, j] = this.Points[i, j].Clone() as PointD;
                    }
                }
            }
            if (this.Normals != null)
            {
                newTopoFace.Normals = new Vector[this.Normals.GetLength(0), this.Normals.GetLength(1)];
                for (int i = 0; i < this.Normals.GetLength(0); i++)
                {
                    for (int j = 0; j < this.Normals.GetLength(1); j++)
                    {
                        if (this.Normals[i, j] != null)
                            newTopoFace.Normals[i, j] = this.Normals[i, j].Clone() as Vector;
                    }
                }
            }
            return newTopoFace;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.Points = TransformPoints(TransformMatrix, this.Points);
            if (this.Normals != null) this.Normals = TransformPoints(TransformMatrix, this.Normals);
        }

        public double[][,][] ToDoubleArray()
        {
            double[,][] pointArray = new double[this.Points.GetLength(0), this.Points.GetLength(1)][];
            for (int i = 0; i < this.Points.GetLength(0); i++)
            {
                for (int j = 0; j < this.Points.GetLength(1); j++)
                {
                    pointArray[i, j] = this.Points[i, j].ToArray();
                }
            }

            double[,][] normalArray = new double[this.Normals.GetLength(0), this.Normals.GetLength(1)][];
            for (int i = 0; i < this.Normals.GetLength(0); i++)
            {
                for (int j = 0; j < this.Normals.GetLength(1); j++)
                {
                    normalArray[i, j] = this.Normals[i, j].ToArray();
                }
            }

            return new double[][,][] { pointArray, normalArray };
        }

        
    }
}
