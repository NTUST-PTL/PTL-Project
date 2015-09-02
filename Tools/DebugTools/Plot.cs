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
    public class Plot
    {
        public PTL.Windows.DebugWindow_Plot Window;

        public Plot(Action<PTL.Windows.DebugWindow_Plot> WindowSetter = null)
        {
            Window = new Windows.DebugWindow_Plot();
            if (WindowSetter != null)
                WindowSetter(Window);
            Window.Show();
        }

        public void Plot2D(Func<double, double> Function, double start, double end, int slices, Action<PolyLine> PolyLineSetter = null)
        {
            #region 主要計算
            PolyLine polyline = new PolyLine();
            
            if (start > end)
            {
                double newEnd = start;
                start = end;
                end = newEnd;
            }
            double dx = (end - start) / slices;
            if (Function != null)
            {
                double x = start;
                for (int i = 0; i < slices + 1; i++)
                {
                    polyline.AddPoint(new PointD(x, Function(x), 0));
                    x += dx;
                }
                if (PolyLineSetter != null)
                    PolyLineSetter(polyline);
                else
                    polyline.Color = System.Drawing.Color.LawnGreen;
            }
            #endregion

            Window.View.AddSomeThing2Show(polyline);
        }

        public void ParameterPlot(Func<double, double[]> Function, double start, double end, uint slices, Action<PolyLine> PolyLineSetter = null)
        {
            PolyLine polyline = PTL.Geometry.PTLConvert.ToPolyLine(
                Function, start, end, slices, PolyLineSetter
                );

            Window.View.AddSomeThing2Show(polyline);
        }

        public void ParameterPlot(Func<double, PointD> Function, double start, double end, uint slices, Action<PolyLine> PolyLineSetter = null)
        {
            PolyLine polyline = PTL.Geometry.PTLConvert.ToPolyLine(
                Function, start, end, slices, PolyLineSetter
                );

            Window.View.AddSomeThing2Show(polyline);
        }

        public void ParameterPlot(Func<double, double, PointD> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
        {
            TopoFace topoFace = PTL.Geometry.PTLConvert.ToTopoFace(
                Function,
                xstart, xend, xslices,
                ystart, yend, yslices,
                TopoFaceSetter);

            Window.View.AddSomeThing2Show(topoFace);
        }

        public void ParameterPlot(Func<double, double, double[]> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
        {
            TopoFace topoFace = PTL.Geometry.PTLConvert.ToTopoFace(
                Function,
                xstart, xend, xslices,
                ystart, yend, yslices,
                TopoFaceSetter);
            Window.View.AddSomeThing2Show(topoFace);
        }

        public void AddSomethings(params ICanPlotInOpenGL[] things)
        {
            Window.View.AddSomeThing2Show(things);
        }

        public void ReplaceAll(params ICanPlotInOpenGL[] things)
        {
            Window.View.ReplaceThing2Show(things);
        }

        public void RemoveSomthing(params ICanPlotInOpenGL[] things)
        {
            Window.View.RemoveSomeThing2Show(things);
        }

        public void Clear()
        {
            Window.View.ClearThings2Show();
        }

        public void Close()
        {
            Action BeginPlot = () =>
            {
                this.Window.Close();
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
        }

        #region Invoke
        public async static Task<Plot> InvokeCreat(Action<PTL.Windows.DebugWindow_Plot> WindowSetter = null)
        {
            Plot newPlot = null;
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => newPlot = new PTL.Tools.DebugTools.Plot()));
            return newPlot;
        }
        public async Task InvokePlot2D(Func<double, double> Function, double start, double end, int slices, Action<PolyLine> PolyLineSetter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    Plot2D(Function, start, end, slices,PolyLineSetter);
                }));
        }
        public async Task InvokeParameterPlot(Func<double, double[]> Function, double start, double end, uint slices, Action<PolyLine> PolyLineSetter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    ParameterPlot(Function, start, end, slices, PolyLineSetter);
                }));
        }
        public async Task InvokeParameterPlot(Func<double, PointD> Function, double start, double end, uint slices, Action<PolyLine> PolyLineSetter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    ParameterPlot(Function, start, end, slices, PolyLineSetter);
                }));
        }
        public async Task InvokeParameterPlot(Func<double, double, PointD> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    ParameterPlot(Function, xstart, xend, xslices, ystart, yend, yslices, TopoFaceSetter);
                }));
        }
        public async Task InvokeParameterPlot(Func<double, double, double[]> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    ParameterPlot(Function, xstart, xend, xslices, ystart, yend, yslices, TopoFaceSetter);
                }));
        }
        public async Task InvokeAddSomethings(params ICanPlotInOpenGL[] things)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    AddSomethings(things);
                }));
        }
        public async Task InvokeRemoveSomthing(params ICanPlotInOpenGL[] things)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    RemoveSomthing(things);
                }));
        }
        public async Task InvokeReplaceAll(params ICanPlotInOpenGL[] things)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    ReplaceAll(things);
                }));
        }
        public async Task InvokeClear()
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(Clear));
        }
        public async Task InvokeClose()
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(Close));
        }
        #endregion Invoke
    }

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
            WindowSetter(Window);
            Window.Show();
            InitializeTimer();
            #endregion
        }

        public static MonitoringPlot InvokeCreat(Action<PTL.Windows.DebugWindow_Plot> WindowSetter = null)
        {
            return Application.Current.Dispatcher.Invoke(new Func<MonitoringPlot>(() => new PTL.Tools.DebugTools.MonitoringPlot())); ;
        }

        public void InitializeTimer()
        {
            this.timer.Interval = RefreshInterval;
            this.timer.AutoReset = true;
            this.timer.Elapsed += timer_Elapsed;
            this.timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Window.View.ReplaceThing2Show(MonitorRecords.Values.ToArray());
        }

        public void AddNewMonitor(String Name = "", Action<PolyLine> Setter = null)
        {
            Action action = () =>
            {
                PolyLine newRecord = new PolyLine();

                if (String.IsNullOrEmpty(Name))
                    Name = "Monitor " + MonitorRecords.Count;

                if (Setter != null)
                    Setter(newRecord);

                MonitorRecords.Add(Name, newRecord);
            };
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        public void RemoveMonitor(String Name)
        {
            Action action = () =>
            {
                MonitorRecords.Remove(Name);
            };
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        public void RemoveMonitor(int index)
        {
            Action action = () =>
            {
                String key = MonitorRecords.Keys.ElementAt(index);
                RemoveMonitor(key);
            };
            Application.Current.Dispatcher.BeginInvoke(action);
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
    }
}
