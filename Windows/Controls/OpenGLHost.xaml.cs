using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PTL.Geometry;
using PTL.Data;

namespace PTL.Windows.Controls
{
    /// <summary>
    /// OpenGLHost.xaml 的互動邏輯
    /// </summary>
    public partial class OpenGLHost : UserControl
    {
        private System.Timers.Timer AnimateTimer = new System.Timers.Timer();

        public Color GridColor1
        {
            get { return (Color)GetValue(GridColor1Property); }
            set { SetValue(GridColor1Property, value); }
        }
        public Color GridColor2
        {
            get { return (Color)GetValue(GridColor2Property); }
            set { SetValue(GridColor2Property, value); }
        }
        public bool AutoScale
        {
            get { return (bool)GetValue(AutoScaleProperty); }
            set { SetValue(AutoScaleProperty, value); }
        }
        public bool AutoRefresh
        {
            get { return (bool)GetValue(AutoRefreshProperty); }
            set { SetValue(AutoRefreshProperty, value); }
        }
        public bool IsSmoothOn
        {
            get { return (bool)GetValue(IsSmoothOnProperty); }
            set { SetValue(IsSmoothOnProperty, value); }
        }
        public bool IsLightOn
        {
            get { return (bool)GetValue(IsLightOnProperty); }
            set { SetValue(IsLightOnProperty, value); }
        }
        public bool IsAnimate
        {
            get { return (bool)GetValue(IsAnimateProperty); }
            set { SetValue(IsAnimateProperty, value); }
        }
        /// <summary>
        /// 設定畫面時間間隔，預設 33 ms。
        /// </summary>
        public int FrameInterval
        {
            get { return (int)GetValue(FrameIntervalProperty); }
            set { SetValue(FrameIntervalProperty, value); }
        }

        
        // Using a DependencyProperty as the backing store for GridLineColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColor1Property =
            DependencyProperty.Register("GridColor1", typeof(Color), typeof(OpenGLHost), new PropertyMetadata(Colors.LightGray, OnGridColor1ChangedCallback));
        // Using a DependencyProperty as the backing store for GridColor2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColor2Property =
            DependencyProperty.Register("GridColor2", typeof(Color), typeof(OpenGLHost), new PropertyMetadata(Colors.LightGray, OnGridColor2ChangedCallback));
        // Using a DependencyProperty as the backing store for AutoScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScaleProperty =
            DependencyProperty.Register("AutoScale", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(false, OnAdaptedPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for AutoRefresh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoRefreshProperty =
            DependencyProperty.Register("AutoRefresh", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for Smoothing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSmoothOnProperty =
            DependencyProperty.Register("IsSmoothOn", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for IsLightOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLightOnProperty =
            DependencyProperty.Register("IsLightOn", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for IsAnimate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAnimateProperty =
            DependencyProperty.Register("IsAnimate", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(false, OnAdaptedPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for FrameInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameIntervalProperty =
            DependencyProperty.Register("FrameInterval", typeof(int), typeof(OpenGLHost), new PropertyMetadata(33, OnAdaptedPropertyChangedCallback));


        static OpenGLHost()
        {
            DataContextProperty.OverrideMetadata(typeof(OpenGLHost), new FrameworkPropertyMetadata(OnDataContextChangedCallback));
            BackgroundProperty.OverrideMetadata(typeof(OpenGLHost), new FrameworkPropertyMetadata( System.Windows.Media.Brushes.DimGray, OnBackgroundChangedCallback));
            ForegroundProperty.OverrideMetadata(typeof(OpenGLHost), new FrameworkPropertyMetadata( System.Windows.Media.Brushes.LightGray, OnForegroundChangedCallback));
        }

        public OpenGLHost()
        {
            InitializeComponent();
        }

        private static void OnDataContextChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as OpenGLHost).OpenGLViewer.View.Things2Show = e.NewValue as ExObservableCollection<ICanPlotInOpenGL>;
        }

        private static void OnBackgroundChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PTL.Data.Conveters.ColorConverter converter = new PTL.Data.Conveters.ColorConverter();
            (o as OpenGLHost).OpenGLViewer.View.BackgroundColor = (System.Drawing.Color)converter.Convert( e.NewValue, typeof(System.Drawing.Color), null, null);
        }

        private static void OnForegroundChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PTL.Data.Conveters.ColorConverter converter = new PTL.Data.Conveters.ColorConverter();
            (o as OpenGLHost).OpenGLViewer.View.GraduationColor = (System.Drawing.Color)converter.Convert(e.NewValue, typeof(System.Drawing.Color), null, null);
        }

        private static void OnGridColor1ChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PTL.Data.Conveters.ColorConverter converter = new PTL.Data.Conveters.ColorConverter();
            (o as OpenGLHost).OpenGLViewer.View.GridColor1 = (System.Drawing.Color)converter.Convert(e.NewValue, typeof(System.Drawing.Color), null, null);
        }

        private static void OnGridColor2ChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PTL.Data.Conveters.ColorConverter converter = new PTL.Data.Conveters.ColorConverter();
            (o as OpenGLHost).OpenGLViewer.View.GridColor2 = (System.Drawing.Color)converter.Convert(e.NewValue, typeof(System.Drawing.Color), null, null);
        }

        private static void OnAdaptedPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as OpenGLHost).OpenGLViewer.View.GetType().GetProperty(e.Property.Name).SetValue(o, e.NewValue);
        }
    }
}
