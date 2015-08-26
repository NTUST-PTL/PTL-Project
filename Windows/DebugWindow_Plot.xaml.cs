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
    }
}
