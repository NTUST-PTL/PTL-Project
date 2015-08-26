using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;
using System.Collections.Generic;
using Tao.Platform.Windows;
using PTL.OpenGL.Definitions;
using PTL.Geometry;

namespace PTL.OpenGL.Window
{
    /// <summary>
    /// OpenGL視窗的2D滑鼠控制介面
    /// </summary>
    public interface IOpenGLWindowMouseControl
    {
        SimpleOpenGlControl ConnectedOpenGLWindow { get; set; }
        Double Range { get; set; }

        void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e);
        void MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e);
    }

    /// <summary>
    /// 提供OpenGL視窗的2D或3D滑鼠控制功能
    /// </summary>
    public class MouseControl : IOpenGLWindowMouseControl
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

        public SimpleOpenGlControl ConnectedOpenGLWindow { get; set; }
        public Double Range { get; set; }
        public float Mouse_Scale_Sensitive;

        public PointF M_RotationD;
        public PointF M_TranslationD;
        public PointF M_Rotation;
        public PointF M_Translation;
        public float M_Scale;

        public bool AutoRefresh { get; set; }
        #endregion

        public MouseControl(SimpleOpenGlControl connectedOpenGLWindow)
        {
            this.Dimension = Dimension.TwoDimension;

            this.RotateButton_down = false;
            this.MoveButton_down = false;
            
            this.Range = 100;
            this.Mouse_Scale_Sensitive = 1.2f;
            this.M_Scale = 1;
            this.AutoRefresh = true;

            this.ConnectedOpenGLWindow = connectedOpenGLWindow;
            this.ConnectedOpenGLWindow.MouseDown += this.MouseDown;
            this.ConnectedOpenGLWindow.MouseMove += this.MouseMove;
            this.ConnectedOpenGLWindow.MouseUp += this.MouseUp;
            this.ConnectedOpenGLWindow.MouseWheel += this.MouseWheel;
            this.ConnectedOpenGLWindow.MouseDoubleClick += this.MouseDoubleClick;
        }

        public virtual void MouseDown(object sender, MouseEventArgs e)
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

        public virtual void MouseMove(object sender, MouseEventArgs e)
        {
            SimpleOpenGlControl openGLWindow = (SimpleOpenGlControl)sender;
            if (this.RotateButton_down == true && Dimension == Dimension.ThreeDimension)
            {
                this.M_Rotation.Y -= (this.M_RotationD.X - e.X) * 0.5f;//繞x軸轉動幾度
                this.M_Rotation.X -= (this.M_RotationD.Y - e.Y) * 0.5f;//繞y軸轉動幾度
                this.M_RotationD = e.Location;
                if (AutoRefresh == true)
                    openGLWindow.Refresh();
            }
            if (this.MoveButton_down == true)
            {
                //New Version
                if (this.ConnectedOpenGLWindow.Width >= this.ConnectedOpenGLWindow.Height)
                {
                    this.M_Translation.X -= (this.M_TranslationD.X - e.X) / ((float)this.ConnectedOpenGLWindow.Height / 2.0f) * (float)Range; //x軸平移多少距離
                    this.M_Translation.Y += (this.M_TranslationD.Y - e.Y) / ((float)this.ConnectedOpenGLWindow.Height / 2.0f) * (float)Range; //y軸平移多少距離
                    this.M_TranslationD = e.Location;
                }
                else
                {
                    this.M_Translation.X -= (this.M_TranslationD.X - e.X) / ((float)this.ConnectedOpenGLWindow.Width / 2.0f) * (float)Range; //x軸平移多少距離
                    this.M_Translation.Y += (this.M_TranslationD.Y - e.Y) / ((float)this.ConnectedOpenGLWindow.Width / 2.0f) * (float)Range; //y軸平移多少距離
                    this.M_TranslationD = e.Location;
                }
                if (AutoRefresh == true)
                    openGLWindow.Refresh();
            }
            
        }

        public virtual void MouseUp(object sender, MouseEventArgs e)
        {
            this.RotateButton_down = false;
            this.MoveButton_down = false;
        }

        public virtual void MouseWheel(object sender, MouseEventArgs e)
        {
            SimpleOpenGlControl openGLWindow = (SimpleOpenGlControl)sender;

            if (e.Delta > 0)//滾輪轉的方向為正 倍率增加
            {
                this.M_Scale *= this.Mouse_Scale_Sensitive;
                if (this.ConnectedOpenGLWindow.Width >= this.ConnectedOpenGLWindow.Height)
                {
                    this.M_Translation.X -= (this.M_Translation.X
                                                        - (e.Location.X - this.ConnectedOpenGLWindow.Width / 2.0f) / (this.ConnectedOpenGLWindow.Height / 2.0f) * (float)this.Range)
                                                        * (1.0f - this.Mouse_Scale_Sensitive);  //x軸平移多少距離
                    this.M_Translation.Y -= (this.M_Translation.Y
                                                        + (e.Location.Y - this.ConnectedOpenGLWindow.Height / 2.0f) / (this.ConnectedOpenGLWindow.Height / 2.0f) * (float)this.Range)
                                                        * (1.0f - this.Mouse_Scale_Sensitive); //y軸平移多少距離
                }
                else
                {
                    this.M_Translation.X -= (this.M_Translation.X
                                                        - (e.Location.X - this.ConnectedOpenGLWindow.Width / 2.0f) / (this.ConnectedOpenGLWindow.Width / 2.0f) * (float)this.Range)
                                                        * (1.0f - this.Mouse_Scale_Sensitive);  //x軸平移多少距離
                    this.M_Translation.Y -= (this.M_Translation.Y
                                                        + (e.Location.Y - this.ConnectedOpenGLWindow.Height / 2.0f) / (this.ConnectedOpenGLWindow.Width / 2.0f) * (float)this.Range)
                                                        * (1.0f - this.Mouse_Scale_Sensitive); //y軸平移多少距離
                }
            }
            if (e.Delta < 0)//滾輪轉的方向為負 倍率減少
            {
                this.M_Scale /= this.Mouse_Scale_Sensitive;
                if (this.ConnectedOpenGLWindow.Width >= this.ConnectedOpenGLWindow.Height)
                {
                    this.M_Translation.X -= (this.M_Translation.X
                                                        - (e.Location.X - this.ConnectedOpenGLWindow.Width / 2.0f) / (this.ConnectedOpenGLWindow.Height / 2.0f) * (float)this.Range)
                                                        * (1.0f - 1.0f / this.Mouse_Scale_Sensitive);  //x軸平移多少距離
                    this.M_Translation.Y -= (this.M_Translation.Y
                                                        + (e.Location.Y - this.ConnectedOpenGLWindow.Height / 2.0f) / (this.ConnectedOpenGLWindow.Height / 2.0f) * (float)this.Range)
                                                        * (1.0f - 1.0f / this.Mouse_Scale_Sensitive); //y軸平移多少距離
                }
                else
                {
                    this.M_Translation.X -= (this.M_Translation.X
                                                        - (e.Location.X - this.ConnectedOpenGLWindow.Width / 2.0f) / (this.ConnectedOpenGLWindow.Width / 2.0f) * (float)this.Range)
                                                        * (1.0f - 1.0f / this.Mouse_Scale_Sensitive);  //x軸平移多少距離
                    this.M_Translation.Y -= (this.M_Translation.Y
                                                        + (e.Location.Y - this.ConnectedOpenGLWindow.Height / 2.0f) / (this.ConnectedOpenGLWindow.Width / 2.0f) * (float)this.Range)
                                                        * (1.0f - 1.0f / this.Mouse_Scale_Sensitive); //y軸平移多少距離
                }
            }
            if (AutoRefresh == true)
                openGLWindow.Refresh();
        }

        public virtual void MouseDoubleClick(object sender, MouseEventArgs e)
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
    }
}
