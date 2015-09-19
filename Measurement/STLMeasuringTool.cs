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

namespace PTL.Measurement
{
    public class STLMeasuringTool_MultiThread : STLMeasuringTool, ISTLMeasurement_NeedNormals
    {
        /// <summary>
        /// 量測點的法向量
        /// </summary>
        public virtual List<Vector> MeasurePointNormals { get; set; }
        /// <summary>
        /// 執行緒數目
        /// </summary>
        public virtual int ThreadNumber { get; set; }

        //狀態
        /// <summary>
        /// 已完成的執行緒
        /// </summary>
        public int FinishedThreadNumber { get; protected set;}


        /// <summary>
        /// 所有執行緒結束時觸發此事件
        /// </summary>
        public override event EventHandler OnFinish;

        //獨立執行緒物件
        public STLMeasuringTool STLMeasurentReferenceObject;
        public List<STLMeasuringTool> STLMeasurent_Objects;

        protected STLMeasuringTool_MultiThread() { }
        public STLMeasuringTool_MultiThread(STL STL2Measure, List<XYZ4> P, List<Vector> N, int ThreadNumber, EventHandler OnFinish, STLMeasuringTool objSTLMeasurement)
        {
            this.STL2Measure = STL2Measure;
            this.MeasurePoints = P;
            this.MeasurePointNormals = N;
            this.ThreadNumber = ThreadNumber;
            this.OnFinish += OnFinish;
            this.STLMeasurentReferenceObject = objSTLMeasurement;
        }


        /// <summary>
        /// 給定STL，量測點及量測方向
        /// 計算 從量測點出發、延量測方向距離最近 的位置
        /// 若與STL所有三角面都無相交則回傳 null
        /// </summary>
        /// <param name="aSTL">STL</param>
        /// <param name="P">量測點</param>
        /// <param name="N">量測方向</param>
        /// <returns>最近點</returns>
        public override void StartMeasure() 
        {
            if (this.IsBusy == false)
            {
                //設定狀態
                this.FinishedThreadNumber = 0;
                this.IsBusy = true;
                this.StartTime = DateTime.Now;

                //劃分執行緒工作區域
                List<int> sliceIndex = new List<int>();
                for (int i = 0; i <= this.ThreadNumber; i++)
                {
                    sliceIndex.Add((int)((double)i * (double)MeasurePoints.Count / (double)this.ThreadNumber));
                }
                //建立量測物件並指定量測範圍
                STLMeasurent_Objects = new List<STLMeasuringTool>();
                
                for (int i = 0; i < sliceIndex.Count - 1; i++)
                {
                    XYZ4[] PartoalPoints = new XYZ4[sliceIndex[i + 1] - sliceIndex[i]];
                    for (int j = 0; j < sliceIndex[i + 1] - sliceIndex[i]; j++)
                        PartoalPoints[j] = this.MeasurePoints[j + sliceIndex[i]];
                    STLMeasuringTool aSTLMeasurementObject = (STLMeasuringTool)Activator.CreateInstance(this.STLMeasurentReferenceObject.GetType());
                    aSTLMeasurementObject.STL2Measure = this.STL2Measure;
                    aSTLMeasurementObject.MeasurePoints = PartoalPoints.ToList();
                    aSTLMeasurementObject.PercentageChanged += this.OnThreadPercentageChanged;
                    aSTLMeasurementObject.OnFinish += this.OnThreadFinished;
                    if (aSTLMeasurementObject is ISTLMeasurement_NeedNormals && this.MeasurePointNormals != null)
                    {
                        Vector[] PartoalNormals = new Vector[sliceIndex[i + 1] - sliceIndex[i]];
                        for (int j = 0; j < sliceIndex[i + 1] - sliceIndex[i]; j++)
                            PartoalNormals[j] = this.MeasurePointNormals[j + sliceIndex[i]];
                        (aSTLMeasurementObject as ISTLMeasurement_NeedNormals).MeasurePointNormals = PartoalNormals.ToList();
                    }
                    STLMeasurent_Objects.Add(aSTLMeasurementObject);
                }

                foreach (var aThreadObject in STLMeasurent_Objects)
                {
                    aThreadObject.PercentageChanged += OnThreadPercentageChanged;
                    aThreadObject.StartMeasure();
                }
            }
        }
        protected override void Solve() { }

