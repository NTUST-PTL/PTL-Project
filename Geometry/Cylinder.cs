using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGL.OpenGL;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry
{
    public class Cylinder : SurfaceEntity
    {
        public PointD axisStart;
        public PointD axisEnd;
        public double radius;
        public int sliceNumber = 60;

        public PointD AxisStart
        {
            get { return axisStart; }
            set
            {
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
            set
            {
                if (this.sliceNumber != value)
                {
                    this.sliceNumber = value;
                }
            }
        }

        TopoFace face;

        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            return this.face.GetBoundary(Dot(externalCoordinateMatrix, CoordinateSystem));
        }

        public void RenderGeometry()
        {
            if (this.axisStart != null && axisEnd != null && this.radius != 0 && this.sliceNumber != 0)
            {
                this.face = new TopoFace(2, sliceNumber);

                VectorD axisDirection = AxisEnd - AxisStart;
                VectorD N1 = Cross(new VectorD(0, 0, 1), axisDirection);
                if (Norm(N1) == 0)
                    N1 = Cross(new VectorD(0, 1, 0), axisDirection);
                N1 = Normalize(N1);

                double dTheta = PI * 2.0 / (sliceNumber - 1);
                double[,] M = NewIdentityMatrix(4);

                for (int i = 0; i < sliceNumber; i++)
                {
                    M = NewRotateMatrix4(axisDirection, dTheta * i);
                    VectorD n = Transport4(M, N1);
                    VectorD r = n * this.Radius;

                    this.face.Normals[0, i] = n;
                    this.face.Normals[1, i] = n;
                    this.face.Points[0, i] = AxisStart + r;
                    this.face.Points[1, i] = AxisEnd + r;
                }
            }
        }

        public override void PlotInOpenGL()
        {
            this.face.Color = this.Color;
            switch (SurfaceDisplayOption)
            {
                case SurfaceDisplayOptions.Null:
                    break;
                case SurfaceDisplayOptions.Surface:
                    PlotFace();
                    break;
                case SurfaceDisplayOptions.Mesh:
                    PlotMesh();
                    break;
                case SurfaceDisplayOptions.Edge:
                    PlotEdge();
                    break;
                case SurfaceDisplayOptions.SurfaceAndEdge:
                    PlotFace();
                    PlotEdge();
                    break;
                case SurfaceDisplayOptions.SurfaceAndMesh:
                    PlotFace();
                    PlotMesh();
                    break;
                default:
                    break;
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

        public override void PlotEdge()
        {
            GL.glDisable(GL.GL_CULL_FACE);//關閉表面剔除選項
            GL.glColor4d(this.Color.R / 255.0, this.Color.G / 255.0, this.Color.B / 255.0, this.Color.A / 255.0); //顏色

            int lastRow = face.Points.GetUpperBound(0);
            int lastCol = face.Points.GetUpperBound(1);

            //Draw Line
            GL.glBegin(GL.GL_LINE_STRIP);
            for (int j = 0; j < (face.Points.GetLength(1)); j++)
            {
                GL.glVertex3d(face.Points[0, j].X,
                                face.Points[0, j].Y,
                                face.Points[0, j].Z);
            }
            GL.glEnd();

            //Draw Line
            GL.glBegin(GL.GL_LINE_STRIP);
            for (int j = 0; j < (face.Points.GetLength(1)); j++)
            {
                GL.glVertex3d(face.Points[lastRow, j].X,
                                face.Points[lastRow, j].Y,
                                face.Points[lastRow, j].Z);
            }
            GL.glEnd();
        }

        public override void PlotMesh()
        {
            face?.PlotMesh();
        }

        public override void PlotNormal()
        {
            face?.PlotNormal();
        }

        public override void PlotFace()
        {
            face?.PlotFace();
        }
    }
}
