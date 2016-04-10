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
    public abstract class STLMeasuringTool
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
}
