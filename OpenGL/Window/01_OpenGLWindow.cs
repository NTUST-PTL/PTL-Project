using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.Platform.Windows;
using PTL.OpenGL.Plot;

namespace PTL.OpenGL.Window
{
    /// <summary>
    /// 具備滑鼠控制器的OpenGL視窗
    /// 已定義：
    ///     MouseDown事件
    ///     MouseMove事件
    ///     MouseUp事件
    ///     MouseWheel事件
    ///     MouseDoubleClick事件
    ///     SizeChanged事件
    /// 未定義：
    ///     Paint事件
    ///     畫面刷新時機
    /// </summary>
    public class OpenGLWindow : SimpleOpenGlControl
    {
        public MouseControl MouseControl { get; set; }

        public OpenGLWindow()
            : base()
        {
            this.InitializeContexts();//初始化simpleopenglcontrol1
            PlotSub.SelectFont(22, "Times New Roman");
            this.MouseControl = new MouseControl(this);
            this.SizeChanged += simpleOpenGlControl1_SizeChanged;
        }

        private void simpleOpenGlControl1_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();//更新畫面
        }

        public void Refresh(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}
