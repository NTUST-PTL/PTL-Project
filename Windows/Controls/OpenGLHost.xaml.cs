using System;
using System.Collections.Generic;
using System.IO;
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
using PTL.Definitions;

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
        public Dimension Dimension
        {
            get { return (Dimension)GetValue(DimensionProperty); }
            set { SetValue(DimensionProperty, value); }
        }
        public bool PlotGrid
        {
            get { return (bool)GetValue(PlotGridProperty); }
            set { SetValue(PlotGridProperty, value); }
        }
        public bool PlotGraduation
        {
            get { return (bool)GetValue(PlotGraduationProperty); }
            set { SetValue(PlotGraduationProperty, value); }
        }
        public double GraduationHeight
        {
            get { return (double)GetValue(GraduationHeightProperty); }
            set { SetValue(GraduationHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridLineColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColor1Property =
            DependencyProperty.Register("GridColor1", typeof(Color), typeof(OpenGLHost), new PropertyMetadata(Colors.LightGray, OnGridColor1ChangedCallback));
        // Using a DependencyProperty as the backing store for GridColor2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColor2Property =
            DependencyProperty.Register("GridColor2", typeof(Color), typeof(OpenGLHost), new PropertyMetadata(Colors.LightGray, OnGridColor2ChangedCallback));
        // Using a DependencyProperty as the backing store for AutoScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScaleProperty =
            DependencyProperty.Register("AutoScale", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(false, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for AutoRefresh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoRefreshProperty =
            DependencyProperty.Register("AutoRefresh", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for Smoothing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSmoothOnProperty =
            DependencyProperty.Register("IsSmoothOn", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for IsLightOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLightOnProperty =
            DependencyProperty.Register("IsLightOn", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for IsAnimate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAnimateProperty =
            DependencyProperty.Register("IsAnimate", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(false, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for FrameInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameIntervalProperty =
            DependencyProperty.Register("FrameInterval", typeof(int), typeof(OpenGLHost), new PropertyMetadata(33, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for Dimension.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DimensionProperty =
            DependencyProperty.Register("Dimension", typeof(Dimension), typeof(OpenGLHost), new PropertyMetadata(Dimension.ThreeDimension, OnAdaptedWindowPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for Dimension.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotGridProperty =
            DependencyProperty.Register("PlotGrid", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for Dimension.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotGraduationProperty =
            DependencyProperty.Register("PlotGraduation", typeof(bool), typeof(OpenGLHost), new PropertyMetadata(true, OnAdaptedViewPropertyChangedCallback));
        // Using a DependencyProperty as the backing store for FrameInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraduationHeightProperty =
            DependencyProperty.Register("GraduationHeight", typeof(double), typeof(OpenGLHost), new PropertyMetadata(20.0, OnAdaptedViewPropertyChangedCallback));



        static OpenGLHost()
        {
            DataContextProperty.OverrideMetadata(typeof(OpenGLHost), new FrameworkPropertyMetadata(OnDataContextChangedCallback));
            BackgroundProperty.OverrideMetadata(typeof(OpenGLHost), new FrameworkPropertyMetadata( System.Windows.Media.Brushes.DimGray, OnBackgroundChangedCallback));
            ForegroundProperty.OverrideMetadata(typeof(OpenGLHost), new FrameworkPropertyMetadata( System.Windows.Media.Brushes.LightGray, OnForegroundChangedCallback));
            FontSizeProperty.OverrideMetadata(typeof(OpenGLHost), new FrameworkPropertyMetadata(20.0, OnFontSizeChangedCallback));
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

        private static void OnFontSizeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as OpenGLHost).GraduationHeight = (double)e.NewValue;
        }

        private static void OnAdaptedViewPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var view = (o as OpenGLHost).OpenGLViewer.View;
            view.GetType().GetProperty(e.Property.Name).SetValue(view, e.NewValue);
        }

        private static void OnAdaptedWindowPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var viewer = (o as OpenGLHost).OpenGLViewer;
            viewer.GetType().GetProperty(e.Property.Name).SetValue(viewer, e.NewValue);
        }

        public void CaptureStaticImage()
        {
            //double width = this.ActualWidth;
            //double height = this.ActualHeight;
            //RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            //DrawingVisual dv = new DrawingVisual();
            //using (DrawingContext dc = dv.RenderOpen())
            //{
            //    VisualBrush vb = new VisualBrush(this);
            //    dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(width, height)));
            //}
            //bmpCopied.Render(dv);

            //var bitmapImage = new BitmapImage();
            //var bitmapEncoder = new PngBitmapEncoder();
            //bitmapEncoder.Frames.Add(BitmapFrame.Create(bmpCopied));

            //using (var stream = new MemoryStream())
            //{
            //    bitmapEncoder.Save(stream);
            //    stream.Seek(0, SeekOrigin.Begin);

            //    bitmapImage.BeginInit();
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapImage.StreamSource = stream;
            //    bitmapImage.EndInit();
            //}

            var bmp = new System.Drawing.Bitmap(this.OpenGLViewer.Width, this.OpenGLViewer.Height);
            OpenGLViewer.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            this.ReplaceImage.Source = image;
        }

        public void ShowStaticImage()
        {
            this.ReplaceImage.Visibility = System.Windows.Visibility.Visible;
            this.WindowsFormsHost.Visibility = System.Windows.Visibility.Hidden;
        }

        public void HideStaticImage()
        {
            this.WindowsFormsHost.Visibility = System.Windows.Visibility.Visible;
            this.ReplaceImage.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
