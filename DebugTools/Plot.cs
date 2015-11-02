using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using PTL.Geometry;
using PTL.Geometry.MathModel;

namespace PTL.DebugTools
{
    public class Plot
    {
        public PTL.Windows.DebugWindow_Plot Window;
        public System.Windows.Controls.TextBox LogTextBox;


        public Plot(Action<PTL.Windows.DebugWindow_Plot> WindowSetter = null)
        {
            Window = new Windows.DebugWindow_Plot();
            LogTextBox = Window.CreateNewLogTextBox("Log", System.Drawing.Color.White);
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

        public void ParameterPlot(Func<double, XYZ4> Function, double start, double end, uint slices, Action<PolyLine> PolyLineSetter = null)
        {
            PolyLine polyline = PTL.Geometry.PTLConvert.ToPolyLine(
                (u) => Function(u), start, end, slices, PolyLineSetter
                );

            Window.View.AddSomeThing2Show(polyline);
        }

        public void ParameterPlot(Func<double, double, XYZ4> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
        {
            TopoFace topoFace = PTL.Geometry.PTLConvert.ToTopoFace(
                (u, v) => Function(u, v),
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

        public void Log(String message = null)
        {
            this.LogTextBox.AppendText(message);
            this.LogTextBox.AppendText("\r\n");
        }

        public void Log(String message, params object[] options)
        {
            this.LogTextBox.AppendText(String.Format(message, options));
            this.LogTextBox.AppendText("\r\n");
        }

        public void Clear()
        {
            this.LogTextBox.Text = null;
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

        public void SetTitle(String title)
        {
            Window.TitleTextBlock.Text = title;
        }

        #region Invoke
        public async static Task<Plot> InvokeCreat(Action<PTL.Windows.DebugWindow_Plot> WindowSetter = null)
        {
            Plot newPlot = null;
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => newPlot = new PTL.DebugTools.Plot()));
            return newPlot;
        }
        public async Task InvokePlot2D(Func<double, double> Function, double start, double end, int slices, Action<PolyLine> PolyLineSetter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    Plot2D(Function, start, end, slices,PolyLineSetter);
                }));
        }
        public async Task InvokeParameterPlot(Func<double, XYZ4> Function, double start, double end, uint slices, Action<PolyLine> PolyLineSetter = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() => {
                    ParameterPlot(Function, start, end, slices, PolyLineSetter);
                }));
        }
        public async Task InvokeParameterPlot(Func<double, double, XYZ4> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
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
        public async Task InvokeLog(String message = null)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Log(message);
                }));
        }
        public async Task InvokeLog(String message = null, params object[] options)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Log(message, options);
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
        public async Task InvokeSetTitle(String title)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(()=>SetTitle(title)));
        }
        #endregion Invoke
    }
}
