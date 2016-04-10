using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using PTL.Geometry;
using PTL.Definitions;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Measurement
{
    /// <summary>
    /// 尋找幾何上的最近點
    /// </summary>
    public class STLMeasuringTool_NearPoint : STLMeasuringTool
    {
        /// <summary>
        /// 執行緒結束時觸發此事件
        /// </summary>
        public override event EventHandler OnFinish;

        protected override void Solve()
        {
            if (this.IsBusy == false)
            {
                ///進度報告
                this.IsBusy = true;
                this.Percentage = 0;
                this.StartTime = DateTime.Now;

                this.TouchPoints = new List<XYZ4>();
                this.TouchedTriangles = new List<Triangle>();
                Entity[] AllTriangle = STL2Measure.Entities.Values.ToArray();

                for (int pointsIndex = 0; pointsIndex < MeasurePoints.Count; pointsIndex++)
                {
                    XYZ4 P = MeasurePoints[pointsIndex];
                    bool alreadyFound = false;
                    //尋找所有通過的三角面及相交的點
                    Dictionary<XYZ4, Triangle> Interaction_Points_Triangles = new Dictionary<XYZ4, Triangle>();
                    for (int TriangleIndex = 0; TriangleIndex < STL2Measure.Entities.Count && !alreadyFound; TriangleIndex++)
                    {
                        ///進度報告
                        double currentPercentage = ((double)pointsIndex * STL2Measure.Entities.Count + TriangleIndex) /
                            ((double)MeasurePoints.Count * STL2Measure.Entities.Count)
                            * 100.0;
                        if (currentPercentage > Percentage + 1)
                            Percentage++;

                        int LastNearPNum = Interaction_Points_Triangles.Count;
                        Triangle aTriangle = AllTriangle[TriangleIndex] as Triangle;
                        XYZ3 N = Cross(aTriangle.P2 - aTriangle.P1, aTriangle.P3 - aTriangle.P1);
                        XYZ4 XC = aTriangle.P1 - P;
                        XYZ4 YC = aTriangle.P2 - P;
                        XYZ4 ZC = aTriangle.P3 - P;

                        //Coordinate System, use Point P as Origin
                        double[,] csPT = new double[,]
                        {
                            {XC.X, YC.X, ZC.X},
                            {XC.Y, YC.Y, ZC.Y},
                            {XC.Z, YC.Z, ZC.Z}
                        };

                        //座標系統反矩陣
                        double[,] inv = Inverse(csPT);
                        //無法求解反矩陣代表P在平面上，其他則代表P不在平面上
                        if (inv == null)//在平面上
                        {
                            //判斷是P否在三角形內
                            XYZ4 v1 = aTriangle.P1 - P;
                            XYZ4 v2 = aTriangle.P2 - P;
                            XYZ4 v3 = aTriangle.P3 - P;

                            XYZ4 n1 = Cross(v1, v2);
                            XYZ4 n2 = Cross(v2, v3);
                            XYZ4 n3 = Cross(v3, v1);
                            if (((n1.Y >= 0 && n2.Y >= 0 && n3.Y >= 0) || (n1.Y <= 0 && n2.Y <= 0 && n3.Y <= 0)) && !(n1.Y == 0 && n2.Y == 0 && n3.Y == 0))
                            {
                                Interaction_Points_Triangles.Add(P, aTriangle);
                                alreadyFound = true;
                            }
                        }
                        else//不在平面上但法向投影點在三角形中
                        {
                            //向量 N 在 csPT 上的X,Y, Z分量
                            XYZ4 component = Transport(inv, N);
                            if ((component.X >= -0 && component.Y >= -0 && component.Z >= -0) ||
                                (component.X <= 0 && component.Y <= 0 && component.Z <= 0))
                            {
                                //求倍率
                                double NRate = 1 / (component.X + component.Y + component.Z);
                                Interaction_Points_Triangles.Add(P + N * NRate, aTriangle);
                            }
                        }
                        //解距邊線和定點的最近距離
                        if (Interaction_Points_Triangles.Count == LastNearPNum && !alreadyFound)
                        {
                            XYZ4 v11 = Normalize(aTriangle.P2 - aTriangle.P1);
                            XYZ4 V12 = P - aTriangle.P1;
                            XYZ4 v21 = Normalize(aTriangle.P3 - aTriangle.P2);
                            XYZ4 V22 = P - aTriangle.P2;
                            XYZ4 v31 = Normalize(aTriangle.P1 - aTriangle.P3);
                            XYZ4 V32 = P - aTriangle.P3;

                            //P距離邊線1的最近點
                            XYZ4 Near1 = (XYZ4)Cross(Cross(V12, v11), v11) + P;
                            double ratio = Dot((Near1 - aTriangle.P1), v11) / Norm(aTriangle.P2 - aTriangle.P1);
                            if (ratio < 0 || ratio > 1)
                            {
                                if (Norm(aTriangle.P1) < Norm(aTriangle.P2))
                                    Near1 = aTriangle.P1;
                                else
                                    Near1 = aTriangle.P2;
                            }
                            //P距離邊線1的最近點
                            XYZ4 Near2 = (XYZ4)Cross(Cross(V22, v21), v21) + P;
                            ratio = Dot((Near2 - aTriangle.P2), v21) / Norm(aTriangle.P3 - aTriangle.P2);
                            if (ratio < 0 || ratio > 1)
                            {
                                if (Norm(aTriangle.P2) < Norm(aTriangle.P3))
                                    Near2 = aTriangle.P2;
                                else
                                    Near2 = aTriangle.P3;
                            }
                            //P距離邊線1的最近點
                            XYZ4 Near3 = (XYZ4)Cross(Cross(V32, v31), v31) + P;
                            ratio = Dot((Near3 - aTriangle.P3), v31) / Norm(aTriangle.P1 - aTriangle.P3);
                            if (ratio < 0 || ratio > 1)
                            {
                                if (Norm(aTriangle.P3) < Norm(aTriangle.P1))
                                    Near3 = aTriangle.P3;
                                else
                                    Near3 = aTriangle.P1;
                            }

                            //找出Near1,Near2,Near3之中的最近點
                            XYZ4 Near = Near1;
                            if (Norm(Near2 - P) < Norm(Near - P))
                                Near = Near2;
                            if (Norm(Near3 - P) < Norm(Near - P))
                                Near = Near3;

                            Interaction_Points_Triangles.Add(Near, aTriangle);
                        }

                    }


                    //篩選出最近點
                    XYZ4[] nearestPoints = Interaction_Points_Triangles.Keys.ToArray();
                    if (nearestPoints.Length > 0)
                    {
                        XYZ4 nearPoint = nearestPoints[0];

                        for (int i = 1; i < Interaction_Points_Triangles.Count; i++)
                        {
                            if (Norm(nearestPoints[i] - P) < Norm(nearPoint - P))
                                nearPoint = nearestPoints[i];
                        }
                        this.TouchPoints.Add(nearPoint);
                        this.TouchedTriangles.Add(Interaction_Points_Triangles[nearPoint]);
                        alreadyFound = true;
                    }
                    else
                    {
                        this.TouchPoints.Add(null);
                        this.TouchedTriangles.Add(null);
                    }

                }
                ///進度報告
                this.IsBusy = false;
                this.Percentage = 100;
                this.FinishTime = DateTime.Now;
                if (OnFinish != null) OnFinish(this, new EventArgs());
            }
            return;
        }
    }
}
