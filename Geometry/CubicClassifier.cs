using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGL.OpenGL;
using PTL.OpenGL.Plot;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class CubicClassifier : PTL.Mathematics.Math, ICanPlotInOpenGL
    {
        private int _n = 100;
        private XYZ4[] _Boundary;
        private double[] _Dimentions;
        private double[] _CubeDimentions;
        private int[] _CubeArrayDimentions;
        private int[] _CubeArrayLastIndex;
        private XYZ4[, ,] _Nodes;
        private CubicSpace[, ,] _CubeSpaces;

        public int n 
        {
            get { return _n; }
            protected set { this._n = value; }
        }
        public XYZ4[] Boundary
        {
            get { return _Boundary; }
            protected set { this._Boundary = value; }
        }
        public double[] Dimentions
        {
            get { return _Dimentions; }
            protected set { this._Dimentions = value; }
        }
        public double[] CubeDimentions
        {
            get { return _CubeDimentions; }
            protected set { this._CubeDimentions = value; }
        }
        public int[] CubeArrayDimentions
        {
            get { return _CubeArrayDimentions; }
            protected set { this._CubeArrayDimentions = value; }
        }
        public int[] CubeArrayLastIndex
        {
            get { return _CubeArrayLastIndex; }
            protected set { this._CubeArrayLastIndex = value; }
        }
        public XYZ4[, ,] Nodes
        {
            get { return _Nodes; }
            protected set { this._Nodes = value; }
        }
        public CubicSpace[, ,] CubeSpaces
        {
            get { return _CubeSpaces; }
            protected set { this._CubeSpaces = value; }
        }

        public void CubicClassify(EntityCollection part)
        {
            this.Boundary = part.Boundary;
            this.Dimentions =  (Boundary[1] - Boundary[0]).Values;
            double longEdgeLength = this.Dimentions.Max();
            double unit = longEdgeLength / this.n;
            this.CubeDimentions = new double[] { unit, unit, unit };
            this.CubeArrayDimentions = new int[]{ 
                (int)System.Math.Ceiling(Dimentions[0] / CubeDimentions[0]), 
                (int)System.Math.Ceiling(Dimentions[1] / CubeDimentions[1]), 
                (int)System.Math.Ceiling(Dimentions[2] / CubeDimentions[2]) };
            this.CubeArrayLastIndex = new int[]{ 
                CubeArrayDimentions[0] - 1, 
                CubeArrayDimentions[1] - 1, 
                CubeArrayDimentions[2] - 1 };
            
            //計算節點
            this.Nodes = new XYZ4[CubeArrayDimentions[0] + 1, CubeArrayDimentions[1] + 1, CubeArrayDimentions[2] + 1];
            for (int i = 0; i < CubeArrayDimentions[0] + 1; i++)
            {
                for (int j = 0; j < CubeArrayDimentions[1] + 1; j++)
                {
                    for (int k = 0; k < CubeArrayDimentions[2] + 1; k++)
                    {
                        this.Nodes[i, j, k] = new PointD()
                        {
                            X = Boundary[0].X + CubeDimentions[0] *i,
                            Y = Boundary[0].Y + CubeDimentions[1] * j,
                            Z = Boundary[0].Z + CubeDimentions[2] * k
                        };
                    }
                }
            }
            //建立方塊
            this.CubeSpaces = new CubicSpace[CubeArrayDimentions[0], CubeArrayDimentions[1], CubeArrayDimentions[2]];
            for (int i = 0; i < CubeArrayDimentions[0]; i++)
            {
                for (int j = 0; j < CubeArrayDimentions[1]; j++)
                {
                    for (int k = 0; k < CubeArrayDimentions[2]; k++)
                    {
                        CubeSpaces[i, j, k] = new CubicSpace() { Boundary = new PointD[] { this.Nodes[i, j, k], this.Nodes[i+1, j+1, k+1] } };
                    }
                }
            }

            foreach (Entity entity in part.Entities.Values)
            {
                XYZ4[] boundary = entity.Boundary;
                int[] startCubeIndex = GetCubicSpaceIndex(boundary[0]);
                int[] endCubeIndex = GetCubicSpaceIndex(boundary[1]);

                for (int i = startCubeIndex[0]; i <= endCubeIndex[0]; i++)
                {
                    for (int j = startCubeIndex[1]; j <= endCubeIndex[1]; j++)
                    {
                        for (int k = startCubeIndex[2]; k <= endCubeIndex[2]; k++)
                        {
                            CubeSpaces[i, j, k].Add(entity);
                        }
                    }
                }
            }
        }

        public XYZ4 GetStanderCoordinate(XYZ4 p)
        {
            XYZ4 p1 = p - this.Boundary[0];
            //if (p1.Z / CubeDimentions[2] >= CubeArrayDimentions[2])
            //{
            //    int a = 0;
            //}
            return new PointD(
                p1.X / CubeDimentions[0],
                p1.Y / CubeDimentions[1],
                p1.Z / CubeDimentions[2]);
            
        }

        public int[] GetCubicSpaceIndex(PointD p)
        {
            PointD p1 = GetStanderCoordinate(p);
            int[] index = new int[]{ 
                (int)p1.X, 
                (int)p1.Y, 
                (int)p1.Z };
            if (p1.X == CubeArrayDimentions[0])
                index[0] -= 1;
            if (p1.Y == CubeArrayDimentions[1])
                index[1] -= 1;
            if (p1.Z == CubeArrayDimentions[2])
                index[2] -= 1;
            
            return index;
        }

        public Vector GetStanderVector(Vector v)
        {
            return new Vector(
                v.X / CubeDimentions[0],
                v.Y / CubeDimentions[1],
                v.Z / CubeDimentions[2]);
        }

        public CubicSpace GetCubicSpace(PointD p)
        {
            int[] index = GetCubicSpaceIndex(p);
            return GetCubicSpace(index);
        }

        public CubicSpace GetCubicSpace(params int[] index)
        {
            if (index[0] > 0 && index[0] < CubeArrayDimentions[0]
             && index[1] > 0 && index[1] < CubeArrayDimentions[1]
             && index[2] > 0 && index[2] < CubeArrayDimentions[2])
            {
                return CubeSpaces[index[0], index[1], index[2]];
            }
            else
                return null;
        }

        //public HashSet<CubicSpace> GetCubicSpaces(PointD position, Vector direction)
        //{
        //    PointD ssP = GetStanderCoordinate(position);
        //    Vector ssD = Normalize(GetStanderVector(direction));
        //    HashSet<CubicSpace> IntersectedCubicSpace = new HashSet<CubicSpace>();
        //    int[] Dquadrant = Sign(direction.ToArray());
        //    int[] startIndex = GetCubicSpaceIndex(position);
        //    int[] nextIndex = GetCubicSpaceIndex(position);
        //    PointD nextsP = ssP.Clone();
        //    if (!(startIndex[0] < 0 || startIndex[0] >= CubeArrayDimentions[0]
        //        || startIndex[1] < 0 || startIndex[1] >= CubeArrayDimentions[1]
        //        || startIndex[2] < 0 || startIndex[2] >= CubeArrayDimentions[2]))
        //        IntersectedCubicSpace.Add(this.GetCubicSpace(startIndex));

        //    bool end = false;
        //    while (!end)
        //    {
        //        //List<iv> rate6Org = new List<iv>{
        //        //    new iv(0, nextIndex[0] - nextsP.X),
        //        //    new iv(1, nextIndex[1] - nextsP.Y),
        //        //    new iv(2, nextIndex[2] - nextsP.Z),
        //        //    new iv(3, nextIndex[0] + 1 - nextsP.X),
        //        //    new iv(4, nextIndex[1] + 1 - nextsP.Y),
        //        //    new iv(5, nextIndex[2] + 1 - nextsP.Z)
        //        //};
        //        List<iv> rate6 = new List<iv>{
        //            new iv(0, nextIndex[0] - nextsP.X),
        //            new iv(1, nextIndex[1] - nextsP.Y),
        //            new iv(2, nextIndex[2] - nextsP.Z),
        //            new iv(3, nextIndex[0] + 1 - nextsP.X),
        //            new iv(4, nextIndex[1] + 1 - nextsP.Y),
        //            new iv(5, nextIndex[2] + 1 - nextsP.Z)
        //        };
        //        rate6[0].V = ssD.X != 0 ? rate6[0].V / ssD.X : double.NegativeInfinity;
        //        rate6[1].V = ssD.Y != 0 ? rate6[1].V / ssD.Y : double.NegativeInfinity;
        //        rate6[2].V = ssD.Z != 0 ? rate6[2].V / ssD.Z : double.NegativeInfinity;
        //        rate6[3].V = ssD.X != 0 ? rate6[3].V / ssD.X : double.NegativeInfinity;
        //        rate6[4].V = ssD.Y != 0 ? rate6[4].V / ssD.Y : double.NegativeInfinity;
        //        rate6[5].V = ssD.Z != 0 ? rate6[5].V / ssD.Z : double.NegativeInfinity;
        //        List<iv> rate26 = new List<iv>{
        //            new iv(0, nextIndex[0] - 1 - nextsP.X),
        //            new iv(1, nextIndex[1] - 1 - nextsP.Y),
        //            new iv(2, nextIndex[2] - 1 - nextsP.Z),
        //            new iv(3, nextIndex[0] + 2 - nextsP.X),
        //            new iv(4, nextIndex[1] + 2 - nextsP.Y),
        //            new iv(5, nextIndex[2] + 2 - nextsP.Z)
        //        };
        //        rate26[0].V = ssD.X != 0 ? rate26[0].V / ssD.X : double.NegativeInfinity;
        //        rate26[1].V = ssD.Y != 0 ? rate26[1].V / ssD.Y : double.NegativeInfinity;
        //        rate26[2].V = ssD.Z != 0 ? rate26[2].V / ssD.Z : double.NegativeInfinity;
        //        rate26[3].V = ssD.X != 0 ? rate26[3].V / ssD.X : double.NegativeInfinity;
        //        rate26[4].V = ssD.Y != 0 ? rate26[4].V / ssD.Y : double.NegativeInfinity;
        //        rate26[5].V = ssD.Z != 0 ? rate26[5].V / ssD.Z : double.NegativeInfinity;

        //        var rate =
        //            (from v in rate6
        //             where v.V >= 0
        //             orderby v.V
        //             select v).ToList();
        //        var rate2 =
        //            (from v in rate26
        //             where v.V >= 0
        //             orderby v.V
        //             select v).ToList();


        //        //int[] lastIndex = (int[])ArrayTake(nextIndex, new int[] { 0 }, new int[] { 2 });
        //        //PointD lastSP = nextsP.Clone();


        //        nextIndex[rate[0].i % 3] += Dquadrant[rate[0].i % 3];
        //        double vRate = rate[1].V < rate2[0].V ? rate[1].V : (rate[0].V + rate2[0].V) / 2.0;
        //        nextsP = nextsP + ssD * vRate;
        //        //if (!(nextIndex[0] <= nextsP.X && nextIndex[0] + 1 >= nextsP.X
        //        //&& nextIndex[1] <= nextsP.Y && nextIndex[1] + 1 >= nextsP.Y
        //        //&& nextIndex[2] <= nextsP.Z && nextIndex[2] + 1 >= nextsP.Z))
        //        //{

        //        //}

        //        if (!(nextIndex[0] < 0 || nextIndex[0] >= CubeArrayDimentions[0]
        //        || nextIndex[1] < 0 || nextIndex[1] >= CubeArrayDimentions[1]
        //        || nextIndex[2] < 0 || nextIndex[2] >= CubeArrayDimentions[2]))
        //        {
        //            CubicSpace cs = this.GetCubicSpace(nextIndex);
        //            if (cs.HaveEntity)
        //                IntersectedCubicSpace.Add(cs);
        //        }
        //        else if (((nextIndex[0] < 0 && Dquadrant[0] > 0) || (nextIndex[0] > CubeArrayLastIndex[0] && Dquadrant[0] < 0))
        //        || ((nextIndex[1] < 0 && Dquadrant[1] > 0) || (nextIndex[1] > CubeArrayLastIndex[1] && Dquadrant[1] < 0))
        //        || ((nextIndex[2] < 0 && Dquadrant[2] > 0) || (nextIndex[2] > CubeArrayLastIndex[2] && Dquadrant[2] < 0)))
        //        {

        //        }
        //        else
        //            end = true;
        //    }
        //    return IntersectedCubicSpace;
        //}

        public HashSet<CubicSpace> GetCubicSpaces(PointD position, Vector direction)
        {
            return GetRay(position, direction);
        }

        public HashSet<CubicSpace> GetRay(PointD position, Vector direction)
        {
            PointD ssP = GetStanderCoordinate(position);
            Vector ssD = Normalize(GetStanderVector(direction));
            int[] Dquadrant = Sign(direction.ToArray());
            int[] startIndex = GetCubicSpaceIndex(position);
            int[] currentIndex = GetCubicSpaceIndex(position);

            HashSet<CubicSpace> Ray = new HashSet<CubicSpace>();

            #region XYZ射線距離計算
            {
                double Rate1 = Dot(ssD, new Vector(1, 0, 0));//////////////////
                double Rate2 = Dot(ssD, new Vector(0, 1, 0));//////////////////
                double Rate3 = Dot(ssD, new Vector(0, 0, 1));//////////////////
                double v1 = Dquadrant[0] != 0 ? Rate1 * Abs(currentIndex[0] + Dquadrant[0] - ssP[0]) : double.PositiveInfinity;
                double v2 = Dquadrant[1] != 0 ? Rate2 * Abs(currentIndex[1] + Dquadrant[1] - ssP[1]) : double.PositiveInfinity;
                double v3 = Dquadrant[2] != 0 ? Rate3 * Abs(currentIndex[2] + Dquadrant[2] - ssP[2]) : double.PositiveInfinity;
                while( !((currentIndex[0] < 0 && Dquadrant[0] < 0)|| (currentIndex[0] > CubeArrayLastIndex[0] && Dquadrant[0] > 0))
                    && !((currentIndex[1] < 0 && Dquadrant[1] < 0)|| (currentIndex[1] > CubeArrayLastIndex[1] && Dquadrant[1] > 0))
                    && !((currentIndex[2] < 0 && Dquadrant[2] < 0)|| (currentIndex[2] > CubeArrayLastIndex[2] && Dquadrant[2] > 0)))
                {
                    CubicSpace cubic = GetCubicSpace(currentIndex);
                    if (cubic != null)
	                    Ray.Add(cubic);


                    int Dir = 0;
                    double minV = v1;
                    if ( minV > v2)
                        Dir = 1; minV = v2;
                    if (minV > v3)
                        Dir = 2; minV = v3;

                    currentIndex[Dir] += Dquadrant[Dir];

                    if (Dir == 0)
                        v1 = Dquadrant[0] != 0 ?Rate1 * Abs(currentIndex[0] + Dquadrant[0] - ssP[0]) : double.PositiveInfinity;
                    else if (Dir == 1)
                        v2 = Dquadrant[1] != 0 ?Rate2 * Abs(currentIndex[1] + Dquadrant[1] - ssP[1]) : double.PositiveInfinity;
                    else if (Dir == 2)
                        v3 = Dquadrant[2] != 0 ? Rate3 * Abs(currentIndex[2] + Dquadrant[2] - ssP[2]) : double.PositiveInfinity;
                }
            }
            #endregion

            return Ray;
        }

        public HashSet<CubicSpace> GetCubicSpaces2(PointD position, Vector direction)
        {
            HashSet<CubicSpace> CubicSpaceInD1 = GetCubicSpaces(position, direction);
            HashSet<CubicSpace> CubicSpaceInD2 = GetCubicSpaces(position, -1 * direction);
            foreach (var item in CubicSpaceInD2)
            {
                CubicSpaceInD1.Add(item);
            }
            //foreach (var item in CubicSpaceInD1)
            //{
            //    if (item.HaveEntity)
            //    item.Visible = Visibility.Visible;
            //}
            return CubicSpaceInD1;
        }

        public HashSet<Entity> GetEntities2(PointD position, Vector direction)
        {
            HashSet<CubicSpace> IntersectedCubicSpace = GetCubicSpaces2(position, direction);
            HashSet<Entity> Entities = new HashSet<Entity>();
            foreach (var cubicSpace in IntersectedCubicSpace)
            {
                foreach (var entity in cubicSpace.Entities)
	            {
                    Entities.Add(entity);
	            }
            }
            return Entities;
        }

        public void PlotInOpenGL()
        {
            for (int i = 0; i < CubeArrayDimentions[0]; i++)
            {
                for (int j = 0; j < CubeArrayDimentions[1]; j++)
                {
                    for (int k = 0; k < CubeArrayDimentions[2]; k++)
                    {
                        CubeSpaces[i, j, k].PlotInOpenGL();
                    }
                }
            }
        }

        public class iv
        {
            public int i;
            public double V;

            public iv(int i, double v)
            {
                this.i = i;
                this.V = v;
            }
        }
    }

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
