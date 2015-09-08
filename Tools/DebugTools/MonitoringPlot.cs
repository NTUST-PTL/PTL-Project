using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using PTL.Geometry;

namespace PTL.Tools.DebugTools
{
    public class MonitoringPlot
    {
        public PTL.Windows.DebugWindow_Plot Window;

        public Dictionary<String, PolyLine> MonitorRecords = new Dictionary<String, PolyLine>();

        /// <summary>
        /// If set to zero, there is no limits for MaxRecords.
        /// </summary>
        public UInt32 MaxRecords = 100;

        /// <summary>
        /// Refresh Interval (ms)
        /// </summary>
        public uint RefreshInterval = 500;

        public System.Timers.Timer timer = new System.Timers.Timer();

        public MonitoringPlot(Action<PTL.Windows.DebugWindow_Plot> WindowSetter = null)
        {
            #region
            Window = new Windows.DebugWindow_Plot();
            Window.View.AutoScale = true;
            if (WindowSetter != null)
                WindowSetter(Window);
            Window.Show();
            InitializeTimer();
            #endregion
        }

        protected void InitializeTimer()
        {
            this.timer.Interval = RefreshInterval;
            this.timer.AutoReset = true;
            this.timer.Elapsed += timer_Elapsed;
            this.timer.Start();
        }

        protected void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Window.View.ReplaceThing2Show(MonitorRecords.Values.ToArray());
        }

        public void AddNewMonitor(String Name = "", Action<PolyLine> Setter = null)
        {
            PolyLine newRecord = new PolyLine();

            if (String.IsNullOrEmpty(Name))
                Name = "Monitor " + MonitorRecords.Count;

            if (Setter != null)
                Setter(newRecord);

            MonitorRecords.Add(Name, newRecord);
        }

        public void RemoveMonitor(String Name)
        {
            MonitorRecords.Remove(Name);
        }

        public void RemoveMonitor(int index)
        {
            String key = MonitorRecords.Keys.ElementAt(index);
            RemoveMonitor(key);
        }

        public void Push(double value, int index)
        {
            PolyLine targetMonitor = MonitorRecords.Values.ElementAt(index);
            int recordCount = targetMonitor.Points.Count;
            if (MaxRecords > 0 && recordCount > MaxRecords)
                targetMonitor.Points.RemoveAt(0);
            if (recordCount == 0)
            {
                targetMonitor.Points.Add(new PointD(0, value, 0));
            }
            else
            {
                PointD lastValue = targetMonitor.Points.Last();
                targetMonitor.Points.Add(new PointD(lastValue.X + 1, value, 0));
            }
        }

        public void Push(double value, String MonitorName)
        {
            PolyLine targetMonitor = MonitorRecords[MonitorName];
            int recordCount = targetMonitor.Points.Count;
            if (MaxRecords > 0 && recordCount > MaxRecords)
                targetMonitor.Points.RemoveAt(0);
            if (recordCount == 0)
            {
                targetMonitor.Points.Add(new PointD(0, value, 0));
            }
            else
            {
                PointD lastValue = targetMonitor.Points.Last();
                targetMonitor.Points.Add(new PointD(lastValue.X + 1, value, 0));
            }
        }

        public void AddSomethings(params ICanPlotInOpenGL[] polyline)
        {
            Window.View.AddSomeThing2Show(polyline);
        }

        public void RemoveSomethings(params ICanPlotInOpenGL[] polyline)
        {
            Window.View.RemoveSomeThing2Show(polyline);
        }

        public void Clear()
        {
            Window.View.ClearThings2Show();
        }

        public void Close()
        {
            Action BeginPlot = () =>
            {
                if (Window != null)
                    this.Window.Close();
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);

        }

        #region Invoke
        public async static Task<MonitoringPlot> InvokeCreate()
        {
            MonitoringPlot Monitor = null;
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => Monitor = new MonitoringPlot()));
            return Monitor;
        }
        public async Task InvokeAddNewMonitor(String Name = "", Action<PolyLine> Setter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     AddNewMonitor(Name, Setter);
                 }));
        }
        public async Task InvokeRemoveMonitor(String Name)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     RemoveMonitor(Name);
                 }));
        }
        public async Task InvokeRemoveMonitor(int index)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     RemoveMonitor(index);
                 }));
        }
        public async Task InvokePush(double value, int index)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     Push(value, index);
                 }));
        }
        public async Task InvokePush(double value, String MonitorName)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     Push(value, MonitorName);
                 }));
        }
        public async Task InvokeAddSomethings(params ICanPlotInOpenGL[] polyline)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     AddSomethings(polyline);
                 }));
        }
        public async Task InvokeRemoveSomethings(params ICanPlotInOpenGL[] polyline)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     RemoveSomethings(polyline);
                 }));
        }
        public async Task InvokeClear()
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     Clear();
                 }));
        }
        public async Task InvokeClose()
        {
            await Application.Current.Dispatcher.BeginInvoke(
                 new Action(() =>
                 {
                     Close();
                 }));
        }
        #endregion
    }
}
