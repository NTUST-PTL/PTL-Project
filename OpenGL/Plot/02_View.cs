using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsGL.OpenGL;
using Tao.Platform.Windows;
using PTL;
using PTL.Geometry;
using PTL.Geometry.MathModel;
using PTL.Definitions;
using PTL.OpenGL;
using PTL.OpenGL.Plot;
using PTL.Windows.Controls;
using PTL.Base;

namespace PTL.OpenGL.Plot
{
    public interface IView
    {
        void Paint(object sender, System.Windows.Forms.PaintEventArgs e);
    }


    /// <summary>
    /// 可提供OpenGL視窗所需的Paint方法
    /// 將指定的ICanPlotInOpenGL類別(屬性名稱Things2Show)轉換至OpenGL繪圖方法
    /// 可自動計算內容範圍，並在視野範圍內顯示格線及刻度
    /// </summary>
    public class OpenGLView :OpenGL.Plot.PlotSub, IView
    {
        #region 欄位
        protected OpenGLWindow openGLWindow;
        public Source<HashSet<ICanPlotInOpenGL>> Things2Show = new Source<HashSet<ICanPlotInOpenGL>>();
        protected Layer gridLayer;
        protected Layer graduationLayer;
        protected Double fRange;

        //邊框、格線、刻度欄位
        protected Double minGridPitch = 30; //unit：pixel
        protected Double[] gridPitchOption = new double[] { 1, 2, 5 };//格線間距選項
        protected XYZ4 centerPoint = new XYZ4();
        protected XYZ4[] geometryBoundary;
        protected XYZ4[] viewBoundary;
        protected XYZ4 geometrySize;
        protected XYZ4 gridSize;
        protected Double xGridPitch;
        protected Double yGridPitch;
        protected Double graphicRangeX;
        protected Double graphicRangeY;
        protected double xScale;
        protected double yScale;
        protected double zScale;
        protected System.Timers.Timer _UpdateTimer;
        protected bool _Boundary_Changed_NeedCheck;
        #endregion 欄位





        /// <summary>
        /// 連結的OpenGL視窗
        /// </summary>
        public OpenGLWindow OpenGLWindow
        { 
            get { return openGLWindow; } 
            private set { 
                this.openGLWindow = value;
                this.openGLWindow.Paint += this.Paint;
                this.openGLWindow.SizeChanged += openGLWindow_SizeChanged;
            }
        }
        void openGLWindow_SizeChanged(object sender, EventArgs e)
        {
            CheckRange();
            if (this.AutoScale == true)
            {
                CheckScale();
            }
        }
        /// <summary>
        /// 增加顯示的物件
        /// 需繼承ICanPlotInOpenGL介面
        /// 若同時亦繼承IHaveBoundary則可自動計算顯示範圍
        /// </summary>
        /// <param name="someThing2Show">欲顯示的物件</param>
        public void AddSomeThing2Show(params ICanPlotInOpenGL[] someThings2Show)
        {
            foreach (var item in someThings2Show)
            {
                if (item != null)
                {
                    Things2Show.V.Add(item);
                    if (item is IHaveBoundary)
                        _Boundary_Changed_NeedCheck = true;
                }
            }
            Update();
        }
        /// <summary>
        /// 移除顯示的物件
        /// 需繼承ICanPlotInOpenGL介面
        /// 若同時亦繼承IHaveBoundary則可自動計算顯示範圍
        /// </summary>
        /// <param name="someThing2Show">不想顯示的物件</param>
        public void RemoveSomeThing2Show(params ICanPlotInOpenGL[] someThingsDontWant2Show)
        {
            foreach (var item in someThingsDontWant2Show)
            {
                if (item != null)
                {
                    Things2Show.V.Remove(item);
                    if (item is IHaveBoundary)
                        _Boundary_Changed_NeedCheck = true;
                }
            }
            Update();
        }
        /// <summary>
        /// 清除舊有物件並增加新的顯示物件
        /// 需繼承ICanPlotInOpenGL介面
        /// 若同時亦繼承IHaveBoundary則可自動計算顯示範圍
        /// </summary>
        /// <param name="someThing2Show">欲顯示的物件</param>
        public void ReplaceThing2Show(params ICanPlotInOpenGL[] someThings2Show)
        {
            this.Things2Show.V.Clear();
            foreach (var item in someThings2Show)
            {
                if (item != null)
                {
                    Things2Show.V.Add(item);
                    if (item is IHaveBoundary)
                        _Boundary_Changed_NeedCheck = true;
                }
            }
            Update();
        }
        /// <summary>
        /// 清除顯示的物件
        /// </summary>
        public void ClearThings2Show()
        {
            Things2Show.V = new HashSet<ICanPlotInOpenGL>();
            CheckBoundary();
            if (AutoRefresh)
            {
                OpenGLWindow.Refresh();
            }
        }

