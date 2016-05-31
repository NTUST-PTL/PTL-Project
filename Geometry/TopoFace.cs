using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGL.OpenGL;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Geometry
{
    public class TopoFace : SurfaceEntity, IToDXFEntity, IToDXFEntities
    {
        public XYZ4[,] Points;
        public XYZ3[,] Normals;
        public int Dim1Length { get { return Points.GetLength(0); } }
        public int Dim2Length { get { return Points.GetLength(1); } }
        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = Dot(externalCoordinateMatrix, this.CoordinateSystem);

            XYZ4[] boundary;
            boundary = new XYZ4[2] { Transport4(M, this.Points[0, 0]), Transport4(M, this.Points[0, 0]) };
            foreach (XYZ4 p in this.Points)
                Compare_Boundary(boundary, Transport4(M, p));
            return boundary;
        }
        public bool NormalVisible = false;

        public TopoFace()
        {
        }

        public void SolveNormalVector()
        {
            if (Points != null && Points.Length >= 4)
            {
                Normals = new XYZ3[Points.GetLength(0), Points.GetLength(1)];
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
                if (NormalVisible)
                    PlotNormal();
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.Surface || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    this.PlotFace();
                }
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.Mesh || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndMesh)
                {
                    this.PlotMesh();
                }
                if (this.SurfaceDisplayOption == SurfaceDisplayOptions.Edge || this.SurfaceDisplayOption == SurfaceDisplayOptions.SurfaceAndEdge)
                {
                    this.PlotEdge();
                }
                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        protected void PlotEdge()
        {
            GL.glDisable(GL.GL_CULL_FACE);//關閉表面剔除選項
            GL.glColor4d(this.Color.R / 255.0, this.Color.G / 255.0, this.Color.B / 255.0, this.Color.A / 255.0); //顏色

            int lastRow = Points.GetUpperBound(0);
            int lastCol = Points.GetUpperBound(1);

            for (int i = 0; i < (Points.GetLength(0)); i++)
            {
                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                GL.glVertex3d(Points[i, 0].X,
                                Points[i, 0].Y,
                                Points[i, 0].Z);
                GL.glEnd();
            }
            for (int i = 0; i < (Points.GetLength(0)); i++)
            {
                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                GL.glVertex3d(Points[i, lastCol].X,
                                Points[i, lastCol].Y,
                                Points[i, lastCol].Z);
                GL.glEnd();
            }
            for (int j = 0; j < (Points.GetLength(1)); j++)
            {
                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                GL.glVertex3d(Points[0, j].X,
                                Points[0, j].Y,
                                Points[0, j].Z);
                GL.glEnd();
            }
            for (int j = 0; j < (Points.GetLength(1)); j++)
            {
                //Draw Line
                GL.glBegin(GL.GL_LINE_STRIP);
                GL.glVertex3d(Points[lastRow, j].X,
                                Points[lastRow, j].Y,
                                Points[lastRow, j].Z);
                GL.glEnd();
            }
        }

        protected void PlotMesh()
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

        protected void PlotNormal()
        {
            //畫法向量
            glColor4d(System.Drawing.Color.LawnGreen);
            for (int i = 0; i < Points.GetLength(0); i++)
            {
                for (int j = 0; j < Points.GetLength(1); j++)
                {
                    PointD p1 = Points[i, j];
                    PointD p2 = Points[i, j] + Normals[i, j];
                    GL.glBegin(GL.GL_LINE_STRIP);
                    GL.glVertex3d(p1.X, p1.Y, p1.Z);
                    GL.glVertex3d(p2.X, p2.Y, p2.Z);
                    GL.glEnd();
                }
            }
        }

        protected void PlotFace()
        {
            GL.glDisable(GL.GL_CULL_FACE);//關閉表面剔除選項

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
                            new netDxf.Vector3(this.Points[i, j].Values),
                            new netDxf.Vector3((this.Points[i, j] + this.Normals[i, j] * 0.5).Values));
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
                newTopoFace.Points = new XYZ4[this.Points.GetLength(0), this.Points.GetLength(1)];
                for (int i = 0; i < this.Points.GetLength(0); i++)
                {
                    for (int j = 0; j < this.Points.GetLength(1); j++)
                    {
                        if (this.Points[i, j] != null)
                            newTopoFace.Points[i, j] = (XYZ4)this.Points[i, j].Clone();
                    }
                }
            }
            if (this.Normals != null)
            {
                newTopoFace.Normals = new XYZ3[this.Normals.GetLength(0), this.Normals.GetLength(1)];
                for (int i = 0; i < this.Normals.GetLength(0); i++)
                {
                    for (int j = 0; j < this.Normals.GetLength(1); j++)
                    {
                        if (this.Normals[i, j] != null)
                            newTopoFace.Normals[i, j] = this.Normals[i, j].Clone() as XYZ3;
                    }
                }
            }
            return newTopoFace;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            for (int i = 0; i < Dim1Length; i++)
                for (int j = 0; j < Dim2Length; j++)
                    this.Points[i, j] = Transport4(TransformMatrix, this.Points[i, j]);
            if (this.Normals != null)
            {
                for (int i = 0; i < Dim1Length; i++)
                    for (int j = 0; j < Dim2Length; j++)
                        this.Normals[i, j] = Transport3(TransformMatrix, this.Normals[i, j]);
            }
        }

        public double[][,][] ToDoubleArray()
        {
            double[,][] pointArray = new double[this.Points.GetLength(0), this.Points.GetLength(1)][];
            for (int i = 0; i < this.Points.GetLength(0); i++)
            {
                for (int j = 0; j < this.Points.GetLength(1); j++)
                {
                    pointArray[i, j] = this.Points[i, j];
                }
            }

            double[,][] normalArray = new double[this.Normals.GetLength(0), this.Normals.GetLength(1)][];
            for (int i = 0; i < this.Normals.GetLength(0); i++)
            {
                for (int j = 0; j < this.Normals.GetLength(1); j++)
                {
                    normalArray[i, j] = this.Normals[i, j];
                }
            }

            return new double[][,][] { pointArray, normalArray };
        }
    }
}
