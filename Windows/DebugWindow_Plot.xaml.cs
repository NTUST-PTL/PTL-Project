using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using PTL.Windows.UIExtentions;

namespace PTL.Windows
{
    /// <summary>
    /// DebugWindow_Plot.xaml 的互動邏輯
    /// </summary>
    public partial class DebugWindow_Plot : Window
    {
        public PTL.Windows.Controls.OpenGLWindow OpenGLWindow;
        public PTL.OpenGL.Plot.OpenGLView View;

        public DebugWindow_Plot()
        {
            InitializeComponent();
            this.SourceInitialized += new System.EventHandler(MainWindow_SourceInitialized);

            this.OpenGLWindow = new Controls.OpenGLWindow();
            this.View = new OpenGL.Plot.OpenGLView(this.OpenGLWindow);
            this.View.AutoScale = true;
            
            this.OpenGLWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WindowsFormsHost.Child = this.OpenGLWindow;
        }

        public TextBox CreateNewLogTextBox(string name, System.Drawing.Color textColor)
        {
            TextBox newTextBox = new TextBox();
            newTextBox.Name = name;
            newTextBox.Foreground = new SolidColorBrush(Color.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B));
            newTextBox.Margin = new Thickness(0, 2.5, 0, 2.5);
            newTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            newTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            newTextBox.AcceptsReturn = true;
            newTextBox.AcceptsTab = true;
            newTextBox.AutoWordSelection = true;
            newTextBox.TextChanged += LogTextBox_TextChanged;

            TextBlock title = new TextBlock();
            title.Text = name;

            DockPanel newStackPanel = new DockPanel();
            newStackPanel.Children.Add(title);
            newStackPanel.Children.Add(newTextBox);

            RowDefinition newRow = new RowDefinition();
            newRow.Height = new GridLength(1, GridUnitType.Star);
            this.LogGrid.RowDefinitions.Add(newRow);
            this.LogGrid.Children.Add(newStackPanel);
            Grid.SetRow(newStackPanel, this.LogGrid.RowDefinitions.Count - 1);

            return newTextBox;
        }

        #region 外觀介面控制
        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource hwndSource;
        IntPtr retInt = IntPtr.Zero;

        void MainWindow_SourceInitialized(object sender, System.EventArgs e)
        {
            hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            hwndSource.AddHook(new HwndSourceHook(WndProc));
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Debug.WriteLine("WndProc messages: " + msg.ToString());

            if (msg == WM_SYSCOMMAND)
            {
                //Debug.WriteLine("WndProc messages: " + msg.ToString());
            }

            return IntPtr.Zero;
        }

        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Ellipse_Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Ellipse_Minimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Ellipse_Maximize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = (this.WindowState != WindowState.Maximized) ? WindowState.Maximized : WindowState.Normal;
        }

        private void thumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thumb thumb = sender as Thumb;

            switch (thumb.Name.Substring(5))
            {
                case "Top":
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "Bottom":
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "Left":
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "Right":
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "TopLeft":
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "TopRight":
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "BottomLeft":
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "BottomRight":
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }

        }

        #endregion

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.LogGrid.FindVisualChildren<TextBox>())
                item.Text = null;
        }

        private void LogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.AutoScrollCheckBox.IsChecked == true)
            {
                TextBox tb = sender as TextBox;
                tb.ScrollToEnd();
            }
        }

        private void WrapCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chb = sender as CheckBox;
            TextWrapping wrapingType = TextWrapping.Wrap;
            if (chb.IsChecked == false)
                wrapingType = TextWrapping.NoWrap;
            foreach (TextBox item in this.LogGrid.Children)
                item.TextWrapping = wrapingType;
        }
    }
}
