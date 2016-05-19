using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;
using CsGL.OpenGL;
using System.Collections.ObjectModel;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Geometry
{
    public class PolyFan : SurfaceEntity
    {
        protected ObservableCollection<XYZ4> _Points = new ObservableCollection<XYZ4>();
        protected ObservableCollection<XYZ3> _Normals = new ObservableCollection<XYZ3>();

        public ObservableCollection<XYZ4> Points
        {
            get { return _Points; }
            set
            {
                if (_Points != value)
                {
                    _Points = value;
                    NotifyChanged(nameof(Points));
                }
            }
        }
        public ObservableCollection<XYZ3> Normals
        {
            get { return _Normals; }
            set
            {
                if (_Normals != value)
                {
                    _Normals = value;
                    NotifyChanged(nameof(Normals));
                }
            }
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

        public override void PlotInOpenGL()
        {
            PlotFace();
        }

        protected void PlotFace()
        {
            GL.glDisable(GL.GL_CULL_FACE);//關閉表面剔除選項

            GL.glColor4d(this.Color.R / 255.0, this.Color.G / 255.0, this.Color.B / 255.0, this.Color.A / 255.0); //顏色
            GL.glBegin(GL.GL_TRIANGLE_FAN); //三角面起點
            GL.glVertex3d(Points[0].X,
                          Points[0].Y,
                          Points[0].Z);
            GL.glVertex3d(Points[1].X,
                          Points[1].Y,
                          Points[1].Z);
            for (int i = 2; i < Points.Count; i++)
            {
                GL.glNormal3d(Normals[i-2].X,
                              Normals[i-2].Y,
                              Normals[i-2].Z);//頂點法向量
                GL.glVertex3d(Points[i].X,
                              Points[i].Y,
                              Points[i].Z);
            }
            GL.glEnd();
        }

        public void SolveNormalVectors(bool reverseDirection = false)
        {
            if (Normals != null)
                Normals.Clear();
            else
                Normals = new ObservableCollection<XYZ3>();

            for (int i = 2; i < Points.Count; i++)
            {
                XYZ3 v1 = Points[i-1] - Points.First();
                XYZ3 v2 = Points[i] - Points.First();
                XYZ3 n = reverseDirection? Cross(v1, v2) : Cross(v2, v1);
                Normals.Add(n);
            }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            ObservableCollection<XYZ4> newPoints = new ObservableCollection<XYZ4>();
            ObservableCollection<XYZ3> newNormals = new ObservableCollection<XYZ3>();

            foreach (var p in Points)
            {
                newPoints.Add(Transport4(TransformMatrix, p));
            }

            foreach (var n in Normals)
            {
                newNormals.Add(Transport3(TransformMatrix, n));
            }

            this.Points = newPoints;
            this.Normals = newNormals;
        }

        public override object Clone()
        {
            PolyFan newFan = new PolyFan
            {
                Color = this.Color,
                CoordinateSystem = this.CoordinateSystem
            };

            foreach (var p in Points)
            {
                newFan.Points.Add((XYZ4)p.Clone());
            }

            foreach (var n in Normals)
            {
                newFan.Normals.Add((XYZ3)n.Clone());
            }

            return newFan;
        }
    }
}
