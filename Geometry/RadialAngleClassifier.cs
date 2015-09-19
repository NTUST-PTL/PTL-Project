using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    class RadialAngleClassifier : PTL.Mathematics.Math
    {
        private int _n = 100;
        private XYZ4[] _Boundary;
        private double[] _Dimentions;
        private double[] _CubeDimentions;
        private int[] _CubeArrayDimentions;
        private int[] _CubeArrayLastIndex;
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
        public CubicSpace[, ,] CubeSpaces
        {
            get { return _CubeSpaces; }
            protected set { this._CubeSpaces = value; }
        }

        public void CubicClassify(EntityCollection part)
        {
            this.Boundary = part.Boundary;
            this.Dimentions = (Boundary[1] - Boundary[0]);
            double longEdgeLength = (new double[] { Norm(new PointD(this.Dimentions[0], this.Dimentions[1], 0)), this.Dimentions[2] }).Max();
            double unit = longEdgeLength / this.n;
            this.CubeDimentions = new double[] { unit, unit, unit };
            this.CubeArrayDimentions = new int[]{ 
                (int)System.Math.Ceiling(Dimentions[0] / CubeDimentions[0]), 
                n, 
                (int)System.Math.Ceiling(Dimentions[2] / CubeDimentions[2]) };
            this.CubeArrayLastIndex = new int[]{ 
                CubeArrayDimentions[0] - 1, 
                CubeArrayDimentions[1] - 1, 
                CubeArrayDimentions[2] - 1 };

            CubeSpaces = new CubicSpace[CubeArrayDimentions[0], CubeArrayDimentions[1], CubeArrayDimentions[2]];
            for (int i = 0; i < CubeArrayDimentions[0]; i++)
            {
                for (int j = 0; j < CubeArrayDimentions[1]; j++)
                {
                    for (int k = 0; k < CubeArrayDimentions[2]; k++)
                    {
                        CubeSpaces[i, j, k] = new CubicSpace();
                    }
                }
            }

            //foreach (Entity entity in part.Entities.Values)
            //{
            //    PointD[] boundary = entity.Boundary;
            //    int[] startCubeIndex = GetCubicSpaceIndex(boundary[0]);
            //    int[] endCubeIndex = GetCubicSpaceIndex(boundary[1]);

            //    for (int i = startCubeIndex[0]; i <= endCubeIndex[0]; i++)
            //    {
            //        for (int j = startCubeIndex[1]; j <= endCubeIndex[1]; j++)
            //        {
            //            for (int k = startCubeIndex[2]; k <= endCubeIndex[2]; k++)
            //            {
            //                CubeSpaces[i, j, k].Add(entity);
            //            }
            //        }
            //    }
            //}
        }

        public PointD GetStanderCoordinate(XYZ4 p)
        {
            PointD p1 = p - this.Boundary[0];
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
            return CubeSpaces[index[0], index[1], index[2]];
        }

        public CubicSpace GetCubicSpace(params int[] index)
        {
            return CubeSpaces[index[0], index[1], index[2]];
        }

        public HashSet<CubicSpace> GetCubicSpaces(PointD position, Vector direction)
        {
            PointD ssP = GetStanderCoordinate(position);
            Vector ssD = Normalize(GetStanderVector(direction));
            HashSet<CubicSpace> IntersectedCubicSpace = new HashSet<CubicSpace>();
            int[] Dquadrant = Sign(direction.ToArray());
            int[] startIndex = GetCubicSpaceIndex(position);
            int[] nextIndex = GetCubicSpaceIndex(position);
            PointD nextsP = (PointD)ssP.Clone();
            if (startIndex[0] > 0 && startIndex[0] < CubeArrayDimentions[0]
                && startIndex[1] > 0 && startIndex[1] < CubeArrayDimentions[1]
                && startIndex[2] > 0 && startIndex[2] < CubeArrayDimentions[2])
                IntersectedCubicSpace.Add(this.GetCubicSpace(startIndex));

            bool end = false;
            while (!end)
            {
                double[] rate6 = new double[]{
                    (System.Math.Ceiling(nextsP.X) - nextsP.X),
                    (System.Math.Ceiling(nextsP.Y) - nextsP.Y),
                    (System.Math.Ceiling(nextsP.Z) - nextsP.Z),
                    (System.Math.Floor(nextsP.X) - nextsP.X),
                    (System.Math.Floor(nextsP.Y) - nextsP.Y),
                    (System.Math.Floor(nextsP.Z) - nextsP.Z)
                };
                rate6[0] = ssD.X != 0 ? rate6[0] / ssD.X : double.NegativeInfinity;
                rate6[1] = ssD.Y != 0 ? rate6[1] / ssD.Y : double.NegativeInfinity;
                rate6[2] = ssD.Z != 0 ? rate6[2] / ssD.Z : double.NegativeInfinity;
                rate6[3] = ssD.X != 0 ? rate6[3] / ssD.X : double.NegativeInfinity;
                rate6[4] = ssD.Y != 0 ? rate6[4] / ssD.Y : double.NegativeInfinity;
                rate6[5] = ssD.Z != 0 ? rate6[5] / ssD.Z : double.NegativeInfinity;


                double maxRate = rate6[0];
                int maxRateIndex = 0;
                for (int i = 0; i < 6; i++)
                    if (rate6[i] > maxRate)
                    {
                        maxRate = rate6[i];
                        maxRateIndex = i;
                    }
                double minRate = maxRate;
                int minRateIndex = maxRateIndex;
                for (int i = 0; i < 6; i++)
                    if (rate6[i] < minRate && rate6[i] >= 0)
                    {
                        minRate = rate6[i];
                        minRateIndex = i;
                    }
                if (minRateIndex == 0 || minRateIndex == 3)
                    nextIndex[0] += Dquadrant[0];
                if (minRateIndex == 1 || minRateIndex == 4)
                    nextIndex[1] += Dquadrant[1];
                if (minRateIndex == 2 || minRateIndex == 5)
                    nextIndex[2] += Dquadrant[2];
                nextsP = nextsP + ssD * (maxRate + minRate) / 2.0;

                if (nextIndex[0] > 0 && nextIndex[0] < CubeArrayDimentions[0]
                && nextIndex[1] > 0 && nextIndex[1] < CubeArrayDimentions[1]
                && nextIndex[2] > 0 && nextIndex[2] < CubeArrayDimentions[2])
                    IntersectedCubicSpace.Add(this.GetCubicSpace(nextIndex));//
                else if (
                      ((nextIndex[0] < 0 || nextIndex[0] >= CubeArrayDimentions[0]) && nextIndex[0] / Dquadrant[0] < 0)
                    && ((nextIndex[1] < 0 || nextIndex[1] >= CubeArrayDimentions[1]) && nextIndex[1] / Dquadrant[1] < 0)
                    && ((nextIndex[2] < 0 || nextIndex[2] >= CubeArrayDimentions[2]) && nextIndex[2] / Dquadrant[2] < 0))
                {

                }
                else
                    end = true;
            }
            return IntersectedCubicSpace;
        }

        public HashSet<CubicSpace> GetCubicSpaces2(PointD position, Vector direction)
        {
            HashSet<CubicSpace> CubicSpaceInD1 = GetCubicSpaces(position, direction);
            HashSet<CubicSpace> CubicSpaceInD2 = GetCubicSpaces(position, -1 * direction);
            foreach (var item in CubicSpaceInD2)
            {
                CubicSpaceInD1.Add(item);
            }
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
    }
}