        protected virtual void OnThreadPercentageChanged(object sender, double e)
        {
            double currentPercentage = 0;
            foreach (var item in STLMeasurent_Objects)
                currentPercentage += item.Percentage;
            currentPercentage /= STLMeasurent_Objects.Count;
            if (currentPercentage >= this.Percentage + 1)
            {
                this.Percentage += 1;
                //Console.WriteLine(this.Percentage);
            }
        }
        protected virtual void OnThreadFinished(object sender, EventArgs e)
        {
            this.FinishedThreadNumber++;
            this.TouchPoints = new List<XYZ4>();
            if (this.FinishedThreadNumber == this.ThreadNumber)
            {
                foreach (var item in STLMeasurent_Objects)
                {
                    for (int i = 0; i < item.TouchPoints.Count; i++)
                    {
                        this.TouchPoints.Add(item.TouchPoints[i]);
                    }
                }
                //設定狀態
                this.IsBusy = false;
                this.FinishTime = DateTime.Now;

                if (OnFinish != null)
                {
                    OnFinish(this, new EventArgs());
                }
            }
        }

        public void AddNormalLine( Layer aLayer)
        {
            foreach (var item in this.STLMeasurent_Objects)
            {
                if ( item is STLMeasuringTool_CMM)
                {
                    ISTLMeasurement_NeedNormals aSTLMeasurement_CMM = item as ISTLMeasurement_NeedNormals;
                    for (int i = 0; i < item.MeasurePoints.Count; i++)
                    {
                        aLayer.AddEntity(new Line(item.MeasurePoints[i], item.MeasurePoints[i] + aSTLMeasurement_CMM.MeasurePointNormals[i]));
                    }
                }
            }
        }
    }


    public abstract class STLMeasuringTool : PTL.Mathematics.Math
    {
        /// <summary>
        /// 要量測的STL
        /// </summary>
        public virtual STL STL2Measure { get; set; }
        /// <空間陣列化的STL
        /// </summary>
        public virtual CubicClassifier STLCubicClassifier { get; set; }
        /// <summary>
        /// 量測點
        /// </summary>
        public virtual List<XYZ4> MeasurePoints { get; set; }
        /// <summary>
        /// 搜尋結果，最近點
        /// </summary>
        public virtual List<XYZ4> TouchPoints { get; protected set; }
        /// <summary>
        /// 搜尋結果，最近的三角面
        /// </summary>
        public virtual List<Triangle> TouchedTriangles { get; protected set; }

        /// <summary>
        /// 計算開始時間
        /// </summary>
        public virtual DateTime StartTime { get; protected set; }
        /// <summary>
        /// 計算結束時間
        /// </summary>
        public virtual DateTime FinishTime { get; protected set; }
        /// <summary>
        /// 計算完成度百分比
        /// </summary>
        public virtual double Percentage
        {
            get { return percentage; }
            protected set
            {
                if (this.percentage != value)
                {
                    this.percentage = value;
                    if (PercentageChanged != null) PercentageChanged(this, this.percentage);
                }
            }
        }
        protected double percentage;
        /// <summary>
        /// 還在量測中
        /// </summary>
        public virtual bool IsBusy { get; protected set; }

        /// <summary>
        /// 完成度百分比改變觸發此事件
        /// </summary>
        public virtual event EventHandler<double> PercentageChanged;
        /// <summary>
        /// 執行緒結束時觸發此事件
        /// </summary>
        public virtual event EventHandler OnFinish;
        
        /// <summary>
        /// 建立獨立執行緒並開始計算
        /// </summary>
        public virtual void StartMeasure()
        
        {
            //獨立執行緒方法1，手動建立執行緒並指派任務
            //Thread newThread;
            //newThread = new Thread(new ThreadStart(this.Solve));
            //newThread.Start();

            //獨立執行緒方法2，委派本身的BeginInvoke方法其實也是建立另外的執行去進行計算
            
            (new Action(Solve)).BeginInvoke(null, null);
        }

        /// <summary>
        /// 求解
        /// </summary>
        protected abstract void Solve();
    }
    public interface ISTLMeasurement_NeedNormals 
    {
        /// <summary>
        /// 量測點的法向量
        /// </summary>
        List<Vector> MeasurePointNormals { get; set; }
    }

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
                        double[,] inv = MatrixInverse(csPT);
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

