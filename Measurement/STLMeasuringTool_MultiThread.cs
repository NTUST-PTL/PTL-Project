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
        public virtual List<VectorD> MeasurePointNormals { get; set; }
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
        public STLMeasuringTool_MultiThread(STL STL2Measure, List<XYZ4> P, List<VectorD> N, int ThreadNumber, EventHandler OnFinish, STLMeasuringTool objSTLMeasurement)
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
                        VectorD[] PartoalNormals = new VectorD[sliceIndex[i + 1] - sliceIndex[i]];
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

                OnFinish?.Invoke(this, new EventArgs());
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
}
