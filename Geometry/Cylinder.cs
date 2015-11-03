using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGL.OpenGL;
using PTL.Geometry.MathModel;

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

        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;

                PointD p1 = Transport4(this.CoordinateSystem, this.AxisStart);
                PointD p2 = Transport4(this.CoordinateSystem, this.AxisEnd);
                XYZ3 axis = Normalize(p2 - p1);
                Vector Nx = Cross(Cross(new XYZ3(1, 0, 0), axis), axis * radius);
                Vector Ny = Cross(Cross(new XYZ3(0, 1, 0), axis), axis * radius);
                Vector Nz = Cross(Cross(new XYZ3(0, 0, 1), axis), axis * radius);
                List<PointD> pp = new List<PointD>();
                pp.Add(p1 + Nx);
                pp.Add(p1 - Nx);
                pp.Add(p2 + Nx);
                pp.Add(p2 - Nx);
                pp.Add(p1 + Ny);
                pp.Add(p1 - Ny);
                pp.Add(p2 + Ny);
                pp.Add(p2 - Ny);
                pp.Add(p1 + Nz);
                pp.Add(p1 - Nz);
                pp.Add(p2 + Nz);
                pp.Add(p2 - Nz);

                boundary = new XYZ4[2] { Transport4(this.CoordinateSystem, pp[0]), Transport4(this.CoordinateSystem, pp[0]) };
                for (int i = 1; i < pp.Count; i++)
                    Compare_Boundary(boundary, Transport4(this.CoordinateSystem, pp[i]));

                return boundary;
            }
        }

        public void RenderGeometry()
        {
            if (this.axisStart != null && axisEnd != null && this.radius != 0 && this.sliceNumber != 0)
            {
                this.face = new TopoFace() { Points = new XYZ4[2, sliceNumber], Normals = new XYZ3[2, sliceNumber] };

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
                    Vector n = Transport4(M, N1);
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
}