        /// <summary>
        /// 增加顯示的物件
        /// 需繼承ICanPlotInOpenGL介面
        /// 若同時亦繼承IHaveBoundary則可自動計算顯示範圍
        /// </summary>
        /// <param name="someThing2Show">欲顯示的物件</param>
        public async Task InvokeAddSomeThing2Show(params ICanPlotInOpenGL[] someThings2Show)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AddSomeThing2Show(someThings2Show);
            }));
        }
        /// <summary>
        /// 移除顯示的物件
        /// 需繼承ICanPlotInOpenGL介面
        /// 若同時亦繼承IHaveBoundary則可自動計算顯示範圍
        /// </summary>
        /// <param name="someThing2Show">不想顯示的物件</param>
        public async Task InvokeRemoveSomeThing2Show(params ICanPlotInOpenGL[] someThingsDontWant2Show)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                RemoveSomeThing2Show(someThingsDontWant2Show);
            }));
        }
        /// <summary>
        /// 清除舊有物件並增加新的顯示物件
        /// 需繼承ICanPlotInOpenGL介面
        /// 若同時亦繼承IHaveBoundary則可自動計算顯示範圍
        /// </summary>
        /// <param name="someThing2Show">欲顯示的物件</param>
        public async Task InvokeReplaceThing2Show(params ICanPlotInOpenGL[] someThings2Show)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ReplaceThing2Show(someThings2Show);
            }));
        }
        /// <summary>
        /// 清除顯示的物件
        /// </summary>
        public async Task InvokeClearThings2Show()
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(ClearThings2Show));
        }

        private void CheckBoundary()
        {
            this.geometryBoundary = new XYZ4[2];
            foreach (var item in Things2Show.V)
            {
                if (item is IHaveBoundary)
                {
                    XYZ4[] itemboundary = (item as IHaveBoundary).Boundary;
                    if (this.geometryBoundary[0] == null && itemboundary != null)
                        this.geometryBoundary = (item as IHaveBoundary).Boundary;
                    else
                    {
                        if (itemboundary != null)
                        {
                            Compare_Boundary(this.geometryBoundary, itemboundary[0]);
                            Compare_Boundary(this.geometryBoundary, itemboundary[1]);
                        }
                    }

                    if (this.geometryBoundary[0] != null && this.geometryBoundary[1] != null)
                    {
                        geometrySize = this.geometryBoundary[1] - this.geometryBoundary[0];

                        centerPoint = (this.geometryBoundary[0] + this.geometryBoundary[1]) / 2.0;

                        #region 設定objMouseControl.Range：scale = 1 時，畫面中心點至邊框的最近處之間的距離
                        if (this.AutoScale == true)
                        {
                            CheckScale();
                        }
                        graphicRangeX = geometrySize.X * (1.0 + BoundaryGap) / 2.0 * this.XScale;
                        graphicRangeY = geometrySize.Y * (1.0 + BoundaryGap) / 2.0 * this.YScale;
                        #endregion
                        CheckRange();
                    }
                }
            }
            _Boundary_Changed_NeedCheck = false;
        }
        private void CheckScale()
        {
            this.AutoRefresh = false;
            this.XScale = 1.0;
            if (geometrySize != null)
                this.YScale = ((double)openGLWindow.Height / openGLWindow.Width) / (geometrySize.Y / geometrySize.X);
            this.AutoRefresh = true;
        }
        private void CheckRange()
        {
            #region 檢查Range
            if (this.graphicRangeY / this.graphicRangeX < ((double)(this.openGLWindow.Height)) / ((double)(this.openGLWindow.Width)))
                this.Range = this.graphicRangeX;
            else
                this.Range = this.graphicRangeY * (((double)(this.openGLWindow.Width)) / ((double)(this.openGLWindow.Height)));
            #endregion 檢查Range
            if (this.AutoScale == true)
            {
                CheckScale();
            }
        }
        private void Update()
        {
            if (this._Boundary_Changed_NeedCheck)
                CheckBoundary();
            OpenGLWindow.Refresh();
            Things2Show.NoticeChange();
        }



        //行為
        /// <summary>
        /// 內容物邊框建隙與畫面的比例
        /// </summary>
        public Double BoundaryGap { get; set; }
        public Dimension GridDimension { get; set; }
        public Double Range
        {
            get { return fRange; }
            set
            {
                if (this.fRange != value)
                {
                    this.fRange = value;
                    this.openGLWindow.Range = this.fRange;
                }
            }
        }
        /// <summary>
        /// 當DXF2Show參考改變時是否自動刷新畫面
        /// </summary>
        public Boolean AutoRefresh { get; set; }
        /// <summary>
        /// X 方向所放比例
        /// </summary>
        public double XScale
        {
            get { return xScale; }
            set
            {
                if (this.xScale != value)
                {
                    this.xScale = value;
                    if (AutoRefresh) { OpenGLWindow.Refresh(); }
                }
            }
        }
        /// <summary>
        /// Y 方向所放比例
        /// </summary>
        public double YScale
        {
            get { return yScale; }
            set
            {
                if (this.yScale != value)
                {
                    this.yScale = value;
                    if (AutoRefresh) { OpenGLWindow.Refresh(); }
                }
            }
        }
        /// <summary>
        /// Z 方向所放比例
        /// </summary>
        public double ZScale
        {
            get { return zScale; }
            set
            {
                if (this.zScale != value)
                {
                    this.zScale = value;
                    if (AutoRefresh) { OpenGLWindow.Refresh(); }
                }
            }
        }
        /// <summary>
        /// 是否自動縮放內容物以符合視窗大小
        /// </summary>
        public Boolean AutoScale { get; set; }
        /// <summary>
        /// 畫面刷新的最短時間間隔，以毫秒為單位，預設為100毫秒
        /// </summary>
        public uint LeastRefreshTimeStep { get; set; }





        //邊框、格線、刻度屬性
        /// <summary>
        /// 是否顯示格線
        /// </summary>
        public Boolean PlotGrid { get; set; }
        /// <summary>
        /// 是否顯示刻度
        /// </summary>
        public Boolean PlotGraduation { get; set; }
        public Double GraduationHeight { get; set; }
        public Boolean GraduationOutside { get; set; }
        




        //顏色屬性
        public bool LightOn { get; set; }
        public Color BackGroundColor { get; set; }
        public Color GridColor1 { get; set; }
        public Color GridColor2 { get; set; }
        public Color GraduationColor { get; set; }




        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="openGLWindow">指定欲連結的OpenGL視窗，將自動加入Paint方法至OpenGL視窗的paint事件]</param>
        public OpenGLView(OpenGLWindow openGLWindow)
        {
            this.OpenGLWindow = openGLWindow;
            this.Things2Show.V = new HashSet<ICanPlotInOpenGL>();


            this.geometryBoundary = new XYZ4[2];
            this.viewBoundary = new XYZ4[2];
            this.BackGroundColor = Color.FromArgb(50, 50, 50);

            this.BoundaryGap = 0.2;

            this.LightOn = true;
            this.PlotGrid = true;
            this.GridColor1 = Color.FromArgb(160, 160, 160);
            this.GridColor2 = Color.FromArgb(70, 70, 70);

            this.PlotGraduation = true;
            this.GraduationHeight = 20;
            this.GraduationOutside = true;
            this.GraduationColor = Color.WhiteSmoke;

            this.GridDimension = Dimension.TwoDimension;

            this.graphicRangeX = 100;
            this.graphicRangeY = 100;
            this.Range = 100;

            this.XScale = 1.0;
            this.YScale = 1.0;
            this.ZScale = 1.0;
            this.AutoScale = false;
            this.AutoRefresh = true;

            this.openGLWindow.Paint += Paint;
            this._UpdateTimer = new System.Timers.Timer();
            this._UpdateTimer.Interval = 1000;
            this._UpdateTimer.Elapsed += (o, e) => 
            {
                Update();
                Console.WriteLine("upfate");
                this._UpdateTimer.Stop();
            };
        }

        public virtual void Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //背景
            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);// Clear Screen And Depth Buffer
            GL.glClearColor(Convert.ToSingle(this.BackGroundColor.R / 255.0),//R G B alpha
                            Convert.ToSingle(this.BackGroundColor.G / 255.0),
                            Convert.ToSingle(this.BackGroundColor.B / 255.0),
                            Convert.ToSingle(this.BackGroundColor.A / 255.0));
            GL.glFlush();
            
            //打光與材質
            if (LightOn)
                PlotSub.Light();
            GL.glDisable(GL.GL_CULL_FACE);
            
            #region 設定畫布與視野大小
            int width = openGLWindow.Width;
            int height = openGLWindow.Height;
            double range = this.Range;
            GL.glViewport(0, 0, width, height);//設定畫布大小
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();
            double h = height;
            double w = width;
            if (width == 0)// prevent a divide by zero
                width = 1;
            GL.glOrtho(-this.Range, this.Range, -this.Range * h / w, this.Range * h / w, -this.Range * 1000, this.Range * 1000);//設定視野大小
            #endregion 設定畫布與視野

            //滑鼠控制
            openGLWindow.MouseMoveInOpenGLPaint();
            //x,y,z軸比例縮放
            GL.glScaled(XScale, YScale, ZScale);

            //Default Tanslation
            Translated(centerPoint * -1);//平移原點DXF中心
            //開啟平滑模式
            //GL.glEnable(GL.GL_NORMALIZE);
            GL.glEnable(GL.GL_POINT_SMOOTH);
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glEnable(GL.GL_SMOOTH);
            GL.glEnable(GL.GL_LINE_SMOOTH);
            //GL.glFlush();
            

            //GL.glDisable(GL.GL_LINE_SMOOTH);
            //畫DXFD
            if (Things2Show.V.Count != 0)
                foreach (var item in Things2Show.V)
                    item.PlotInOpenGL();

            ////計算網格
            if (this.PlotGrid == true || this.PlotGraduation == true)
            {
                GanerateGridLayer(GridColor1, GridColor2, GraduationColor);
                if (this.PlotGrid)
                    gridLayer.PlotInOpenGL();
                if (this.PlotGraduation)
                    graduationLayer.PlotInOpenGL();
            }
            
        }

        protected virtual void GanerateGridLayer(Color gridColor1, Color gridColor2, Color graduationColor)
        {
            //清空格線
            this.gridLayer = new Layer("Grid");
            this.graduationLayer = new Layer("Graduation");

            //計算格線範圍
            CalculateViewBoundary();

            //計算格線密度
            CalculateGridPitch();

            #region 計算格線
            //設定範圍
            Double X1, X2, Y1, Y2;
            PointD[] gridBoundary = new PointD[2];
            if (this.GraduationOutside == false)
            {
                gridBoundary[0] = viewBoundary[0];
                gridBoundary[1] = viewBoundary[1];
            }
            else
            {
                double xSpace = (viewBoundary[1].X - viewBoundary[0].X) / this.openGLWindow.Width * GraduationHeight;
                double ySpace = (viewBoundary[1].Y - viewBoundary[0].Y) / this.openGLWindow.Height * GraduationHeight;
                gridBoundary[0] = viewBoundary[0] + new XYZ4(2.0 * xSpace, ySpace, 0);
                gridBoundary[1] = viewBoundary[1] - new XYZ4(xSpace, ySpace, 0);
            }
            //移動格線位置至內容的最後方，避免格線擋住顯示內容
            if (gridBoundary != null)
            {
                if (geometryBoundary[0] != null)
                {
                    gridBoundary[0].Z = geometryBoundary[0].Z - geometrySize.Z * 0.05;
                    gridBoundary[1].Z = geometryBoundary[0].Z - geometrySize.Z * 0.05; 
                }
            }
            

            X1 = System.Math.Ceiling(gridBoundary[0].X / xGridPitch) * xGridPitch;
            X2 = System.Math.Floor(gridBoundary[1].X / xGridPitch) * xGridPitch;
            Y1 = System.Math.Ceiling(gridBoundary[0].Y / yGridPitch) * yGridPitch;
            Y2 = System.Math.Floor(gridBoundary[1].Y / yGridPitch) * yGridPitch;

            String decFormat = "G4";

            //文字、格線
            Line tLine;
            for (double x = X1; x <= X2; x += xGridPitch)
            {
                tLine = new Line(new PointD(x, gridBoundary[0].Y, gridBoundary[0].Z),
                             new PointD(x, gridBoundary[1].Y, gridBoundary[0].Z));
                //粗格線
                if (Abs(x % (xGridPitch * 5.0)) < (xGridPitch * 0.5) || Abs(x % (xGridPitch * 5.0)) > (xGridPitch * 4.5))
                {
                    tLine.Color = gridColor1;
                    tLine.LineType = LineType.Solid;
                    tLine.LineWidth = 0.5f;
                    //座標文字
                    if (this.GraduationOutside == false)
                        graduationLayer.AddEntity(new Text(x.ToString(decFormat),
                                                    new PointD(x + (gridBoundary[1].X - gridBoundary[0].X) / this.OpenGLWindow.Width * 5.0,
                                                               gridBoundary[0].Y + (gridBoundary[1].Y - gridBoundary[0].Y) / this.OpenGLWindow.Height * 5.0,
                                                               gridBoundary[0].Z),
                                                    graduationColor));
                    else
                        graduationLayer.AddEntity(new Text(x.ToString(decFormat),
                                                    new PointD(x,
                                                               viewBoundary[0].Y + (viewBoundary[1].Y - viewBoundary[0].Y) / this.OpenGLWindow.Height * 5.0,
                                                               gridBoundary[0].Z),
                                                    graduationColor));
                }
                //細格線
                else
                {
                    tLine.Color = gridColor2;
                    tLine.LineType = LineType.Solid;
                    tLine.LineWidth = 0.3f;
                }
                //加入格線
                gridLayer.AddEntity(tLine);
            }
            for (double y = Y1; y <= Y2; y += yGridPitch)
            {
                //格線
                tLine = new Line(new PointD(gridBoundary[0].X, y, gridBoundary[0].Z),
                                 new PointD(gridBoundary[1].X, y, gridBoundary[0].Z));
                //粗格線
                if (Abs(y % (yGridPitch * 5.0)) < (yGridPitch * 0.5) || Abs(y % (yGridPitch * 5.0)) > (yGridPitch * 4.5))
                {
                    tLine.Color = gridColor1;
                    tLine.LineType = LineType.Solid;
                    tLine.LineWidth = 0.5f;
                    //座標文字
                    if (this.GraduationOutside == false)
                        graduationLayer.AddEntity(new Text(y.ToString(decFormat),
                                           new PointD(gridBoundary[0].X + (gridBoundary[1].X - gridBoundary[0].X) / this.OpenGLWindow.Width * 5.0,
                                                      y + (gridBoundary[1].Y - gridBoundary[0].Y) / this.OpenGLWindow.Height * 5.0,
                                                      gridBoundary[0].Z),
                                           graduationColor));
                    else
                        graduationLayer.AddEntity(new Text(y.ToString(decFormat),
                                           new PointD(viewBoundary[0].X + (viewBoundary[1].X - viewBoundary[0].X) / this.OpenGLWindow.Width * 5.0,
                                                      y,
                                                      gridBoundary[0].Z),
                                           graduationColor));
                }
                //細格線
                else
                {
                    tLine.Color = gridColor2;
                    tLine.LineType = LineType.Solid;
                    tLine.LineWidth = 0.3f;
                }
                //加入格線
                gridLayer.AddEntity(tLine);
            }

            //邊框
            //下
            tLine = new Line(new PointD(gridBoundary[0].X,
                                        gridBoundary[0].Y,
                                        gridBoundary[0].Z),
                             new PointD(gridBoundary[1].X,
                                        gridBoundary[0].Y,
                                        gridBoundary[1].Z));
            tLine.Color = gridColor1;
            tLine.LineType = LineType.Solid;
            tLine.LineWidth = 0.5f;
            gridLayer.AddEntity(tLine);
            //右
            tLine = new Line(new PointD(gridBoundary[1].X,
                                        gridBoundary[0].Y,
                                        gridBoundary[1].Z),
                             new PointD(gridBoundary[1].X,
                                        gridBoundary[1].Y,
                                        gridBoundary[1].Z));
            tLine.Color = gridColor1;
            tLine.LineType = LineType.Solid;
            tLine.LineWidth = 0.5f;
            gridLayer.AddEntity(tLine);
            //上
            tLine = new Line(new PointD(gridBoundary[1].X,
                                        gridBoundary[1].Y,
                                        gridBoundary[1].Z),
                             new PointD(gridBoundary[0].X,
                                        gridBoundary[1].Y,
                                        gridBoundary[0].Z));
            tLine.Color = gridColor1;
            tLine.LineType = LineType.Solid;
            tLine.LineWidth = 0.5f;
            gridLayer.AddEntity(tLine);
            //左
            tLine = new Line(new PointD(gridBoundary[0].X,
                                        gridBoundary[1].Y,
                                        gridBoundary[0].Z),
                             new PointD(gridBoundary[0].X,
                                        gridBoundary[0].Y,
                                        gridBoundary[0].Z));
            tLine.Color = gridColor1;
            tLine.LineType = LineType.Solid;
            tLine.LineWidth = 0.5f;
            gridLayer.AddEntity(tLine);
            #endregion 計算格線
        }

        protected virtual void CalculateViewBoundary()
        {
            #region 計算視野範圍
            viewBoundary[0] = centerPoint
                - new XYZ4((Double)openGLWindow.M_Translation.X, (Double)openGLWindow.M_Translation.Y, 0) / (Double)openGLWindow.M_Scale
                - new XYZ4(Range,
                            Range * ((Double)openGLWindow.Height / (Double)openGLWindow.Width),
                            0) / (Double)openGLWindow.M_Scale;
            viewBoundary[1] = centerPoint
                - new XYZ4((Double)openGLWindow.M_Translation.X, (Double)openGLWindow.M_Translation.Y, 0) / (Double)openGLWindow.M_Scale
                + new XYZ4(Range,
                            Range * ((Double)openGLWindow.Height / (Double)openGLWindow.Width),
                            0) / (Double)openGLWindow.M_Scale;

            gridSize = viewBoundary[1] - viewBoundary[0];

            //依AspectRatio縮放
            viewBoundary[0].X = this.centerPoint.X + (viewBoundary[0].X - this.centerPoint.X) / this.XScale;
            viewBoundary[1].X = this.centerPoint.X + (viewBoundary[1].X - this.centerPoint.X) / this.XScale;
            viewBoundary[0].Y = this.centerPoint.Y + (viewBoundary[0].Y - this.centerPoint.Y) / this.YScale;
            viewBoundary[1].Y = this.centerPoint.Y + (viewBoundary[1].Y - this.centerPoint.Y) / this.YScale;
            
            #endregion 計算格線範圍
        }

        protected virtual void CalculateGridPitch()
        {
            //計算格線密度 1 2 5
            #region X
            int i = 0;
            int j = 0;
            bool findPicth = false;
            while (!double.IsNaN(viewBoundary[0].X)
                && !double.IsNegativeInfinity(viewBoundary[0].X)
                && !double.IsPositiveInfinity(viewBoundary[0].X)
                && !double.IsNaN(viewBoundary[1].X)
                && !double.IsNegativeInfinity(viewBoundary[1].X)
                && !double.IsPositiveInfinity(viewBoundary[1].X))
            {
                for (j = 0; j < gridPitchOption.Length; j++)
                    if (openGLWindow.Width / ((viewBoundary[1].X - viewBoundary[0].X) / (gridPitchOption[j] * Pow(10, i))) >= minGridPitch)
                    {
                        findPicth = true;
                        break;
                    }
                if (findPicth)
                    break;
                i++;
            }
            if (i == 0 && j == 0)
            {
                findPicth = false;
                i--;
                while (true)
                {
                    for (j = gridPitchOption.Length - 1; j >= 0; j--)
                        if (openGLWindow.Width / ((viewBoundary[1].X - viewBoundary[0].X) / (gridPitchOption[j] * Pow(10, i))) < minGridPitch || (i == -4 && j == 0))
                        {
                            findPicth = true;
                            break;
                        }
                    if (findPicth)
                        break;
                    i--;
                }

                if (j < gridPitchOption.Length - 1)
                    j += 1;
                else
                {
                    i += 1;
                    j = 0;
                }
            }

            xGridPitch = gridPitchOption[j] * Pow(10, i);
            #endregion

            #region Y
            i = 0;
            j = 0;
            findPicth = false;
            while (
                   !double.IsNaN(viewBoundary[0].Y)
                && !double.IsNegativeInfinity(viewBoundary[0].Y)
                && !double.IsPositiveInfinity(viewBoundary[0].Y)
                && !double.IsNaN(viewBoundary[1].Y)
                && !double.IsNegativeInfinity(viewBoundary[1].Y)
                && !double.IsPositiveInfinity(viewBoundary[1].Y))
            {
                for (j = 0; j < gridPitchOption.Length; j++)
                    if (openGLWindow.Height / ((viewBoundary[1].Y - viewBoundary[0].Y) / (gridPitchOption[j] * Pow(10, i))) >= minGridPitch)
                    {
                        findPicth = true;
                        break;
                    }
                if (findPicth)
                    break;
                i++;
            }
            if (i == 0 && j == 0)
            {
                findPicth = false;
                i--;
                while (true)
                {
                    for (j = gridPitchOption.Length - 1; j >= 0; j--)
                        if (openGLWindow.Height / ((viewBoundary[1].Y - viewBoundary[0].Y) / (gridPitchOption[j] * Pow(10, i))) < minGridPitch || (i == -4 && j == 0))
                        {
                            findPicth = true;
                            break;
                        }
                    if (findPicth)
                        break;
                    i--;
                }

                if (j < gridPitchOption.Length - 1)
                    j += 1;
                else
                {
                    i += 1;
                    j = 0;
                }
            }

            yGridPitch = gridPitchOption[j] * Pow(10, i);
            #endregion
        }
    }
}
