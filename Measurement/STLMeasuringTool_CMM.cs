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
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Measurement
{
    /// <summary>
    /// 尋找給定法向直線上的最近點
    /// </summary>
    public class STLMeasuringTool_CMM : STLMeasuringTool, ISTLMeasurement_NeedNormals
    {
        /// <summary>
        /// 量測點的法向量
        /// </summary>
        public List<VectorD> MeasurePointNormals { get; set; }

        /// <summary>
        /// 執行緒結束時觸發此事件
        /// </summary>
        public override event EventHandler OnFinish;

        /// <summary>
        /// 求解
        /// </summary>
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
                //Entity[] AllTriangle = STL2Measure.Entities.Values.ToArray();

                for (int pointsIndex = 0; pointsIndex < MeasurePoints.Count; pointsIndex++)
                {
                    //Console.WriteLine(pointsIndex);
                    XYZ4 P = MeasurePoints[pointsIndex];
                    VectorD N = MeasurePointNormals[pointsIndex];
                    bool alreadyFound = false;

                    ///進度報告
                    double currentPercentage = ((double)pointsIndex) / (double)MeasurePoints.Count * 100.0;
                    if (currentPercentage > Percentage + 1)
                        Percentage = currentPercentage;

                    //所有可能的三角面
                    Entity[] AllTriangle = STLCubicClassifier.GetEntities2(P, N).ToArray();

                    //所有通過的三角面及相交的點
                    Dictionary<XYZ4, Triangle> Interaction_Points_Triangles = new Dictionary<XYZ4, Triangle>();
                    for (int TriangleIndex = 0; TriangleIndex < AllTriangle.Length && !alreadyFound; TriangleIndex++)
                    {
                        int LastNearPNum = Interaction_Points_Triangles.Count;
                        Triangle aTriangle = AllTriangle[TriangleIndex] as Triangle;
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
                        else
                        {
                            //向量 N 在 csPT 上的X,Y, Z分量
                            VectorD component = Transport3(inv, N);
                            if ((component.X >= -0 && component.Y >= -0 && component.Z >= -0) ||
                                (component.X <= 0 && component.Y <= 0 && component.Z <= 0))
                            {
                                //求倍率
                                double NRate = 1 / (component.X + component.Y + component.Z);
                                Interaction_Points_Triangles.Add(P + N * NRate, aTriangle);
                            }
                        }
                    }


                    //篩選出最近點
                    XYZ4[] nearestPoints = Interaction_Points_Triangles.Keys.ToArray();
                    if (nearestPoints.Length > 0)
                    {
                        XYZ4 nearPoint = nearestPoints[0];

                        for (int i = 1; i < nearestPoints.Length; i++)
                        {
                            if (Norm(nearestPoints[i] - P) < Norm(nearPoint - P))
                            {
                                nearPoint = nearestPoints[i];
                            }
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

                    if (this.TouchPoints.Count < pointsIndex)
                    {
                        System.Windows.MessageBox.Show("this.TouchPoints.Count < pointsIndex"); 
                    }
                }

                ///進度報告
                this.IsBusy = false;
                this.Percentage = 100;
                this.FinishTime = DateTime.Now;
                OnFinish?.Invoke(this, new EventArgs());
            }
            return;
        }
    }
}
