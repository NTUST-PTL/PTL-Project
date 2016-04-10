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
    /// 尋找延Z軸旋轉的最近點
    /// </summary>
    public class STLMeasuringTool_ZAngle : STLMeasuringTool
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
                    //Console.WriteLine(pointsIndex);
                    XYZ4 P = MeasurePoints[pointsIndex];
                    bool alreadyFound = false;

                    //所有通過的三角面及相交的點
                    Dictionary<XYZ4, Triangle> Interaction_Points_Triangles = new Dictionary<XYZ4, Triangle>();
                    for (int TriangleIndex = 0; TriangleIndex < AllTriangle.Length && !alreadyFound; TriangleIndex++)
                    {
                        //進度報告
                        double currentPercentage = ((double)pointsIndex * this.STL2Measure.Entities.Count + TriangleIndex) /
                            ((double)MeasurePoints.Count * this.STL2Measure.Entities.Count)
                            * 100.0;
                        if (currentPercentage > Percentage + 1)
                            Percentage++;

                        Triangle aTriangle = AllTriangle[TriangleIndex] as Triangle;

                        XYZ4 TriangleNormal = Cross(aTriangle.P2 - aTriangle.P1, aTriangle.P3 - aTriangle.P1);
                        double r = Sqrt(P.X * P.X + P.Y * P.Y);
                        double L = Sqrt((TriangleNormal.X) * r * (TriangleNormal.X) * r + (TriangleNormal.Y) * r * (TriangleNormal.Y) * r);
                        double C = (Dot(aTriangle.P1, TriangleNormal) - TriangleNormal.Z * P.Z) / L;
                        double phi = Atan2(TriangleNormal.Y * r, TriangleNormal.X * r);
                        double gamma = Acos(C);
                        double[] thetas = new double[] { gamma + phi, phi - gamma };

                        for (int i = 0; i < thetas.Length; i++)
                        {
                            XYZ4 ans = new XYZ4(r * Cos(thetas[i]), r * Sin(thetas[i]), P.Z);

                            XYZ4 v1 = aTriangle.P1 - ans;
                            XYZ4 v2 = aTriangle.P2 - ans;
                            XYZ4 v3 = aTriangle.P3 - ans;

                            XYZ4 n1 = Cross(v1, v2);
                            XYZ4 n2 = Cross(v2, v3);
                            XYZ4 n3 = Cross(v3, v1);
                            if (((n1.Y >= 0 && n2.Y >= 0 && n3.Y >= 0) || (n1.Y <= 0 && n2.Y <= 0 && n3.Y <= 0)) && !(n1.Y == 0 && n2.Y == 0 && n3.Y == 0))
                                Interaction_Points_Triangles.Add(ans, aTriangle);
                        }
                    }


                    //篩選出最近點
                    XYZ4[] Interaction_Points = Interaction_Points_Triangles.Keys.ToArray();
                    Triangle[] Interaction_Triangles = Interaction_Points_Triangles.Values.ToArray();
                    if (Interaction_Points.Length > 0)
                    {
                        XYZ4 nearestPoint = Interaction_Points[0];
                        Triangle nearestTriangle = Interaction_Triangles[0];
                        for (int i = 1; i < Interaction_Points.Length; i++)
                        {
                            if (Norm(Interaction_Points[i] - P) < Norm(nearestPoint - P))
                            {
                                nearestPoint = Interaction_Points[i];
                                nearestTriangle = Interaction_Triangles[i];
                            }
                        }
                        this.TouchPoints.Add(nearestPoint);
                        this.TouchedTriangles.Add(nearestTriangle);
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
            
            ;
        }
    }
}
