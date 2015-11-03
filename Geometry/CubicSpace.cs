using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGL.OpenGL;
using PTL.OpenGL.Plot;

namespace PTL.Geometry
{
    public class CubicSpace : ICanPlotInOpenGL, IHaveColor
    {
        private PointD[] _Boundary;
        public PointD[] Boundary
        {
            get { return _Boundary; }
            set { this._Boundary = value; }
        }
        public bool HaveMaterial = false;
        public bool HaveEntity
        {
            get { return EntityNumber > 0; }
        }
        public uint EntityNumber = 0;

        private System.Drawing.Color _Color = System.Drawing.Color.FromArgb(25, 150, 200, 150);
        public System.Drawing.Color Color
        {
            get { return this._Color; }
            set { if (this._Color != value) this._Color = value; }
        }
        private bool? _Visible = false;
        public bool? Visible
        {
            get { return this._Visible; }
            set { if (this._Visible != value) this._Visible = value; }
        }

        public HashSet<Entity> Entities = new HashSet<Entity>();

        public bool Add(Entity Entity)
        {
            bool result = Entities.Add(Entity);
            if (result)
                EntityNumber++;
            return result;
        }

        public bool Remove(Entity Entity)
        {
            bool result = Entities.Remove(Entity);
            if (result)
                EntityNumber--;
            return result;
        }

        public void PlotInOpenGL()
        {
            if (this.Visible == true)
            {
                PlotSub.glColor4d(this.Color);
                #region 上下面
                GL.glBegin(GL.GL_TRIANGLE_STRIP); //三角面起點
                GL.glNormal3d(0, 0, -1);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[0].Y,
                                Boundary[0].Z);
                GL.glNormal3d(0, 0, -1);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[0].Y,
                                Boundary[0].Z);
                GL.glNormal3d(0, 0, -1);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[1].Y,
                                Boundary[0].Z);
                GL.glNormal3d(0, 0, -1);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[1].Y,
                                Boundary[0].Z);
                GL.glEnd();
                GL.glBegin(GL.GL_TRIANGLE_STRIP); //三角面起點
                GL.glNormal3d(0, 0, 1);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[0].Y,
                                Boundary[1].Z);
                GL.glNormal3d(0, 0, 1);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[0].Y,
                                Boundary[1].Z);
                GL.glNormal3d(0, 0, 1);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[1].Y,
                                Boundary[1].Z);
                GL.glNormal3d(0, 0, 1);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[1].Y,
                                Boundary[1].Z);
                GL.glEnd();
                #endregion
                #region 左右面
                GL.glBegin(GL.GL_TRIANGLE_STRIP); //三角面起點
                GL.glNormal3d(-1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[0].Y,
                                Boundary[0].Z);
                GL.glNormal3d(-1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[0].Y,
                                Boundary[1].Z);
                GL.glNormal3d(-1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[1].Y,
                                Boundary[0].Z);
                GL.glNormal3d(-1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[1].Y,
                                Boundary[1].Z);
                GL.glEnd();
                GL.glBegin(GL.GL_TRIANGLE_STRIP); //三角面起點
                GL.glNormal3d(1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[0].Y,
                                Boundary[0].Z);
                GL.glNormal3d(1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[0].Y,
                                Boundary[1].Z);
                GL.glNormal3d(1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[1].Y,
                                Boundary[0].Z);
                GL.glNormal3d(1, 0, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[1].Y,
                                Boundary[1].Z);
                GL.glEnd();
                #endregion
                #region 前後面
                GL.glBegin(GL.GL_TRIANGLE_STRIP); //三角面起點
                GL.glNormal3d(0, -1, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[0].Y,
                                Boundary[0].Z);
                GL.glNormal3d(0, -1, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[0].Y,
                                Boundary[0].Z);
                GL.glNormal3d(0, -1, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[0].Y,
                                Boundary[1].Z);
                GL.glNormal3d(0, -1, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[0].Y,
                                Boundary[1].Z);
                GL.glEnd();
                GL.glBegin(GL.GL_TRIANGLE_STRIP); //三角面起點
                GL.glNormal3d(0, 1, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[1].Y,
                                Boundary[0].Z);
                GL.glNormal3d(0, 1, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[1].Y,
                                Boundary[0].Z);
                GL.glNormal3d(0, 1, 0);//頂點法向量
                GL.glVertex3d(Boundary[0].X,
                                Boundary[1].Y,
                                Boundary[1].Z);
                GL.glNormal3d(0, 1, 0);//頂點法向量
                GL.glVertex3d(Boundary[1].X,
                                Boundary[1].Y,
                                Boundary[1].Z);
                GL.glEnd();
                #endregion
            }
        }
    }
}
