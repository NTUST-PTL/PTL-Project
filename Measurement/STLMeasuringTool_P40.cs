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
    public class STLMeasuringTool_P40
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
                    PercentageChanged?.Invoke(this, this.percentage);
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
                            double[][,] faceError_average = new double[][,]{ NewZeroMatrix(NRow, NCol), NewZeroMatrix(NRow, NCol)};

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
                                faceError_average[0] = Add(faceError_average[0], faceError[0]);
                                faceError_average[1] = Add(faceError_average[1], faceError[1]);
                            }

                            pitchAngleError_average[0] /= TopoFaces.Count;
                            pitchAngleError_average[1] /= TopoFaces.Count;
                            pitchDistanceError_average[0] /= TopoFaces.Count;
                            pitchDistanceError_average[1] /= TopoFaces.Count;
                            faceError_average[0] = Mult(faceError_average[0], 1.0 / TopoFaces.Count);
                            faceError_average[1] = Mult(faceError_average[1], 1.0 / TopoFaces.Count);

                            this.PitchAngleError_Average = pitchAngleError_average;
                            this.PitchDistanceError_Average = pitchDistanceError_average;
                            this.FaceError_Average = faceError_average;

                            this.OnFinish?.Invoke(this, null);
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
