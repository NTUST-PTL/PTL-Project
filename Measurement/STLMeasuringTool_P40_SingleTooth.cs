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

        public override List<VectorD> MeasurePointNormals
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
                TopoFace1.Transform(NewRotateMatrix4(Axis.Z, rotateAngle));
                rotateAngle = RotateAngles[this.PitchPoint2];
                TopoFace2.Transform(NewRotateMatrix4(Axis.Z, rotateAngle));
                Solve();
            }
        }

        protected override void Solve()
        {
            //提取所有拓樸點
            XYZ4[] allTopoPoints = new XYZ4[this.TopoFace1.Points.Length + this.TopoFace2.Points.Length];
            VectorD[] allTopoPointNormals = new VectorD[this.TopoFace1.Normals.Length + this.TopoFace2.Normals.Length];
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
                VectorD[] PartoalNormals = new VectorD[sliceIndex[i + 1] - sliceIndex[i]];
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
                this.MeasurePointNormals = new List<VectorD>();
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

                this.Distance1 = (double[,])Reshape(
                    (double[])Take(distanceAll, new int[] { 0 }, new int[] { count / 2 - 1 }),
                    GetDimensions(this.TopoFace1.Points));
                this.Distance2 = (double[,])Reshape(
                    (double[])Take(distanceAll, new int[] { count / 2 }, new int[] { count - 1 }),
                    GetDimensions(this.TopoFace1.Points));

                //設定狀態
                this.IsBusy = false;
                this.FinishTime = DateTime.Now;

                OnFinish?.Invoke(this, new EventArgs());
            }
        }
    }
}
