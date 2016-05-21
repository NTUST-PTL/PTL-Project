using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGL.OpenGL;
using PTL.OpenGL.Plot;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Geometry
{
    public class CubicClassifier : ICanPlotInOpenGL
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
            this.Boundary = part.GetBoundary(null);
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
                XYZ4[] boundary = entity.GetBoundary(null);
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

        public VectorD GetStanderVector(VectorD v)
        {
            return new VectorD(
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

        public HashSet<CubicSpace> GetCubicSpaces(PointD position, VectorD direction)
        {
            return GetRay(position, direction);
        }

        public HashSet<CubicSpace> GetRay(PointD position, VectorD direction)
        {
            PointD ssP = GetStanderCoordinate(position);
            VectorD ssD = Normalize(GetStanderVector(direction));
            int[] Dquadrant = EachSign(direction.ToArray()).ToArray();
            int[] startIndex = GetCubicSpaceIndex(position);
            int[] currentIndex = GetCubicSpaceIndex(position);

            HashSet<CubicSpace> Ray = new HashSet<CubicSpace>();

            #region XYZ射線距離計算
            {
                double deltaLX = Abs(1 / ssD[0]);//////////////////
                double deltaLY = Abs(1 / ssD[1]);//////////////////
                double deltaLZ = Abs(1 / ssD[2]);//////////////////
                double initailLX = ssD[0] > 0? (Ceiling(ssP[0]) - ssP[0]) / ssD[0] : (Floor(ssP[0]) - ssP[0]) / ssD[0];
                double initailLY = ssD[1] > 0 ? (Ceiling(ssP[1]) - ssP[1]) / ssD[1] : (Floor(ssP[1]) - ssP[1]) / ssD[1];
                double initailLZ = ssD[2] > 0 ? (Ceiling(ssP[2]) - ssP[2]) / ssD[2] : (Floor(ssP[2]) - ssP[2]) / ssD[2];
                double currentLX = initailLX;
                double currentLY = initailLY;
                double currentLZ = initailLZ;
                while( !((currentIndex[0] < 0 && Dquadrant[0] < 0)|| (currentIndex[0] > CubeArrayLastIndex[0] && Dquadrant[0] > 0))
                    && !((currentIndex[1] < 0 && Dquadrant[1] < 0)|| (currentIndex[1] > CubeArrayLastIndex[1] && Dquadrant[1] > 0))
                    && !((currentIndex[2] < 0 && Dquadrant[2] < 0)|| (currentIndex[2] > CubeArrayLastIndex[2] && Dquadrant[2] > 0)))
                {
                    CubicSpace cubic = GetCubicSpace(currentIndex);
                    if (cubic != null)
	                    Ray.Add(cubic);

                    double nextLX = currentLX + deltaLX;
                    double nextLY = currentLY + deltaLY;
                    double nextLZ = currentLZ + deltaLZ;

                    int Dir = 0;
                    double minL = nextLX;
                    if (minL > nextLY)
                    { minL = nextLY; Dir = 1; }
                    if (minL > nextLZ)
                    { minL = nextLZ; Dir = 2; }

                    currentIndex[Dir] += Dquadrant[Dir];

                    if (Dir == 0)
                        currentLX = nextLX;
                    else if (Dir == 1)
                        currentLY = nextLY;
                    else if (Dir == 2)
                        currentLZ = nextLZ;
                }
            }
            #endregion

            return Ray;
        }

        public HashSet<CubicSpace> GetCubicSpaces2(PointD position, VectorD direction)
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

        public HashSet<Entity> GetEntities2(PointD position, VectorD direction)
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
}
