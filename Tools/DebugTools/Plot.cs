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

        public Plot()
        {
            Window = new Windows.DebugWindow_Plot();
            Window.Show();
        }

        public Plot(Action<PTL.Windows.DebugWindow_Plot> WindowSetter)
        {
            Action CreatDebugForm = () =>
            {
                #region
                Window = new Windows.DebugWindow_Plot();
                WindowSetter(Window);
                Window.Show();
                #endregion
            };
            Application.Current.Dispatcher.BeginInvoke(CreatDebugForm);
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
                for (double x = start; x <= end; x += dx)
                    polyline.AddPoint(new PointD(x, Function(x), 0));
                if (PolyLineSetter != null)
                    PolyLineSetter(polyline);
                else
                    polyline.Color = System.Drawing.Color.LawnGreen;
            }
            #endregion

            Action BeginPlot = () =>
            {
                #region
                if (Window != null)
                    Window.View.AddSomeThing2Show(polyline);
                #endregion
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
        }

        public void ParameterPlot(Func<double, PointD> Function, double start, double end, int slices, Action<PolyLine> PolyLineSetter = null)
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
                for (double x = start; x <= end; x += dx)
                    polyline.AddPoint(Function(x));
                if (PolyLineSetter != null)
                    PolyLineSetter(polyline);
                else
                    polyline.Color = System.Drawing.Color.LawnGreen;
            }
            #endregion

            Action BeginPlot = () =>
            {
                #region
                if (Window != null)
                    Window.View.AddSomeThing2Show(polyline);
                #endregion
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
        }

        public void PlotLine(PolyLine polyline)
        {
            Action BeginPlot = () =>
            {
                #region
                if (Window != null)
                    Window.View.AddSomeThing2Show(polyline);
                #endregion
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
        }

        public void Plot2D(Func<double, PointD> Function, double start, double end, int slices, Action<PolyLine> PolyLineSetter = null)
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
                for (double x = start; x <= end; x += dx)
                    polyline.AddPoint(Function(x));
                if (PolyLineSetter != null)
                    PolyLineSetter(polyline);
                else
                    polyline.Color = System.Drawing.Color.LawnGreen;
            }
            #endregion

            Action BeginPlot = () =>
            {
                #region
                if (Window != null)
                    Window.View.AddSomeThing2Show(polyline);
                #endregion
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
        }

        public void ParameterPlot(Func<double, double, PointD> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
        {
            TopoFace topoFace = null;
            #region 主要計算
            if (Function != null && xslices > 0 && yslices > 0)
	        {
                uint NRow = xslices + 1;
                uint NCol = yslices + 1;
                topoFace = new TopoFace() { Points = new PointD[NRow, NCol] };

                //確認方向
                if (xstart > xend)
                {
                    double newEnd = xstart;
                    xstart = xend;
                    xend = newEnd;
                }
                if (ystart > yend)
                {
                    double newEnd = ystart;
                    ystart = yend;
                    yend = newEnd;
                }


                double dx = (xend - xstart) / xslices;
                double dy = (yend - ystart) / yslices;
                int i = 0;
                for (double x = xstart; x <= xend; x += dx)
                {
                    int j = 0;
                    for (double y = ystart; y <= yend; y += dy)
                    {
                        topoFace.Points[i, j] = Function(x, y);
                        j++;
                    }
                    i++; 
                }
                topoFace.SovleNormalVector();
                if (TopoFaceSetter != null)
                    TopoFaceSetter(topoFace);
                else
                    topoFace.Color = System.Drawing.Color.LawnGreen;
	        }
            
            #endregion

            Action BeginPlot = () =>
            {
                #region
                if (Window != null)
                    Window.View.AddSomeThing2Show(topoFace);
                #endregion
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
        }

        public void Clear()
        {
            Action BeginPlot = () =>
            {
                #region
                if (Window != null)
                    Window.View.ClearThings2Show();
                #endregion
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
        }

        public void Close()
        {
            Action BeginPlot = () =>
            {
                #region
                if (Window != null)
                    this.Window.Close();
                #endregion
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
            
        }
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

        public MonitoringPlot()
        {
            Window = new Windows.DebugWindow_Plot();
            Window.View.AutoScale = true;
            Window.Show();
            InitializeTimer();
        }

        public MonitoringPlot(Action<PTL.Windows.DebugWindow_Plot> WindowSetter)
        {
            #region
            Window = new Windows.DebugWindow_Plot();
            Window.View.AutoScale = true;
            WindowSetter(Window);
            Window.Show();
            InitializeTimer();
            #endregion
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

        public void Clear()
        {
            this.MonitorRecords.Clear();
            Action BeginPlot = () =>
            {
                if (Window != null)
                    Window.View.ClearThings2Show();
            };

            //因為牽扯到視窗元素，所以需委托應用程式的STA執行緒來執行
            Application.Current.Dispatcher.BeginInvoke(BeginPlot);
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
