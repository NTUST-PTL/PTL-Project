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
using System.Windows.Media.Media3D;
using PTL.Windows.VisualExtensions;
using PTL.Geometry;
using PTL.Geometry.WPFExtensions;
using PTL.Geometry.MathModel;
using PTL.FileOperation;
using PTL.Mathematics;
using PTL.Windows.Media.Media3D;

namespace PTL.Windows.Controls
{
    /// <summary>
    /// CAD_Like_ViewPort3D.xaml 的互動邏輯
    /// </summary>
    public partial class CAD_Like_ViewPort3D : UserControl
    {
        public Model3DGroup AllModels = new Model3DGroup() { Transform = new Transform3DGroup() };
        private Dictionary<GeometryModel3D, Material> OriginalColor = new Dictionary<GeometryModel3D, Material>();
        private Material selecetColor = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 50, 125, 200)));
        private Model3DGroup SelectedModels = new Model3DGroup();

        private bool mouseMovedAfterButtonDown;
        private Point mLastPos;
        private Point3D AllModelCenter = new Point3D(0, 0, 0);
        private Point3D rotateCenter = new Point3D(0, 0, 0);
        private double highLightScale = 1.01;
        private double reverseHighLightScale = 1 / 1.01;
        private double XScale = 1;
        private double YScale = 1;
        private double ZScale = 1;
        private double ScaleSensitivity = 1.1;
        

        public CAD_Like_ViewPort3D()
        {
            InitializeComponent();
            InitializeCamera();
            ResetAllTransform();
            AllModels.Changed += ModelGroup_Changed;
            SelectedModels.Changed += SelectedModels_Changed;
        }

        public void InitializeCamera()
        {
            camera.FarPlaneDistance = double.MaxValue;
            camera.NearPlaneDistance = 0;
            camera.LookDirection = new Vector3D(0, 0, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
        }

        public void BuildSolid()
        {
            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();
            // Front face
            mesh.Positions.Add(new Point3D(-1, -1, 1));
            mesh.Positions.Add(new Point3D(1, -1, 1));
            mesh.Positions.Add(new Point3D(1, 1, 1));
            mesh.Positions.Add(new Point3D(-1, 1, 1));
            // Back face
            mesh.Positions.Add(new Point3D(-1, -1, -1));
            mesh.Positions.Add(new Point3D(1, -1, -1));
            mesh.Positions.Add(new Point3D(1, 1, -1));
            mesh.Positions.Add(new Point3D(-1, 1, -1));

            // Front face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            // Back face
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Right face
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);

            // Top face
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            // Bottom face
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            // Right face
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);

            GeometryModel3D mGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.YellowGreen));
            mGeometry.Transform = new Transform3DGroup();
            AddInteractiveModel(mGeometry);
        }

        #region Change Item
        private void ModelGroup_Changed(object sender, EventArgs e)
        {
            AllModelCenter = getModelCenter(AllModels);
        }

        private void SelectedModels_Changed(object sender, EventArgs e)
        {
            if (SelectedModels.Children.Count != 0)
                rotateCenter = getModelCenter(SelectedModels);
            else
                rotateCenter = getModelCenter(AllModels);
        }

        public void AddInteractiveModel(params Model3D[] models)
        {
            Viewport.Children.Remove(MovingUIElementsVisual3D);
            foreach (var model in models)
            {
                ModelUIElement3D UIModel = new ModelUIElement3D();
                UIModel.Model = model;
                UIModel.MouseEnter += UI_MouseEnter;
                UIModel.MouseLeave += UI_MouseLeave;
                UIModel.MouseUp += UI_MouseUp;

                MovingUIElementsVisual3D.Children.Add(UIModel);
                AllModels.Children.Add(model);
            }
            Viewport.Children.Add(MovingUIElementsVisual3D);
        }

        public void AddInteractiveWireframeModel(params FakeLineGeometryModel3D[] models)
        {
            Viewport.Children.Remove(MovingUIElementsVisual3D);
            List<ModelUIElement3D> AddedUIModel = new List<ModelUIElement3D>();
            foreach (var model in models)
            {
                ModelUIElement3D UIModel = new ModelUIElement3D();
                UIModel.Model = model.Model;
                UIModel.MouseEnter += UI_MouseEnter;
                UIModel.MouseLeave += UI_MouseLeave;
                UIModel.MouseUp += UI_MouseUp;

                AddedUIModel.Add(UIModel);
                MovingUIElementsVisual3D.Children.Add(UIModel);
                WireframeModelUIElement3D.Add(UIModel);
                WireframeModel3D.Add(model);
                AllModels.Children.Add(model.Model);
            }
            Viewport.Children.Add(MovingUIElementsVisual3D);
            RefreshWireframe((wf) => AddedUIModel.Contains(wf));
        }

        public void Clear()
        {
            MovingUIElementsVisual3D.Children.Clear();
            AllModels.Children.Clear();
        }

        public void AddSelectedModel(params Model3D[] models)
        {
            foreach (var model in models)
            {
                if (!SelectedModels.Children.Contains(model))
                {
                    SelectedModels.Children.Add(model);
                    GeometryModel3D geometryModel3D = model as GeometryModel3D;
                    if (geometryModel3D != null)
                    {
                        OriginalColor.Add(geometryModel3D, geometryModel3D.Material);
                        geometryModel3D.Material = selecetColor;
                    }
                }
            };
        }

        public void ClearSelectedModels()
        {
            SelectedModels.Children.Clear();
            foreach (var item in OriginalColor)
                item.Key.Material = item.Value;
            OriginalColor.Clear();
        }
        #endregion Change Item

        #region Wireframe Refresh
        List<ModelUIElement3D> WireframeModelUIElement3D = new List<ModelUIElement3D>();
        List<FakeLineGeometryModel3D> WireframeModel3D = new List<FakeLineGeometryModel3D>();

        public void RefreshWireframe(Predicate<ModelUIElement3D> filter = null)
        {
            DateTime startTime = DateTime.Now;

            

            List<Tuple<ModelUIElement3D, Transform3D>> allModelUI3D = FindVisualChildrenTransform<ModelUIElement3D>(this.Viewport);

            List<Tuple<ModelUIElement3D, Transform3D, FakeLineGeometryModel3D>> needRefresh =
                (from item in allModelUI3D
                 where WireframeModelUIElement3D.Contains(item.Item1)
                 select new Tuple<ModelUIElement3D, Transform3D, FakeLineGeometryModel3D>(
                     item.Item1,
                     item.Item2,
                     WireframeModel3D[WireframeModelUIElement3D.IndexOf(item.Item1)])).ToList();


            //Matrix3D M = MovingUIElementsVisual3D.Transform.Value;
            //double[,] m = new double[,]
            //  { { M.M11, M.M21, M.M31, M.OffsetX },
            //        { M.M12, M.M22, M.M32, M.OffsetY },
            //        { M.M13, M.M23, M.M33, M.OffsetZ },
            //        { M.M14, M.M24, M.M34, M.M44 }};
            //double[,] mi = PTLM.MatrixInverse(m);
            //XYZ3 look = PTLM.Transport(mi, new XYZ3(0, 0, -1));
            //XYZ3 up = PTLM.Transport(mi, new XYZ3(0, 1, 0));

            this.Viewport.Children.Remove(MovingUIElementsVisual3D);

            foreach (var item in needRefresh)
            {
                if (filter == null || filter(item.Item1))
                {
                //Tranform relate to Viewport
                     Matrix3D M = item.Item2.Value;
                    double[,] m = new double[,]
                      { { M.M11, M.M21, M.M31, M.OffsetX },
                      { M.M12, M.M22, M.M32, M.OffsetY },
                      { M.M13, M.M23, M.M33, M.OffsetZ },
                      { M.M14, M.M24, M.M34, M.M44 }};
                    double[,] mi = PTLM.MatrixInverse(m);
                    XYZ3 look = PTLM.Transport(mi, new XYZ3(0, 0, -1));
                    XYZ3 up = PTLM.Transport(mi, new XYZ3(0, 1, 0));

                    item.Item3.ReshreshModelMesh(look, up, camera.Width, Convert.ToInt32(Viewport.ActualWidth));
                }
                //item.Item3.ReshreshModelMesh(look, up, camera.Width, Convert.ToInt32(Viewport.ActualWidth));
            }

            this.Viewport.Children.Add(MovingUIElementsVisual3D);
            DateTime endTime = DateTime.Now;
            TimeSpan dt = endTime - startTime;
            Console.WriteLine(dt.TotalMilliseconds);
        }

        public List<Tuple<T, Transform3D>> FindVisualChildrenTransform<T>(DependencyObject depObj, Transform3D depObj_Transform = null) where T : DependencyObject
        {
            List<Tuple<T, Transform3D>> Result = new List<Tuple<T, Transform3D>>();
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    Transform3DGroup CumulativeTransform = new Transform3DGroup();
                    if (depObj_Transform != null)
                        CumulativeTransform.Children.Add(depObj_Transform);

                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    Transform3D childTransform = null;
                    var t = child.GetType().GetProperty("Transform")?.GetValue(child);
                    if (t is Transform3D)
                        childTransform = (Transform3D)t;
                    if (childTransform != null)
                        CumulativeTransform.Children.Add(childTransform);

                    if (child != null && child is T)
                    {
                        Result.Add(new Tuple<T, Transform3D>((T)child, CumulativeTransform));
                    }

                    foreach (var childOfChild in FindVisualChildrenTransform<T>(child, CumulativeTransform))
                    {
                        Result.Add(childOfChild);
                    }
                }
            }
            return Result;
        }
        #endregion Wireframe Refresh

        #region About Transform
        public void TranslateViewTo(Model3D model)
        {
            Rect3D bound = model.Bounds;
            double x1 = bound.X;
            double x2 = bound.X + bound.SizeX;
            double y1 = bound.Y;
            double y2 = bound.Y + bound.SizeY;
            double z = bound.Z + bound.SizeZ;
            double sizeX = bound.SizeX;
            double sizeY = bound.SizeY;
            double sizeZ = bound.SizeZ;

            double viewport_Hight = Viewport.ActualHeight;
            double viewport_Width = Viewport.ActualWidth;

            double horizontalRequiredScale = (sizeX * 1.1);
            double verticalRequiredScale = (sizeY * 1.1) * (viewport_Width / viewport_Hight);

            camera.Width = horizontalRequiredScale > verticalRequiredScale ? horizontalRequiredScale : verticalRequiredScale;

            Point3D center = getModelCenter(model);
            Transform3D t = new TranslateTransform3D(new Point3D(0, 0, center.Z) - center);
            TransformAllModel(t);
            double scale = System.Math.Sqrt(sizeX * sizeX + sizeY * sizeY + sizeZ * sizeZ);
            camera.Position = new Point3D(0, 0, (model.Bounds.Z + model.Bounds.SizeZ) + scale);
            RefreshWireframe();
        }

        public void ResetViewTo(Model3D model)
        {
            ResetAllTransform();
            TranslateViewTo(model);
            
        }

        public Point3D getModelCenter(Model3D model)
        {
            Rect3D bound = model.Bounds;
            return new Point3D(
                bound.X + bound.SizeX / 2.0,
                bound.Y + bound.SizeY / 2.0,
                bound.Z + bound.SizeZ / 2.0
                );
        }

        public void ResetAllTransform()
        {
            MovingUIElementsVisual3D.Transform = new Transform3DGroup();
            MovingModelGroup.Transform = new Transform3DGroup();
            AllModels.Transform = new Transform3DGroup();
            SelectedModels.Transform = new Transform3DGroup();
            RefreshWireframe();
        }

        public void TransformAllModel(Transform3D transform)
        {
            if (MovingUIElementsVisual3D.Transform == null)
                MovingUIElementsVisual3D.Transform = new Transform3DGroup();
            if (AllModels.Transform == null)
                AllModels.Transform = new Transform3DGroup();
            if (SelectedModels.Transform == null)
                SelectedModels.Transform = new Transform3DGroup();

            Transform3DGroup uiGroupT = MovingUIElementsVisual3D.Transform as Transform3DGroup;
            Transform3DGroup allModelsT = AllModels.Transform as Transform3DGroup;
            Transform3DGroup selectedModelsT = SelectedModels.Transform as Transform3DGroup;
            uiGroupT.Children.Add(transform);
            allModelsT.Children.Add(transform);
            selectedModelsT.Children.Add(transform);
            RefreshWireframe();
        }

        private void Viewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshWireframe();
        }
        #endregion About Transform

        #region Mouse Event
        public void UI_MouseEnter(object sender, MouseEventArgs e)
        {
            //ModelUIElement3D model = (ModelUIElement3D)sender;
            //Transform3DGroup transformgroup = new Transform3DGroup();
            //if (model.Transform != null)
            //    transformgroup.Children.Add(model.Transform);
            //transformgroup.Children.Add(new ScaleTransform3D(
            //    new Vector3D(highLightScale, highLightScale, highLightScale),
            //    getModelCenter(model.Model)));
            //model.Transform = transformgroup;
        }

        public void UI_MouseLeave(object sender, MouseEventArgs e)
        {
            //ModelUIElement3D model = (ModelUIElement3D)sender;
            //Transform3DGroup transformgroup = new Transform3DGroup();
            //if (model.Transform != null)
            //    transformgroup.Children.Add(model.Transform);
            //transformgroup.Children.Add(new ScaleTransform3D(
            //    new Vector3D(reverseHighLightScale, reverseHighLightScale, reverseHighLightScale),
            //    getModelCenter(model.Model)));
            //model.Transform = transformgroup;
        }

        public void UI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mouseMovedAfterButtonDown && e.ChangedButton == MouseButton.Left)
            {
                ModelUIElement3D model = (ModelUIElement3D)sender;
                AddSelectedModel(model.Model);
                mouseMovedAfterButtonDown = false;
                e.Handled = true;
                return;
            }
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            ResetViewTo(AllModels);
        }

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            camera.Width *= System.Math.Pow(ScaleSensitivity, e.Delta / 120.0);
            RefreshWireframe();
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            bool functioned = true;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                double viewport_Hight = Viewport.ActualHeight;
                double viewport_Width = Viewport.ActualWidth;

                Point pos = Mouse.GetPosition(Viewport);
                Point actualPos = new Point(pos.X - viewport_Width / 2, viewport_Hight / 2 - pos.Y);
                double dx = actualPos.X - mLastPos.X, dy = actualPos.Y - mLastPos.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = System.Math.Asin(System.Math.Abs(dy) / System.Math.Sqrt(System.Math.Pow(dx, 2) + System.Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0) mouseAngle += System.Math.PI / 2;
                    else if (dx < 0 && dy < 0) mouseAngle += System.Math.PI;
                    else if (dx > 0 && dy < 0) mouseAngle += System.Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0) mouseAngle = System.Math.Sign(dy) > 0 ? System.Math.PI / 2 : System.Math.PI * 1.5;
                else if (dx != 0 && dy == 0) mouseAngle = System.Math.Sign(dx) > 0 ? 0 : System.Math.PI;

                double axisAngle = mouseAngle + System.Math.PI / 2;

                Vector3D axis = new Vector3D(System.Math.Cos(axisAngle) * 4, System.Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * System.Math.Sqrt(System.Math.Pow(dx, 2) + System.Math.Pow(dy, 2));

                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / System.Math.PI));
                Transform3D t = new RotateTransform3D(r, rotateCenter);

                TransformAllModel(t);
                mLastPos = actualPos;
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                double viewport_Hight = Viewport.ActualHeight;
                double viewport_Width = Viewport.ActualWidth;
                double moveRate = camera.Width / viewport_Width;

                Point pos = Mouse.GetPosition(Viewport);
                Point actualPos = new Point(pos.X - Viewport.ActualWidth / 2, Viewport.ActualHeight / 2 - pos.Y);
                double dx = (actualPos.X - mLastPos.X) * moveRate;
                double dy = (actualPos.Y - mLastPos.Y) * moveRate;

                Transform3D t = new TranslateTransform3D(dx, dy, 0);

                TransformAllModel(t);

                mLastPos = actualPos;
            }
            else
            {
                functioned = false;
            }
            if (functioned)
            {
                mouseMovedAfterButtonDown = true;
            }
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //List<DependencyObject> list = Viewport.FindVisualChildren<DependencyObject>().ToList();
                //List<Tuple<ModelUIElement3D, Transform3D>> listT = FindVisualChildrenTransform<ModelUIElement3D>(Viewport, new Transform3DGroup()).ToList();
                //Point3D p0 = listT[0].Item2.Transform(new Point3D(0, 0, 0));
                //Point3D p1 = listT[0].Item2.Transform(new Point3D(1, 0, 0));
                //Point3D p2 = listT[0].Item2.Transform(new Point3D(0, 1, 0));
                //Point3D p3 = listT[0].Item2.Transform(new Point3D(0, 0, 1));
                //Polyline lineX = new Polyline() { Points = new }
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    Point pos = Mouse.GetPosition(Viewport);
                    mLastPos = new Point(pos.X - Viewport.ActualWidth / 2, Viewport.ActualHeight / 2 - pos.Y);
                }
                else if (e.ClickCount == 2)
                {
                    if (SelectedModels.Children.Count > 0)
                    {
                        ResetViewTo(SelectedModels);
                    }
                    else
                    {
                        ResetViewTo(AllModels);
                    }
                }

            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    Point pos = Mouse.GetPosition(Viewport);
                    mLastPos = new Point(pos.X - Viewport.ActualWidth / 2, Viewport.ActualHeight / 2 - pos.Y);
                }
                else if (e.ClickCount == 2)
                {
                    if (SelectedModels.Children.Count > 0)
                    {
                        TranslateViewTo(SelectedModels);
                    }
                    else
                    {
                        TranslateViewTo(AllModels);
                    }
                }
            }
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            mouseMovedAfterButtonDown = false;

            if (e.ChangedButton == MouseButton.Left)
            {
                ClearSelectedModels();
            }
        }
        #endregion Mouse Event

        #region Grid
        //protected virtual void GanerateGridLayer(Color gridColor1, Color gridColor2, Color graduationColor)
        //{
        //    //清空格線
        //    this.Viewport.Children.Remove(Grid);
        //    Grid.Children.Clear();

        //    //計算格線範圍
        //    CalculateViewBoundary();

        //    //計算格線密度
        //    CalculateGridPitch();

        //    #region 計算格線
        //    //設定範圍
        //    Double X1, X2, Y1, Y2;
        //    PointD[] gridBoundary = new PointD[2];
        //    if (this.GraduationOutside == false)
        //    {
        //        gridBoundary[0] = viewBoundary[0];
        //        gridBoundary[1] = viewBoundary[1];
        //    }
        //    else
        //    {
        //        double xSpace = (viewBoundary[1].X - viewBoundary[0].X) / this.openGLWindow.Width * GraduationHeight;
        //        double ySpace = (viewBoundary[1].Y - viewBoundary[0].Y) / this.openGLWindow.Height * GraduationHeight;
        //        gridBoundary[0] = viewBoundary[0] + new XYZ4(2.0 * xSpace, ySpace, 0);
        //        gridBoundary[1] = viewBoundary[1] - new XYZ4(xSpace, ySpace, 0);
        //    }
        //    //移動格線位置至內容的最後方，避免格線擋住顯示內容
        //    if (gridBoundary != null)
        //    {
        //        if (geometryBoundary[0] != null)
        //        {
        //            gridBoundary[0].Z = geometryBoundary[0].Z - geometrySize.Z * 0.05;
        //            gridBoundary[1].Z = geometryBoundary[0].Z - geometrySize.Z * 0.05;
        //        }
        //    }


        //    X1 = System.Math.Ceiling(gridBoundary[0].X / xGridPitch) * xGridPitch;
        //    X2 = System.Math.Floor(gridBoundary[1].X / xGridPitch) * xGridPitch;
        //    Y1 = System.Math.Ceiling(gridBoundary[0].Y / yGridPitch) * yGridPitch;
        //    Y2 = System.Math.Floor(gridBoundary[1].Y / yGridPitch) * yGridPitch;

        //    String decFormat = "G4";

        //    //文字、格線
        //    Line tLine;
        //    for (double x = X1; x <= X2; x += xGridPitch)
        //    {
        //        tLine = new Line(new PointD(x, gridBoundary[0].Y, gridBoundary[0].Z),
        //                     new PointD(x, gridBoundary[1].Y, gridBoundary[0].Z));
        //        //粗格線
        //        if (Abs(x % (xGridPitch * 5.0)) < (xGridPitch * 0.5) || Abs(x % (xGridPitch * 5.0)) > (xGridPitch * 4.5))
        //        {
        //            tLine.Color = gridColor1;
        //            tLine.LineType = LineType.Solid;
        //            tLine.LineWidth = 0.5f;
        //            //座標文字
        //            if (this.GraduationOutside == false)
        //                graduationLayer.AddEntity(new Text(x.ToString(decFormat),
        //                                            new PointD(x + (gridBoundary[1].X - gridBoundary[0].X) / this.OpenGLWindow.Width * 5.0,
        //                                                       gridBoundary[0].Y + (gridBoundary[1].Y - gridBoundary[0].Y) / this.OpenGLWindow.Height * 5.0,
        //                                                       gridBoundary[0].Z),
        //                                            graduationColor));
        //            else
        //                graduationLayer.AddEntity(new Text(x.ToString(decFormat),
        //                                            new PointD(x,
        //                                                       viewBoundary[0].Y + (viewBoundary[1].Y - viewBoundary[0].Y) / this.OpenGLWindow.Height * 5.0,
        //                                                       gridBoundary[0].Z),
        //                                            graduationColor));
        //        }
        //        //細格線
        //        else
        //        {
        //            tLine.Color = gridColor2;
        //            tLine.LineType = LineType.Solid;
        //            tLine.LineWidth = 0.3f;
        //        }
        //        //加入格線
        //        gridLayer.AddEntity(tLine);
        //    }
        //    for (double y = Y1; y <= Y2; y += yGridPitch)
        //    {
        //        //格線
        //        tLine = new Line(new PointD(gridBoundary[0].X, y, gridBoundary[0].Z),
        //                         new PointD(gridBoundary[1].X, y, gridBoundary[0].Z));
        //        //粗格線
        //        if (Abs(y % (yGridPitch * 5.0)) < (yGridPitch * 0.5) || Abs(y % (yGridPitch * 5.0)) > (yGridPitch * 4.5))
        //        {
        //            tLine.Color = gridColor1;
        //            tLine.LineType = LineType.Solid;
        //            tLine.LineWidth = 0.5f;
        //            //座標文字
        //            if (this.GraduationOutside == false)
        //                graduationLayer.AddEntity(new Text(y.ToString(decFormat),
        //                                   new PointD(gridBoundary[0].X + (gridBoundary[1].X - gridBoundary[0].X) / this.OpenGLWindow.Width * 5.0,
        //                                              y + (gridBoundary[1].Y - gridBoundary[0].Y) / this.OpenGLWindow.Height * 5.0,
        //                                              gridBoundary[0].Z),
        //                                   graduationColor));
        //            else
        //                graduationLayer.AddEntity(new Text(y.ToString(decFormat),
        //                                   new PointD(viewBoundary[0].X + (viewBoundary[1].X - viewBoundary[0].X) / this.OpenGLWindow.Width * 5.0,
        //                                              y,
        //                                              gridBoundary[0].Z),
        //                                   graduationColor));
        //        }
        //        //細格線
        //        else
        //        {
        //            tLine.Color = gridColor2;
        //            tLine.LineType = LineType.Solid;
        //            tLine.LineWidth = 0.3f;
        //        }
        //        //加入格線
        //        gridLayer.AddEntity(tLine);
        //    }

        //    //邊框
        //    //下
        //    tLine = new Line(new PointD(gridBoundary[0].X,
        //                                gridBoundary[0].Y,
        //                                gridBoundary[0].Z),
        //                     new PointD(gridBoundary[1].X,
        //                                gridBoundary[0].Y,
        //                                gridBoundary[1].Z));
        //    tLine.Color = gridColor1;
        //    tLine.LineType = LineType.Solid;
        //    tLine.LineWidth = 0.5f;
        //    gridLayer.AddEntity(tLine);
        //    //右
        //    tLine = new Line(new PointD(gridBoundary[1].X,
        //                                gridBoundary[0].Y,
        //                                gridBoundary[1].Z),
        //                     new PointD(gridBoundary[1].X,
        //                                gridBoundary[1].Y,
        //                                gridBoundary[1].Z));
        //    tLine.Color = gridColor1;
        //    tLine.LineType = LineType.Solid;
        //    tLine.LineWidth = 0.5f;
        //    gridLayer.AddEntity(tLine);
        //    //上
        //    tLine = new Line(new PointD(gridBoundary[1].X,
        //                                gridBoundary[1].Y,
        //                                gridBoundary[1].Z),
        //                     new PointD(gridBoundary[0].X,
        //                                gridBoundary[1].Y,
        //                                gridBoundary[0].Z));
        //    tLine.Color = gridColor1;
        //    tLine.LineType = LineType.Solid;
        //    tLine.LineWidth = 0.5f;
        //    gridLayer.AddEntity(tLine);
        //    //左
        //    tLine = new Line(new PointD(gridBoundary[0].X,
        //                                gridBoundary[1].Y,
        //                                gridBoundary[0].Z),
        //                     new PointD(gridBoundary[0].X,
        //                                gridBoundary[0].Y,
        //                                gridBoundary[0].Z));
        //    tLine.Color = gridColor1;
        //    tLine.LineType = LineType.Solid;
        //    tLine.LineWidth = 0.5f;
        //    gridLayer.AddEntity(tLine);
        //    #endregion 計算格線
        //}

        //protected virtual void CalculateViewBoundary()
        //{
        //    #region 計算視野範圍
        //    viewBoundary[0] = centerPoint
        //        - new XYZ4((Double)openGLWindow.M_Translation.X, (Double)openGLWindow.M_Translation.Y, 0) / (Double)openGLWindow.M_Scale
        //        - new XYZ4(Range,
        //                    Range * ((Double)openGLWindow.Height / (Double)openGLWindow.Width),
        //                    0) / (Double)openGLWindow.M_Scale;
        //    viewBoundary[1] = centerPoint
        //        - new XYZ4((Double)openGLWindow.M_Translation.X, (Double)openGLWindow.M_Translation.Y, 0) / (Double)openGLWindow.M_Scale
        //        + new XYZ4(Range,
        //                    Range * ((Double)openGLWindow.Height / (Double)openGLWindow.Width),
        //                    0) / (Double)openGLWindow.M_Scale;

        //    gridSize = viewBoundary[1] - viewBoundary[0];

        //    //依AspectRatio縮放
        //    viewBoundary[0].X = this.centerPoint.X + (viewBoundary[0].X - this.centerPoint.X) / this.XScale;
        //    viewBoundary[1].X = this.centerPoint.X + (viewBoundary[1].X - this.centerPoint.X) / this.XScale;
        //    viewBoundary[0].Y = this.centerPoint.Y + (viewBoundary[0].Y - this.centerPoint.Y) / this.YScale;
        //    viewBoundary[1].Y = this.centerPoint.Y + (viewBoundary[1].Y - this.centerPoint.Y) / this.YScale;

        //    #endregion 計算格線範圍
        //}

        //protected virtual void CalculateGridPitch()
        //{
        //    //計算格線密度 1 2 5
        //    #region X
        //    int i = 0;
        //    int j = 0;
        //    bool findPicth = false;
        //    while (!double.IsNaN(viewBoundary[0].X)
        //        && !double.IsNegativeInfinity(viewBoundary[0].X)
        //        && !double.IsPositiveInfinity(viewBoundary[0].X)
        //        && !double.IsNaN(viewBoundary[1].X)
        //        && !double.IsNegativeInfinity(viewBoundary[1].X)
        //        && !double.IsPositiveInfinity(viewBoundary[1].X))
        //    {
        //        for (j = 0; j < gridPitchOption.Length; j++)
        //            if (openGLWindow.Width / ((viewBoundary[1].X - viewBoundary[0].X) / (gridPitchOption[j] * Pow(10, i))) >= minGridPitch)
        //            {
        //                findPicth = true;
        //                break;
        //            }
        //        if (findPicth)
        //            break;
        //        i++;
        //    }
        //    if (i == 0 && j == 0)
        //    {
        //        findPicth = false;
        //        i--;
        //        while (true)
        //        {
        //            for (j = gridPitchOption.Length - 1; j >= 0; j--)
        //                if (openGLWindow.Width / ((viewBoundary[1].X - viewBoundary[0].X) / (gridPitchOption[j] * Pow(10, i))) < minGridPitch || (i == -4 && j == 0))
        //                {
        //                    findPicth = true;
        //                    break;
        //                }
        //            if (findPicth)
        //                break;
        //            i--;
        //        }

        //        if (j < gridPitchOption.Length - 1)
        //            j += 1;
        //        else
        //        {
        //            i += 1;
        //            j = 0;
        //        }
        //    }

        //    xGridPitch = gridPitchOption[j] * Pow(10, i);
        //    #endregion

        //    #region Y
        //    i = 0;
        //    j = 0;
        //    findPicth = false;
        //    while (
        //           !double.IsNaN(viewBoundary[0].Y)
        //        && !double.IsNegativeInfinity(viewBoundary[0].Y)
        //        && !double.IsPositiveInfinity(viewBoundary[0].Y)
        //        && !double.IsNaN(viewBoundary[1].Y)
        //        && !double.IsNegativeInfinity(viewBoundary[1].Y)
        //        && !double.IsPositiveInfinity(viewBoundary[1].Y))
        //    {
        //        for (j = 0; j < gridPitchOption.Length; j++)
        //            if (openGLWindow.Height / ((viewBoundary[1].Y - viewBoundary[0].Y) / (gridPitchOption[j] * Pow(10, i))) >= minGridPitch)
        //            {
        //                findPicth = true;
        //                break;
        //            }
        //        if (findPicth)
        //            break;
        //        i++;
        //    }
        //    if (i == 0 && j == 0)
        //    {
        //        findPicth = false;
        //        i--;
        //        while (true)
        //        {
        //            for (j = gridPitchOption.Length - 1; j >= 0; j--)
        //                if (openGLWindow.Height / ((viewBoundary[1].Y - viewBoundary[0].Y) / (gridPitchOption[j] * Pow(10, i))) < minGridPitch || (i == -4 && j == 0))
        //                {
        //                    findPicth = true;
        //                    break;
        //                }
        //            if (findPicth)
        //                break;
        //            i--;
        //        }

        //        if (j < gridPitchOption.Length - 1)
        //            j += 1;
        //        else
        //        {
        //            i += 1;
        //            j = 0;
        //        }
        //    }

        //    yGridPitch = gridPitchOption[j] * Pow(10, i);
        //    #endregion
        //}
        #endregion Grid

        #region File Drop
        private async void DropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (String[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in filePaths)
                {
                    if (file.Split('.').Last().ToLower() == "stl")
                    {
                        STL stl = await STLReader.ReadSTLFile(file);
                        stl.Color = System.Drawing.Color.FromArgb(255, 255, 255, 0);
                        Model3D mGeometry = stl.ToModel3D();
                        this.AddInteractiveModel(mGeometry);
                        this.TranslateViewTo(AllModels);
                    }
                }
            }
        }
        #endregion File Drop
    }
}
