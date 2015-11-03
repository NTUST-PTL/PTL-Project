using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGL.OpenGL;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class Triangle : SurfaceEntity
    {
        public XYZ4 P1, P2, P3;
        public XYZ3 N1, N2, N3;

        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                if (this.CoordinateSystem != null)
                {
                    boundary = new XYZ4[2] { Transport(this.CoordinateSystem, P1), Transport(this.CoordinateSystem, P1) };
                    Compare_Boundary(boundary, Transport(this.CoordinateSystem, P2));
                    Compare_Boundary(boundary, Transport(this.CoordinateSystem, P3));
                }
                else
                {
                    boundary = new XYZ4[2] { (XYZ4)(P1.Clone()), (XYZ4)(P1.Clone()) };
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
                newTriangle.P1 = this.P1.Clone() as XYZ4;
            if (this.P2 != null)
                newTriangle.P2 = this.P2.Clone() as XYZ4;
            if (this.P3 != null)
                newTriangle.P3 = this.P3.Clone() as XYZ4;
            if (this.N1 != null)
                newTriangle.N1 = this.N1.Clone() as XYZ3;
            if (this.N2 != null)
                newTriangle.N2 = this.N2.Clone() as XYZ3;
            if (this.N3 != null)
                newTriangle.N3 = this.N3.Clone() as XYZ3;

            return newTriangle;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            if (this.P1 != null) this.P1 = Transport(TransformMatrix, this.P1);
            if (this.P2 != null) this.P2 = Transport(TransformMatrix, this.P2);
            if (this.P3 != null) this.P3 = Transport(TransformMatrix, this.P3);
            if (this.N1 != null) this.N1 = Transport(TransformMatrix, this.N1);
            if (this.N2 != null) this.N2 = Transport(TransformMatrix, this.N2);
            if (this.N3 != null) this.N3 = Transport(TransformMatrix, this.N3);
        }
    }
}
