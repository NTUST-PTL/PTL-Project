using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsGL.OpenGL;
using Tao.Platform.Windows;
using PTL.Definitions;
using PTL.OpenGL.Plot;

namespace PTL.Windows.Controls
{
    /// <summary>
    /// OpenGL視窗的2D滑鼠控制介面
    /// </summary>
    public interface IOpenGLWindowMouseControl
    {
        Double Range { get; set; }

        void MouseDown_EventHandler(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseMove_EventHandler(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseUp_EventHandler(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseWheel_EventHandler(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseDoubleClick_EventHandler(object sender, System.Windows.Forms.MouseEventArgs e);

        void MouseMoveInOpenGLPaint();
    }

    /// <summary>
    /// 提供OpenGL視窗的2D或3D滑鼠控制功能
    /// </summary>
    public class OpenGLWindow : SimpleOpenGlControl, IOpenGLWindowMouseControl
    {
        //-------opengl滑鼠控制-------------------
        #region Mouse Variable
        public Dimension Dimension { get; set; }
        public enum MouseFunction
        {
            Move,
            Rotate,
            Null
        }
        public Dictionary<System.Windows.Forms.MouseButtons, MouseFunction> ButtonSettings
            = new Dictionary<MouseButtons, MouseFunction>
            { {System.Windows.Forms.MouseButtons.Left, MouseFunction.Move},
              {System.Windows.Forms.MouseButtons.Middle, MouseFunction.Null},
              {System.Windows.Forms.MouseButtons.Right, MouseFunction.Rotate},
              {System.Windows.Forms.MouseButtons.None, MouseFunction.Null},
              {System.Windows.Forms.MouseButtons.XButton1, MouseFunction.Null},
              {System.Windows.Forms.MouseButtons.XButton2, MouseFunction.Null}};

        public bool RotateButton_down;
        public bool MoveButton_down;

        public Double Range { get; set; }
        public float Mouse_Scale_Sensitive;

        public PointF m_RotationD;
        public PointF m_TranslationD;
        public PointF m_Rotation;
        public PointF m_Translation;
        public float m_Scale;
        public PointF M_RotationD {
            get { return m_RotationD ;}
            set { 
                if (this.m_RotationD != value)
	            {
		            this.m_RotationD = value;
	            }
            }
        }
        public PointF M_TranslationD
        {
            get { return m_TranslationD; }
            set
            {
                if (this.m_TranslationD != value)
                {
                    this.m_TranslationD = value;
                }
            }
        }
        public PointF M_Rotation
        {
            get { return m_Rotation; }
            set
            {
                if (this.m_Rotation != value)
                {
                    this.m_Rotation = value;
                }
            }
        }
        public PointF M_Translation
        {
            get { return m_Translation; }
            set
            {
                if (this.m_Translation != value)
                {
                    this.m_Translation = value;
                }
            }
        }
        public float M_Scale
        {
            get { return m_Scale; }
            set
            {
                if (this.m_Scale != value)
                {
                    this.m_Scale = value;
                }
            }
        }

        public bool AutoRefresh { get; set; }
        #endregion

        public OpenGLWindow()
            : base()
        {
            this.InitializeContexts();//初始化simpleopenglcontrol1
            PTL.OpenGL.Plot.PlotSub.SelectFont(22, "Times New Roman");
            this.SizeChanged += simpleOpenGlControl1_SizeChanged;

            this.Dimension = Dimension.ThreeDimension;

            this.RotateButton_down = false;
            this.MoveButton_down = false;

            this.Range = 100;
            this.Mouse_Scale_Sensitive = 1.2f;
            this.M_Scale = 1;
            this.AutoRefresh = true;

            this.MouseDown += this.MouseDown_EventHandler;
            this.MouseMove += this.MouseMove_EventHandler;
            this.MouseUp += this.MouseUp_EventHandler;
            this.MouseWheel += this.MouseWheel_EventHandler;
            this.MouseDoubleClick += this.MouseDoubleClick_EventHandler;
        }

        public virtual void MouseDown_EventHandler(object sender, MouseEventArgs e)
        {
            MouseFunction currentFunction = ButtonSettings[e.Button];
            if (currentFunction == MouseFunction.Rotate)//按下旋轉鍵
            {
                this.RotateButton_down = true;
                this.M_RotationD = e.Location; //儲存目前滑鼠按下的座標
            }
            if (currentFunction == MouseFunction.Move)//按下移動鍵
            {
                this.MoveButton_down = true;
                this.M_TranslationD = e.Location; //儲存目前滑鼠按下的座標
            }
        }

        public virtual void MouseMove_EventHandler(object sender, MouseEventArgs e)
        {
            SimpleOpenGlControl openGLWindow = (SimpleOpenGlControl)sender;
            if (this.RotateButton_down == true && Dimension == Dimension.ThreeDimension)
            {
                this.m_Rotation.Y -= (this.M_RotationD.X - e.X) * 0.5f;//繞x軸轉動幾度
                this.m_Rotation.X -= (this.M_RotationD.Y - e.Y) * 0.5f;//繞y軸轉動幾度
                this.M_RotationD = e.Location;
                if (AutoRefresh == true)
                    openGLWindow.Refresh();
            }
            if (this.MoveButton_down == true)
            {
                this.m_Translation.X -= (this.M_TranslationD.X - e.X) / ((float)this.Width / 2.0f) * (float)Range; //x軸平移多少距離
                this.m_Translation.Y += (this.M_TranslationD.Y - e.Y) / ((float)this.Width / 2.0f) * (float)Range; //y軸平移多少距離
                this.M_TranslationD = e.Location;
                if (AutoRefresh == true)
                    openGLWindow.Refresh();
            }
        }

        public virtual void MouseUp_EventHandler(object sender, MouseEventArgs e)
        {
            this.RotateButton_down = false;
            this.MoveButton_down = false;
        }

        public virtual void MouseWheel_EventHandler(object sender, MouseEventArgs e)
        {
            SimpleOpenGlControl openGLWindow = (SimpleOpenGlControl)sender;

            if (e.Delta > 0)//滾輪轉的方向為正 倍率增加
            {
                this.m_Scale *= this.Mouse_Scale_Sensitive;
                this.m_Translation.X -= (this.M_Translation.X
                                                    - (e.Location.X - this.Width / 2.0f) / (this.Width / 2.0f) * (float)this.Range)
                                                    * (1.0f - this.Mouse_Scale_Sensitive);  //x軸平移多少距離
                this.m_Translation.Y -= (this.M_Translation.Y
                                                    + (e.Location.Y - this.Height / 2.0f) / (this.Width / 2.0f) * (float)this.Range)
                                                    * (1.0f - this.Mouse_Scale_Sensitive); //y軸平移多少距離
            }
            if (e.Delta < 0)//滾輪轉的方向為負 倍率減少
            {
                this.m_Scale /= this.Mouse_Scale_Sensitive;
                this.m_Translation.X -= (this.M_Translation.X
                                                    - (e.Location.X - this.Width / 2.0f) / (this.Width / 2.0f) * (float)this.Range)
                                                    * (1.0f - 1.0f / this.Mouse_Scale_Sensitive);  //x軸平移多少距離
                this.m_Translation.Y -= (this.M_Translation.Y
                                                    + (e.Location.Y - this.Height / 2.0f) / (this.Width / 2.0f) * (float)this.Range)
                                                    * (1.0f - 1.0f / this.Mouse_Scale_Sensitive); //y軸平移多少距離
            }
            if (AutoRefresh == true)
                openGLWindow.Refresh();
        }

        public virtual void MouseDoubleClick_EventHandler(object sender, MouseEventArgs e)
        {
            SimpleOpenGlControl openGLWindow = (SimpleOpenGlControl)sender;
            this.M_Translation = new PointF(0.0f, 0.0f);
            this.M_Rotation = new PointF(0.0f, 0.0f);
            this.M_Scale = 1;
            if (AutoRefresh == true)
                openGLWindow.Refresh();
        }

        public virtual void Reset(object sender, EventArgs e)
        {
            SimpleOpenGlControl openGLWindow = (SimpleOpenGlControl)sender;
            this.M_Translation = new PointF(0.0f, 0.0f);
            this.M_Rotation = new PointF(0.0f, 0.0f);
            this.M_Scale = 1;
            if (AutoRefresh == true)
                openGLWindow.Refresh();
        }

        public virtual void MouseMoveInOpenGLPaint()
        {
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();
            GL.glTranslatef(this.M_Translation.X, this.M_Translation.Y, 0);//'沿x,y,z軸移動多少距離
            GL.glRotatef(this.M_Rotation.X, 1, 0, 0);// '繞 x軸旋轉幾度
            GL.glRotatef(this.M_Rotation.Y, 0, 1, 0);// '繞 y軸旋轉幾度
            GL.glScaled(this.M_Scale, this.M_Scale, this.M_Scale);//'x,y,z軸比例縮放
        }

        private void simpleOpenGlControl1_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();//更新畫面
        }

        public override void Refresh()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(Refresh));
            else
                base.Refresh();
        }
    }
}