    /// <summary>
    /// 尋找給定法向直線上的最近點
    /// </summary>
    public class STLMeasuringTool_CMM : STLMeasuringTool, ISTLMeasurement_NeedNormals
    {
        /// <summary>
        /// 量測點的法向量
        /// </summary>
        public List<Vector> MeasurePointNormals { get; set; }

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
                    Vector N = MeasurePointNormals[pointsIndex];
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
                        double[,] inv = MatrixInverse(csPT);
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
                            Vector component = Transport3(inv, N);
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
                if (OnFinish != null) OnFinish(this, new EventArgs());
            }
            return;
        }
    }

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

    /// <summary>
    /// 模擬P40齒輪專用量測機
    /// </summary>
    public class STLMeasuringTool_P40_SingleTooth : STLMeasuringTool_MultiThread, ISTLMeasurement_NeedNormals
    {
        public virtual TopoFace TopoFace1 { get; set; }
        public virtual TopoFace TopoFace2 { get; set; }
        public virtual XYZ4 PitchPoint1 { get; set; }
        public virtual XYZ4 PitchPoint2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Dictionary<XYZ4, double> RotateAngles { get; private set; }
        public virtual Dictionary<XYZ4, double> RotateDistance { get; private set; }
        public virtual Dictionary<XYZ4, XYZ4> RotatedPitchPoint { get; private set; }
        public virtual double[,] Distance1 { get; private set; }
        public virtual double[,] Distance2 { get; private set; }
        public virtual RotationOptions RotationOption { get; set; }
        public enum RotationOptions
        {
            None,
            ToFirstSide,
            ToSecondSide,
            ToBothSide
        }

        public override List<Vector> MeasurePointNormals
        {
            get
            {
                return base.MeasurePointNormals;
            }
            set
            {
                base.MeasurePointNormals = value;
            }
        }

        /// <summary>
        /// 執行緒結束時觸發此事件
        /// </summary>
        public override event EventHandler OnFinish;

        /// <summary>
        /// 模擬P40量測類別的建構式
        /// </summary>
        /// <param name="STL2Measure">欲量測的STL</param>
        /// <param name="TopoFace1">第一拓樸面</param>
        /// <param name="TopoFace2">第二拓樸面</param>
        /// <param name="PitchPoint1">第一拓樸面的PitchPoint</param>
        /// <param name="PitchPoint2">第二拓樸面的PitchPoint</param>
        /// <param name="ThreadNumber">執行緒數目</param>
        /// <param name="OnFinish">計算完成時的後續動作</param>
        public STLMeasuringTool_P40_SingleTooth(STL STL2Measure, TopoFace TopoFace1, TopoFace TopoFace2, XYZ4 PitchPoint1, XYZ4 PitchPoint2, int ThreadNumber, EventHandler OnFinish)
        {
            this.STL2Measure = STL2Measure;
            this.TopoFace1 = TopoFace1;
            this.TopoFace2 = TopoFace2;
            this.PitchPoint1 = PitchPoint1;
            this.PitchPoint2 = PitchPoint2;
            this.ThreadNumber = ThreadNumber;
            this.OnFinish += OnFinish;
        }

        public override void StartMeasure()
        {
            if (this.STLCubicClassifier == null)
            {
                this.STLCubicClassifier = new CubicClassifier();
                this.STLCubicClassifier.CubicClassify(this.STL2Measure);
            }

            this.FinishedThreadNumber = 0;
            this.RotateAngles = new Dictionary<XYZ4, double>();
            this.RotateDistance = new Dictionary<XYZ4, double>();
            this.RotatedPitchPoint = new Dictionary<XYZ4, XYZ4>();
            this.STLMeasurent_Objects = new List<STLMeasuringTool>();
            STLMeasuringTool_ZAngle aSTLMeasuringTool_ZAngle;
            aSTLMeasuringTool_ZAngle = new STLMeasuringTool_ZAngle() { STL2Measure = this.STL2Measure, MeasurePoints = new List<XYZ4>() { this.PitchPoint1, this.PitchPoint2 } };
            aSTLMeasuringTool_ZAngle.OnFinish += aSTLMeasuringTool_ZAngle_OnFinish;
            aSTLMeasuringTool_ZAngle.StartMeasure();
        }

        private void aSTLMeasuringTool_ZAngle_OnFinish(object sender, EventArgs e)
        {
            STLMeasuringTool_ZAngle STLMeasuringTool_ZAngle = sender as STLMeasuringTool_ZAngle;
            if (STLMeasuringTool_ZAngle.TouchPoints[0] != null && STLMeasuringTool_ZAngle.TouchPoints[1] != null)
            {
                XYZ4 measurePoint, touchPoint;
                //讀取第一個PitchPoint的旋轉角
                measurePoint = this.PitchPoint1;
                touchPoint = STLMeasuringTool_ZAngle.TouchPoints[0];
                RotateAngles.Add(measurePoint, Atan2(touchPoint.Y, touchPoint.X) - Atan2(measurePoint.Y, measurePoint.X));
                RotateDistance.Add(measurePoint, Norm(measurePoint - new XYZ4(0, 0, measurePoint.Z)) * RotateAngles[measurePoint]);
                RotatedPitchPoint.Add(measurePoint, STLMeasuringTool_ZAngle.TouchPoints[0]);
                //讀取第二個PitchPoint的旋轉角
                measurePoint = this.PitchPoint2;
                touchPoint = STLMeasuringTool_ZAngle.TouchPoints[1];
                RotateAngles.Add(measurePoint, Atan2(touchPoint.Y, touchPoint.X) - Atan2(measurePoint.Y, measurePoint.X));
                RotateDistance.Add(measurePoint, Norm(measurePoint - new XYZ4(0, 0, measurePoint.Z)) * RotateAngles[measurePoint]);
                RotatedPitchPoint.Add(measurePoint, STLMeasuringTool_ZAngle.TouchPoints[1]);

                //根據旋轉兩個旋轉角旋轉拓樸面
                double rotateAngle;
                rotateAngle = RotateAngles[this.PitchPoint1];
                TopoFace1.Transform(RotateMatrix(Axis.Z, rotateAngle));
                rotateAngle = RotateAngles[this.PitchPoint2];
                TopoFace2.Transform(RotateMatrix(Axis.Z, rotateAngle));
                Solve();
            }
        }

        protected override void Solve()
        {
            //提取所有拓樸點
            XYZ4[] allTopoPoints = new XYZ4[this.TopoFace1.Points.Length + this.TopoFace2.Points.Length];
            Vector[] allTopoPointNormals = new Vector[this.TopoFace1.Normals.Length + this.TopoFace2.Normals.Length];
            int index;
            index = 0;
            foreach (var item in this.TopoFace1.Points)
            {
                allTopoPoints[index] = item;
                index++;
            }
            foreach (var item in this.TopoFace2.Points)
            {
                allTopoPoints[index] = item;
                index++;
            }
            index = 0;
            foreach (var item in this.TopoFace1.Normals)
            {
                allTopoPointNormals[index] = item;
                index++;
            }
            foreach (var item in this.TopoFace2.Normals)
            {
                allTopoPointNormals[index] = item;
                index++;
            }

            //劃分計算範圍
            List<int> sliceIndex = new List<int>();
            for (int i = 0; i < this.ThreadNumber + 1; i++)
                sliceIndex.Add((int)(((double)i * (double)allTopoPoints.Length) / (double)this.ThreadNumber));

            //建立獨立執行序物件
            for (int i = 0; i < this.ThreadNumber; i++)
            {
                //取出計算範圍內的點
                XYZ4[] PartoalPoints = new XYZ4[sliceIndex[i + 1] - sliceIndex[i]];
                for (int j = 0; j < sliceIndex[i + 1] - sliceIndex[i]; j++)
                    PartoalPoints[j] = allTopoPoints[j + sliceIndex[i]];
                Vector[] PartoalNormals = new Vector[sliceIndex[i + 1] - sliceIndex[i]];
                for (int j = 0; j < sliceIndex[i + 1] - sliceIndex[i]; j++)
                    PartoalNormals[j] = allTopoPointNormals[j + sliceIndex[i]];
                //建立物件
                STLMeasuringTool_CMM aSTLMeasuringTool_CMM = new STLMeasuringTool_CMM()
                {
                    STL2Measure = this.STL2Measure,
                    STLCubicClassifier = this.STLCubicClassifier,
                    MeasurePoints = PartoalPoints.ToList(),
                    MeasurePointNormals = PartoalNormals.ToList(),
                };
                aSTLMeasuringTool_CMM.PercentageChanged += this.OnThreadPercentageChanged;
                aSTLMeasuringTool_CMM.OnFinish += this.OnThreadFinished;
                STLMeasurent_Objects.Add(aSTLMeasuringTool_CMM);
            }

            this.StartTime = DateTime.Now;
            foreach (var aThreadObject in STLMeasurent_Objects)
            {
                aThreadObject.PercentageChanged += OnThreadPercentageChanged;
                aThreadObject.StartMeasure();
            }
        }

        protected override void OnThreadFinished(object sender, EventArgs e)
        {
            this.FinishedThreadNumber++;
            if (this.FinishedThreadNumber == this.ThreadNumber)
            {
                //抓取結果
                this.MeasurePoints = new List<XYZ4>();
                this.MeasurePointNormals = new List<Vector>();
                this.TouchPoints = new List<XYZ4>();
                this.TouchedTriangles = new List<Triangle>();
                foreach (var item in STLMeasurent_Objects)
                {
                    for (int i = 0; i < item.TouchPoints.Count; i++)
                    {
                        this.TouchPoints.Add(item.TouchPoints[i]);
                        this.TouchedTriangles.Add(item.TouchedTriangles[i]);
                        this.MeasurePoints.Add(item.MeasurePoints[i]);
                        this.MeasurePointNormals.Add((item as STLMeasuringTool_CMM).MeasurePointNormals[i]);
                    } 
                }

                int count = MeasurePoints.Count;
                double[] distanceAll = new double[count];
                for (int i = 0; i < count; i++)
                    if (this.TouchPoints[i] != null)
                        distanceAll[i] = Dot(this.TouchPoints[i] - this.MeasurePoints[i], this.MeasurePointNormals[i]);
                    else
                        distanceAll[i] = 0;

                this.Distance1 = (double[,])ArrayReshape(
                    (double[])ArrayTake(distanceAll, new int[] { 0 }, new int[] { count / 2 - 1 }),
                    ArrayGetDimension(this.TopoFace1.Points));
                this.Distance2 = (double[,])ArrayReshape(
                    (double[])ArrayTake(distanceAll, new int[] { count / 2 }, new int[] { count - 1 }),
                    ArrayGetDimension(this.TopoFace1.Points));

                //設定狀態
                this.IsBusy = false;
                this.FinishTime = DateTime.Now;

                if (OnFinish != null)
                {
                    OnFinish(this, new EventArgs());
                }
            }
        }
    }

    public class STLMeasuringTool_P40 : PTL.Mathematics.Math
    {
        public virtual List<TopoFace[]> TopoFaces { get; set; }
        public virtual List<XYZ4[]> PitchPoints { get; set; }
        public virtual STL STL2Measure { get; set; }

        public virtual CubicClassifier STLCubicClassifier { get; set; }
        

        //Result
        public virtual List<double[]> PitchAngleErrors { get; protected set; }
        public virtual List<double[]> PitchDistanceErrors { get; protected set; }
        public virtual List<double[][,]> FaceErrors { get; protected set; }
        public virtual double[] PitchAngleError_Average { get; protected set; }
        public virtual double[] PitchDistanceError_Average { get; protected set; }
        public virtual double[][,] FaceError_Average { get; protected set; }

        public virtual List<STLMeasuringTool_P40_SingleTooth> P40_Singles { get; protected set; }
        public virtual int ThreadNumber { get; set; }
        public virtual event EventHandler OnFinish;
        public virtual int currentIndex { get; protected set; }

        /// <summary>
        /// 計算完成度百分比
        /// </summary>
        public virtual int Percentage
        {
            get { return percentage; }
            protected set
            {
                if (this.percentage != value)
                {
                    this.percentage = value;
                    if (PercentageChanged != null) PercentageChanged(this, this.percentage);
                }
            }
        }
        protected int percentage;
        /// <summary>
        /// 完成度百分比改變觸發此事件
        /// </summary>
        public virtual event EventHandler<int> PercentageChanged;
        /// <summary>
        /// 執行緒結束時觸發此事件
        /// </summary>

        public STLMeasuringTool_P40(
            STL STL2Measure,
            List<TopoFace[]> topoFaces,
            List<XYZ4[]> pitchPoints
            )
        {
            this.STL2Measure = STL2Measure;
            this.TopoFaces = topoFaces;
            this.PitchPoints = pitchPoints;
        }

        public virtual void StartMeasure(int ThreadNumber)
        {
            if (this.STLCubicClassifier == null)
            {
                this.STLCubicClassifier = new CubicClassifier();
                this.STLCubicClassifier.CubicClassify(this.STL2Measure);
            }

            this.currentIndex = 0;
            this.PitchAngleErrors = new List<double[]>();
            this.PitchDistanceErrors = new List<double[]>();
            this.FaceErrors = new List<double[][,]>();

            this.P40_Singles = new List<STLMeasuringTool_P40_SingleTooth>();
            this.ThreadNumber = ThreadNumber;

            Measure();
        }

        protected virtual void Measure()
        {
            P40_Singles.Add(
                new STLMeasuringTool_P40_SingleTooth(
                    this.STL2Measure,
                    this.TopoFaces[currentIndex][0],
                    this.TopoFaces[currentIndex][1],
                    this.PitchPoints[currentIndex][0],
                    this.PitchPoints[currentIndex][1],
                    this.ThreadNumber,
                    (o, e) =>
                    {
                        this.PitchAngleErrors.Add(P40_Singles.Last().RotateAngles.Values.ToArray());
                        this.PitchDistanceErrors.Add(P40_Singles.Last().RotateDistance.Values.ToArray());
                        this.FaceErrors.Add(new double[][,] { P40_Singles.Last().Distance1, P40_Singles.Last().Distance2 });
                        if (currentIndex < TopoFaces.Count - 1)
                        {
                            currentIndex++;
                            Measure();
                        }
                        else//All Finished
                        {
                            int NRow = TopoFaces[0][0].Points.GetLength(0);
                            int NCol = TopoFaces[0][0].Points.GetLength(1);

                            double[] pitchAngleError_average = new double[] { 0, 0 };
                            double[] pitchDistanceError_average = new double[] { 0, 0 };
                            double[][,] faceError_average = new double[][,]{ ZeroMatrix(NRow, NCol), ZeroMatrix(NRow, NCol)};

                            foreach (var pitchError in PitchAngleErrors)
                            {
                                pitchAngleError_average[0] += pitchError[0];
                                pitchAngleError_average[1] += pitchError[1];
                            }
                            foreach (var pitchError in PitchDistanceErrors)
                            {
                                pitchDistanceError_average[0] += pitchError[0];
                                pitchDistanceError_average[1] += pitchError[1];
                            }
                            foreach (var faceError in FaceErrors)
                            {
                                faceError_average[0] = MatrixAdd(faceError_average[0], faceError[0]);
                                faceError_average[1] = MatrixAdd(faceError_average[1], faceError[1]);
                            }

                            pitchAngleError_average[0] /= TopoFaces.Count;
                            pitchAngleError_average[1] /= TopoFaces.Count;
                            pitchDistanceError_average[0] /= TopoFaces.Count;
                            pitchDistanceError_average[1] /= TopoFaces.Count;
                            faceError_average[0] = MatrixScale(faceError_average[0], 1.0 / TopoFaces.Count);
                            faceError_average[1] = MatrixScale(faceError_average[1], 1.0 / TopoFaces.Count);

                            this.PitchAngleError_Average = pitchAngleError_average;
                            this.PitchDistanceError_Average = pitchDistanceError_average;
                            this.FaceError_Average = faceError_average;

                            if (this.OnFinish != null)
                                this.OnFinish(this, null);
                        }
                    }
                )
            );

            P40_Singles.Last().PercentageChanged += (o, e) =>
            {
                this.Percentage = (int)((double)(P40_Singles.Count - 1) / TopoFaces.Count * 100.0 + e / (double)TopoFaces.Count);
            };
            P40_Singles.Last().STLCubicClassifier = this.STLCubicClassifier;
            P40_Singles.Last().StartMeasure();
        }
    }
}
