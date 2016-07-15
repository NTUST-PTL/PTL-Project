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
using System.ComponentModel;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Windows.Controls
{
    /// <summary>
    /// TopoErrorDiagramControl.xaml 的互動邏輯
    /// </summary>
    public partial class TopoErrorDiagramControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ToothErrorDatas
        {
            public double[][,] TopoErrors { get; set; }
            public double[] CenterDiviations { get; set; }
        }
        public ToothErrorDatas ErrorData
        {
            get { return (ToothErrorDatas)GetValue(ErrorDataProperty); }
            set { SetValue(ErrorDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorDataProperty =
            DependencyProperty.Register("ErrorData", typeof(ToothErrorDatas), typeof(TopoErrorDiagramControl), new PropertyMetadata(Update));



        private double _FontSize1  =7;
        public double FontSize1
        {
            get { return _FontSize1; }
            set
            {
                if (value != _FontSize1)
                {
                    _FontSize1 = value;
                    NotifyPropertyChanged(nameof(FontSize1));
                }
            }
        }
        private double _FontSize2 = 8.5;
        public double FontSize2
        {
            get { return _FontSize2; }
            set
            {
                if (value != _FontSize2)
                {
                    _FontSize2 = value;
                    NotifyPropertyChanged(nameof(FontSize2));
                }
            }
        }
        private double _FontSize3 = 10;
        public double FontSize3
        {
            get { return _FontSize3; }
            set
            {
                if (value != _FontSize3)
                {
                    _FontSize3 = value;
                    NotifyPropertyChanged(nameof(FontSize3));
                }
            }
        }
        private double _ImageWidth_CM = 10;
        public double ImageWidth_CM
        {
            get { return _ImageWidth_CM; }
            set
            {
                if (value != _ImageWidth_CM)
                {
                    _ImageWidth_CM = value;
                    NotifyPropertyChanged(nameof(ImageWidth_CM));
                }
            }
        }
        private double _GridLineWidth = 0.5;
        public double GridLineWidth
        {
            get { return _GridLineWidth; }
            set
            {
                if (value != _GridLineWidth)
                {
                    _GridLineWidth = value;
                    NotifyPropertyChanged(nameof(GridLineWidth));
                }
            }
        }
        private double _GridLineWidth2 = 1;
        public double GridLineWidth2
        {
            get { return _GridLineWidth2; }
            set
            {
                if (value != _GridLineWidth2)
                {
                    _GridLineWidth2 = value;
                    NotifyPropertyChanged(nameof(GridLineWidth2));
                }
            }
        }

        private double unitCons = 0.25;
        private double _Ratio = 1;
        public double Ratio
        {
            get { return _Ratio; }
            set
            {
                if (value != _Ratio)
                {
                    _Ratio = value;
                    NotifyPropertyChanged(nameof(Ratio));
                }
            }
        }

        private Brush baseGridBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
        private Brush measuredGridBrush = Brushes.Blue;
        private Brush nagtiveErrorBrush = Brushes.Red;
        private Brush positiveErrorBrush = new SolidColorBrush(Color.FromArgb(255, 0, 200, 0));

        public TopoErrorDiagramControl()
        {
            InitializeComponent();
        }

        public static void Update(object o, DependencyPropertyChangedEventArgs e)
        {
            var ted = o as TopoErrorDiagramControl;
            if (ted != null
                && e.Property == ErrorDataProperty
                && ted.ErrorData != null
                && ted.ErrorData.TopoErrors != null
                && ted.ErrorData.CenterDiviations != null)
            {
                Func<double, double> s = (percent) => ted.outGrid.ActualWidth * (percent / 100.0);
                Func<double[], double[]> ss = (input) => {
                    double[] re = new double[input.Length];
                    for (int i = 0; i < input.Length; i++)
                        re[i] = ted.outGrid.ActualWidth * (input[i] / 100.0);
                    return re;
                };

                var TopoErrors = ted.ErrorData.TopoErrors;
                var CenterDiviations = ted.ErrorData.CenterDiviations;

                int nRow = TopoErrors[0].GetLength(0);
                int nCol = TopoErrors[0].GetLength(1);



                #region Clear StackPanel
                ted.mStack.Children.Clear();
                #endregion


                #region Grid Points

                double cTextH = ted.FontSize3 * 2.5;//中央橫向文字區域高度
                double gh1 = s(17);//單齒面高度
                double gw1 = gh1 * Cos(DegToRad(45));//單齒面側傾寬度
                double cH = gh1 + cTextH / 2 + ted.FontSize1 * 2.5;//網格中央水平位置
                double lspw = s(10);//網格左端空白區域大小
                double gw = s(100) - lspw * 2.5 - gw1;//網格寬度
                
                XYZ4 p11 = new XYZ4(lspw, cH - cTextH / 2, 0);
                XYZ4 p12 = new XYZ4(lspw + gw1, cH - cTextH / 2 - gh1, 0);
                XYZ4 p21 = new XYZ4(lspw, cH + cTextH / 2, 0);
                XYZ4 p22 = new XYZ4(lspw + gw1, cH + cTextH / 2 + gh1, 0);

                XYZ4[][,] GridPoints = new XYZ4[2][,] { new XYZ4[nRow, nCol], new XYZ4[nRow, nCol] };
                XYZ4[,] GridPointBase = { { p11, p12 }, { p21, p22 } };
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow; j++)
                    {
                        for (int k = 0; k < nCol; k++)
                        {
                            GridPoints[i][j, k] = GridPointBase[i, 0] + (GridPointBase[i, 1] - GridPointBase[i, 0]) / (nRow - 1) * j
                                + new XYZ4(gw / (nCol - 1) * k, 0, 0);
                        }
                    }
                }

                #endregion Grid Points


                Grid mGrid = new Grid();
                //mGrid.Background = Brushes.LightSkyBlue;
                mGrid.Name = "TopoErrorDiagramGrid";


                #region Base Grid Lines

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow; j++)
                    {
                        Line line = new Line();
                        line.Stroke = ted.baseGridBrush;
                        line.StrokeThickness = ted.GridLineWidth;
                        line.X1 = GridPoints[i][j, 0].X;
                        line.Y1 = GridPoints[i][j, 0].Y;
                        line.X2 = GridPoints[i][j, nCol - 1].X;
                        line.Y2 = GridPoints[i][j, nCol - 1].Y;
                        mGrid.Children.Add(line);
                    }
                    for (int k = 0; k < nCol; k++)
                    {
                        Line line = new Line();
                        line.Stroke = ted.baseGridBrush;
                        line.StrokeThickness = ted.GridLineWidth;
                        line.X1 = GridPoints[i][0, k].X;
                        line.Y1 = GridPoints[i][0, k].Y;
                        line.X2 = GridPoints[i][nRow - 1, k].X;
                        line.Y2 = GridPoints[i][nRow - 1, k].Y;
                        mGrid.Children.Add(line);
                    }
                }

                for (int i = 0; i < nCol; i = i + nCol - 1)
                {
                    Line line = new Line();
                    line.Stroke = ted.baseGridBrush;
                    line.StrokeThickness = ted.GridLineWidth;
                    line.X1 = GridPoints[0][0, i].X;
                    line.Y1 = GridPoints[0][0, i].Y;
                    line.X2 = GridPoints[1][0, i].X;
                    line.Y2 = GridPoints[1][0, i].Y;
                    mGrid.Children.Add(line);
                }
                

                #endregion Base Grid Lines



                #region Measured Points
                double unit = s(ted.unitCons) * ted.Ratio;
                XYZ3[] vec = new XYZ3[] {
                    new XYZ3(Cos(DegToRad(-135)), Sin(DegToRad(-135)), 0)
                    , new XYZ3(Cos(DegToRad(135)), Sin(DegToRad(135)), 0)
                };
                XYZ4[][,] MeasuredPoints = new XYZ4[2][,] { new XYZ4[nRow, nCol], new XYZ4[nRow, nCol] };
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow; j++)
                    {
                        for (int k = 0; k < nCol; k++)
                        {
                            MeasuredPoints[i][j, k] = GridPoints[i][j, k] + vec[i] * TopoErrors[i][j, k] * 1000 * unit;
                        }
                    }
                }
                #endregion Measured Points



                #region Measured Grid Lines
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow - 1; j++)
                    {
                        for (int k = 0; k < nCol - 1; k++)
                        {
                            Line line1 = new Line();
                            Line line2 = new Line();
                            line1.Stroke = line2.Stroke = ted.measuredGridBrush;
                            line1.StrokeThickness = line2.StrokeThickness = ted._GridLineWidth2;
                            XYZ4 p0 = MeasuredPoints[i][j, k];
                            XYZ4 p1 = MeasuredPoints[i][j + 1, k];
                            XYZ4 p2 = MeasuredPoints[i][j, k + 1];
                            line1.X1 = p0.X;
                            line1.Y1 = p0.Y;
                            line1.X2 = p1.X;
                            line1.Y2 = p1.Y;
                            line2.X1 = p0.X;
                            line2.Y1 = p0.Y;
                            line2.X2 = p2.X;
                            line2.Y2 = p2.Y;
                            mGrid.Children.Add(line1);
                            mGrid.Children.Add(line2);
                        }
                    }

                    for (int j = 0; j < nRow - 1; j++)
                    {
                        Line line = new Line();
                        line.Stroke = ted.measuredGridBrush;
                        line.StrokeThickness = ted.GridLineWidth2;
                        XYZ4 p0 = MeasuredPoints[i][j, nCol - 1];
                        XYZ4 p1 = MeasuredPoints[i][j + 1, nCol - 1];
                        line.X1 = p0.X;
                        line.Y1 = p0.Y;
                        line.X2 = p1.X;
                        line.Y2 = p1.Y;
                        mGrid.Children.Add(line);
                    }
                    for (int k = 0; k < nCol - 1; k++)
                    {
                        Line line = new Line();
                        line.Stroke = ted.measuredGridBrush;
                        line.StrokeThickness = ted.GridLineWidth2;
                        XYZ4 p0 = MeasuredPoints[i][nRow - 1, k];
                        XYZ4 p1 = MeasuredPoints[i][nRow - 1, k + 1];
                        line.X1 = p0.X;
                        line.Y1 = p0.Y;
                        line.X2 = p1.X;
                        line.Y2 = p1.Y;
                        mGrid.Children.Add(line);
                    }
                }
                #endregion Measured Grid Lines



                #region Error Lines
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow; j++)
                    {
                        for (int k = 0; k < nCol; k++)
                        {
                            Line line = new Line();
                            line.StrokeThickness = ted._GridLineWidth2;
                            if (TopoErrors[i][j, k] < 0)
                                line.Stroke = ted.nagtiveErrorBrush;
                            else
                                line.Stroke = ted.positiveErrorBrush;
                            XYZ4 p1 = GridPoints[i][j, k];
                            XYZ4 p2 = MeasuredPoints[i][j, k];
                            line.X1 = p1.X;
                            line.Y1 = p1.Y;
                            line.X2 = p2.X;
                            line.Y2 = p2.Y;
                            mGrid.Children.Add(line);
                        }
                    }
                }
                #endregion Measured Grid Lines



                #region Big Text
                {
                    string[] strs = { "Front", "Root", "Back" };
                    TextBlock[] tbs = new TextBlock[strs.Length];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        tbs[i] = new TextBlock() { Text = strs[i], FontSize = ted.FontSize3, FontWeight = FontWeights.Bold };
                    }
                    tbs[2].HorizontalAlignment = HorizontalAlignment.Right;

                    tbs[0].Margin = new Thickness(lspw + ted.FontSize1 * 0.5, cH - ted.FontSize1, 0, 0);
                    tbs[1].Margin = new Thickness(lspw + gw / 2 - ted.FontSize1 * 1, cH - ted.FontSize1, 0, 0);
                    tbs[2].Margin = new Thickness(0, cH - ted.FontSize1, s(100) - lspw - gw + ted.FontSize1 * 0.5, 0);
                    //tbs[3].Margin = new Thickness(s(50), cH - cTextH / 2 - gh1 - ted.FontSize1 * 4.5, 0, 0);
                    //tbs[4].Margin = new Thickness(s(50), cH + cTextH / 2 + gh1 + ted.FontSize1 * 2.8, 0, 0);
                    foreach (var item in tbs)
                    {
                        mGrid.Children.Add(item);
                    }
                }
                #endregion



                #region Row Number Text
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < nRow; j++)
                        {
                            TextBlock Num = new TextBlock();
                            Num.Text = (j + 1).ToString();
                            Num.FontSize = ted.FontSize1;

                            double left = GridPoints[i][j, nCol - 1].X + ted.FontSize1 * 1.5;
                            double top = GridPoints[i][j, nCol - 1].Y - ted.FontSize1 * 0.5;
                            Num.Margin = new Thickness(left, top, 0, 0);
                            mGrid.Children.Add(Num);
                        }
                    }
                }
                #endregion Row Number Text



                #region Column Number Text
                {
                    double[] verOffsetDir = { -1, 1 };
                    for (int i = 0; i < 2; i++)
                    {
                        for (int k = 0; k < nCol; k++)
                        {
                            TextBlock Num = new TextBlock();
                            Num.Text = ((char)(k + 65)).ToString();
                            Num.FontSize = ted.FontSize1;


                            double left = GridPoints[i][nRow - 1, k].X + ted.FontSize1 * 0;
                            double top = 0;
                            if (i == 0)
                                top = GridPoints[i][nRow - 1, k].Y + verOffsetDir[i] * ted.FontSize1 * 2.5;
                            else
                                top = GridPoints[i][nRow - 1, k].Y + verOffsetDir[i] * ted.FontSize1 * 1.5;
                            Num.Margin = new Thickness(left, top, 0, 0);
                            mGrid.Children.Add(Num);
                        }
                    }
                }
                #endregion Row Number Text



                #region Corner Value Text
                {
                    TextBlock[] tbs = new TextBlock[8];
                    double[] errs = {
                    TopoErrors[0][0, 0],
                    TopoErrors[1][0, 0],
                    TopoErrors[0][nRow - 1, 0],
                    TopoErrors[1][nRow - 1, 0],
                    TopoErrors[0][0, nCol - 1],
                    TopoErrors[1][0,  nCol - 1],
                    TopoErrors[0][nRow - 1, nCol - 1],
                    TopoErrors[1][nRow - 1,  nCol - 1] };
                    XYZ4[] ps = {
                    GridPoints[0][0, 0],
                    GridPoints[1][0, 0],
                    GridPoints[0][nRow - 1, 0],
                    GridPoints[1][nRow - 1, 0],
                    GridPoints[0][0,  nCol - 1],
                    GridPoints[1][0,  nCol - 1],
                    GridPoints[0][nRow - 1,  nCol - 1],
                    GridPoints[1][nRow - 1, nCol - 1] };
                    for (int i = 0; i < 8; i++)
                    {
                        tbs[i] = new TextBlock();
                        tbs[i].Text = (errs[i] * 1000).ToString("0.0");
                        tbs[i].Foreground = errs[i] >= 0 ? ted.positiveErrorBrush : ted.nagtiveErrorBrush;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        tbs[i].HorizontalAlignment = HorizontalAlignment.Right;

                    }
                    for (int i = 4; i < 8; i++)
                    {
                        tbs[i].HorizontalAlignment = HorizontalAlignment.Left;
                    }

                    tbs[0].Margin = new Thickness(0, ps[0].Y - ted.FontSize1 * 0.8, s(100) - ps[0].X + ted.FontSize1 * 1, 0);
                    tbs[1].Margin = new Thickness(0, ps[1].Y - ted.FontSize1 * 0.8, s(100) - ps[1].X + ted.FontSize1 * 1, 0);
                    tbs[2].Margin = new Thickness(0, ps[2].Y - ted.FontSize1 * 0.8, s(100) - ps[2].X + ted.FontSize1 * 1.3, 0);
                    tbs[3].Margin = new Thickness(0, ps[3].Y - ted.FontSize1 * 0.5, s(100) - ps[3].X + ted.FontSize1 * 1.3, 0);
                    tbs[4].Margin = new Thickness(ps[4].X + ted.FontSize1 * 3, ps[4].Y - ted.FontSize1 * 0.8, 0, 0);
                    tbs[5].Margin = new Thickness(ps[5].X + ted.FontSize1 * 3, ps[5].Y - ted.FontSize1 * 0.8, 0, 0);
                    tbs[6].Margin = new Thickness(ps[6].X + ted.FontSize1 * 2.5, ps[6].Y - ted.FontSize1 * 0.8, 0, 0);
                    tbs[7].Margin = new Thickness(ps[7].X + ted.FontSize1 * 2.5, ps[7].Y - ted.FontSize1 * 0.5, 0, 0);

                    foreach (var item in tbs)
                    {
                        if (item != null)
                        {
                            item.FontSize = ted.FontSize2;
                            mGrid.Children.Add(item);
                        }
                    }
                }
                #endregion Corner Value Text



                #region Other Texts
                TextBlock[] tips;
                {
                    string[] strs = { "Tip", "Tip" };
                    tips = new TextBlock[strs.Length];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        tips[i] = new TextBlock() { Text = strs[i], FontSize = ted.FontSize3, FontWeight = FontWeights.Bold };
                    }
                    tips[0].Margin = new Thickness(s(50), 0, 0, ted.FontSize3 * 0.1);
                    tips[1].Margin = new Thickness(s(50), 0, 0, 0);
                }



                TextBlock infos = new TextBlock() { FontSize = ted.FontSize2 };
                infos.TextAlignment = TextAlignment.Center;
                infos.TextWrapping = TextWrapping.Wrap;
                double AverageError = 0;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow; j++)
                    {
                        for (int k = 0; k < nCol; k++)
                        {
                            AverageError = Abs(TopoErrors[i][j, k]) * 1e3;
                        }
                    }
                }
                AverageError /= (2 * nRow * nCol);

                double SumOfSquare = 0;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow; j++)
                    {
                        for (int k = 0; k < nCol; k++)
                        {
                            SumOfSquare = TopoErrors[i][j, k] * TopoErrors[i][j, k] * 1e6;
                        }
                    }
                }

                double spaceError = (CenterDiviations[0] - CenterDiviations[1]) * 1e3;

                double Sum = 0;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < nRow; j++)
                    {
                        for (int k = 0; k < nCol; k++)
                        {
                            Sum = TopoErrors[i][j, k] * 1e3;
                        }
                    }
                }

                infos.Text = String.Format(
                    "Average Error: {0:0.#} µm   Sum of Square: {1:0.#} µm^2\r\n" +
                    "Space Error: {2:0.#} µm, Left Div: {3:0.#} µm, Right Div: {4:0.#} µm, Sum: {5:0.#} µm",
                    AverageError, SumOfSquare, spaceError, CenterDiviations[0], CenterDiviations[1], Sum);
                #endregion Other Texts

                ted.mStack.Children.Add(tips[0]);
                ted.mStack.Children.Add(mGrid);
                ted.mStack.Children.Add(tips[1]);
                ted.mStack.Children.Add(infos);
            }
        }

        public static void UpdatePitchErrors(object o, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
