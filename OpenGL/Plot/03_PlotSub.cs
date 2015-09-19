using System;
using System.Drawing;
using CsGL.OpenGL;  // 引入 CsGL.OpenGL 
using System.Runtime.InteropServices;
using PTL.Definitions;
using PTL.Windows.Controls;
using PTL.Geometry;

namespace PTL.OpenGL.Plot
{
    public class PlotSub : PTL.Mathematics.Math
    {
        #region OpenGL 基本程式
        public static void PlotBackground(float colorR, float colorG, float colorB)
        {
            GL.glClearColor(System.Convert.ToSingle(colorR / 255.0), System.Convert.ToSingle(colorG / 255.0), System.Convert.ToSingle(colorB / 255.0), 1.0f);//R G B alpha
            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);// Clear Screen And Depth Buffer
            GL.glLoadIdentity();
        }
        public static void PlotBackground(double colorR, double colorG, double colorB)
        {
            GL.glClearColor(System.Convert.ToSingle(colorR / 255.0), System.Convert.ToSingle(colorG / 255.0), System.Convert.ToSingle(colorB / 255.0), 1.0f);//R G B alpha
            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);// Clear Screen And Depth Buffer
            GL.glLoadIdentity();
        }
        public static void PlotBackground(System.Drawing.Color color)
        {
            GL.glClearColor(System.Convert.ToSingle(color.R / 255.0), System.Convert.ToSingle(color.G / 255.0), System.Convert.ToSingle(color.B / 255.0), System.Convert.ToSingle(color.A / 255.0));//R G B alpha
            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);// Clear Screen And Depth Buffer
            GL.glLoadIdentity();
            GL.glFlush();
        }
        public static void SetView(int width, int height, double range)
        {
            GL.glViewport(0, 0, width, height);
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();
            double h = height;
            double w = width;
            if (height == 0)// prevent a divide by zero
            {
                h = 1;
            }
            if (h > w)
            {
                GL.glOrtho(-range, range, -range * h / w, range * h / w, -range * 1000, range * 1000);
            }
            else
            {
                GL.glOrtho(-range * w / h, range * w / h, -range, range, -range * 1000, range * 1000);
            }
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();
            GL.glFlush();
        }
        public static void Light()
        {
            float[] ambientLight0 = { 0.2f, 0.2f, 0.2f, 1.0f }; // Light values and coordinates  光源亮度
            float[] diffuseLight0 = { 0.7f, 0.7f, 0.7f, 1.0f };

            float[] ambientLight1 = { 0.2f, 0.2f, 0.2f, 1.0f }; // Light values and coordinates  光源亮度
            float[] diffuseLight1 = { 0.7f, 0.7f, 0.7f, 1.0f };

            float[] specular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] specref = { 1.0f, 1.0f, 1.0f, 1.0f };//反射光
            float[] LightPos0 = { -1.0f, -1.0f, -1.0f, 0.0f };//光源位置
            float[] LightPos1 = { 1.0f, 1.0f, 1.0f, 0.0f };

            GL.glEnable(GL.GL_DEPTH_TEST);//Hidden surface removal
            GL.glEnable(GL.GL_CULL_FACE);//Do not calculate inside face
            GL.glCullFace(GL.GL_BACK);

            //GL.glFrontFace(GL.GL_CW);//CW polygon face out
            GL.glEnable(GL.GL_LIGHTING);// Enable lighting Computing   
            GL.glEnable(GL.GL_NORMALIZE);


            GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, ambientLight0); // Setup and enable light 0
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, diffuseLight0);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, specular);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, LightPos0);
            GL.glEnable(GL.GL_LIGHT0);

            GL.glLightfv(GL.GL_LIGHT1, GL.GL_AMBIENT, ambientLight1); // Setup and enable light 0
            GL.glLightfv(GL.GL_LIGHT1, GL.GL_DIFFUSE, diffuseLight1);
            GL.glLightfv(GL.GL_LIGHT1, GL.GL_SPECULAR, specular);
            GL.glLightfv(GL.GL_LIGHT1, GL.GL_POSITION, LightPos1);
            GL.glEnable(GL.GL_LIGHT1);

            GL.glEnable(GL.GL_COLOR_MATERIAL);// Enable material color tracking            
            GL.glColorMaterial(GL.GL_FRONT, GL.GL_AMBIENT_AND_DIFFUSE);// Set Material properties to follow glColor values

            // All materials hereafter have full specular reflectivity
            // with a high shine
            GL.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, specref);
            GL.glMateriali(GL.GL_FRONT, GL.GL_SHININESS, 128);
            // GL.glShadeModel(GL.GL_SMOOTH);// Enable smooth shading
            GL.glFlush();
        }
        public static void SmoothLine()
        {
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glEnable(GL.GL_LINE_SMOOTH);
        }
        public static void glLineWidth(double Width)
        {
            GL.glLineWidth((float)Width);
        }
        public static void glColor3d(double r, double g, double b)
        {
            GL.glColor3d(r / 255.0, g / 255.0, b / 255.0);
        }
        public static void glColor3d(Color color)
        {
            glColor3d(color.R, color.G, color.B);
        }
        public static void glColor4d(double r, double g, double b, double a)
        {
            GL.glColor4d(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
        }
        public static void glColor4d(Color color)
        {
            glColor4d(color.R, color.G, color.B, color.A);
        }
        public static void MultMatrixd(double[,] Mab)
        {
            double[] Mab1D = new double[16] { Mab[0, 0], Mab[1, 0], Mab[2, 0], Mab[3, 0],
                                              Mab[0, 1], Mab[1, 1], Mab[2, 1], Mab[3, 1],
                                              Mab[0, 2], Mab[1, 2], Mab[2, 2], Mab[3, 2],
                                              Mab[0, 3], Mab[1, 3], Mab[2, 3], Mab[3, 3]};
            GL.glMultMatrixd(Mab1D);
        }
        public static void Translated(double[] p1)
        {
            GL.glTranslated(p1[0], p1[1], p1[2]);
        }
        public static void Vertex3d(double[] p1)
        {
            GL.glVertex3d(p1[0], p1[1], p1[2]);
        }
        #endregion

        #region OpenGL文字繪製
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("opengl32.dll")]
        public static extern bool wglUseFontBitmapsW(IntPtr hDC, uint word, uint count, uint listBase);
        [DllImport("opengl32.dll")]
        public static extern bool wglUseFontBitmaps(IntPtr hDC, uint word, uint count, uint listBase);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement,
                int nOrientation, uint fnWeight, uint fdwItalic, uint fdwUnderline, uint
                fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint
                fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

        public static void SelectFont(int size, string font)
        {
            IntPtr hDC = GetDC(IntPtr.Zero);
            IntPtr hOldFont1 = new IntPtr();
            IntPtr hOldFont2 = new IntPtr();
            uint lists = GL.glGenLists(1000) - 1;
            hOldFont1 = (IntPtr)CreateFont(size, 0, 0, 0, 500, 0, 0, 0, 0, 100, 100, 5, 2, font);
            hOldFont2 = (IntPtr)SelectObject(hDC, hOldFont1);
            wglUseFontBitmapsW(hDC, 0, 1000, lists);
            SelectObject(hDC, hOldFont2);
            DeleteObject(hOldFont1);
        }
        public static void SelectFontChinese(int size, string font)
        {
            IntPtr hDC = GetDC(IntPtr.Zero);
            IntPtr hOldFont1 = new IntPtr();
            IntPtr hOldFont2 = new IntPtr();
            uint lists = GL.glGenLists(50000) - 1;
            hOldFont1 = CreateFont(size, 0, 0, 0, 500, 0, 0, 0, 0, 100, 100, 5, 2, font);
            hOldFont2 = (IntPtr)SelectObject(hDC, hOldFont1);
            wglUseFontBitmapsW(hDC, 0, 50000, lists);
            SelectObject(hDC, hOldFont2);
            DeleteObject(hOldFont1);
        }
        public static void WriteString(string str, double x, double y, double z, float colorR, float colorG, float colorB)
        {
            GL.glPushMatrix();
            GL.glColor3f(colorR, colorG, colorB);
            GL.glRasterPos3d(x, y, z);
            for (int i = 0; i < str.Length; i++)
            {
                uint tt = (uint)(str[i]);
                GL.glCallList(tt);
            }
            GL.glPopMatrix();
        }
        public static void WriteString(string str, double x, double y, double z, Color color)
        {
            GL.glPushMatrix();
            glColor3d(color);
            GL.glRasterPos3d(x, y, z);
            for (int i = 0; i < str.Length; i++)
            {
                uint tt = (uint)(str[i]);
                GL.glCallList(tt);
            }
            GL.glPopMatrix();
        }
        public static void WriteString(string str, Vector p1, float colorR, float colorG, float colorB)
        {
            GL.glPushMatrix();
            GL.glColor3f(colorR, colorG, colorB);
            GL.glRasterPos3d(p1.X, p1.Y, p1.Z);
            for (int i = 0; i < str.Length; i++)
            {
                uint tt = (uint)(str[i]);
                GL.glCallList(tt);
            }
            GL.glPopMatrix();
        }
        public static void WriteString(string str, Vector p1, Color color)
        {
            GL.glPushMatrix();
            glColor3d(color);
            GL.glRasterPos3d(p1.X, p1.Y, p1.Z);
            for (int i = 0; i < str.Length; i++)
            {
                uint tt = (uint)(str[i]);
                GL.glCallList(tt);
            }
            GL.glPopMatrix();
        }
        #endregion OpenGL文字繪製

        #region OpenGL Line Style
        public static void SetLineType(LineType Type, int factor)
        {
            GL.glEnable(GL.GL_LINE_STIPPLE);
            GL.glLineStipple(factor, (ushort)Type);
        }
        public static void SetLineType(LineType Type)
        {
            SetLineType(Type, 3);
        }
        #endregion

        #region 幾何圖形
        public static void PlotLine(PointD[] PointSet)
        {
            GL.glBegin(GL.GL_LINE_STRIP);
            for (int k = 0; k < PointSet.Length; k++)
            {
                Vertex3d(PointSet[k]);
            }
            GL.glEnd();
        }
        public static void PlotCircle(PointD center, double radius)
        {
            int nump = 360;
            double step = 2.0 * PI / nump;
            double thetaz;
            PointD pnt;

            GL.glBegin(GL.GL_LINE_STRIP);
            for (int i = 0; i < nump; i++)
            {
                thetaz = step * i;
                pnt = center + new PointD(radius * Cos(thetaz), radius * Sin(thetaz), 0.0);
                Vertex3d(pnt);
            }
            thetaz = 0.0;
            pnt = center + new PointD(radius * Cos(thetaz), radius * Sin(thetaz), 0.0);
            Vertex3d(pnt);
            GL.glEnd();
        }
        public static void PlotArc(PointD center, double radius, double startang, double endang)
        {
            int nump = (int)Abs(RadToDeg(endang - startang) * 3);
            double step = (endang - startang) / (nump - 1);
            double thetaz;
            PointD pnt;

            GL.glBegin(GL.GL_LINE_STRIP);
            for (int i = 0; i < nump; i++)
            {
                thetaz = startang + step * i;
                pnt = center + new PointD(radius * Cos(thetaz), radius * Sin(thetaz), 0.0);
                Vertex3d(pnt);
            }
            GL.glEnd();
        }
        public static void PlotTriangle(PointD p1, PointD p2, PointD p3, PointD Referpn, double theta, Color color)
        {
            //Line Style
            glColor3d(color);

            GL.glPushMatrix();

            Translated(Referpn);
            GL.glRotated(theta, 0, 0, 1.0);

            GL.glBegin(GL.GL_TRIANGLES);
            Vertex3d(p1);
            Vertex3d(p2);
            Vertex3d(p3);
            GL.glEnd();

            GL.glPopMatrix();
        }
        public static void PlotArcXY(PointD Center, double radius, double StartAng, double EndAng)
        {
            //Point Number
            double step = DegToRad(1.0);
            int NumP = (int)Round((EndAng - StartAng) / step, 1.0);
            if (NumP < 2)
                NumP = 2;

            //Points
            double theta = 0;
            PointD[] PointSet = new PointD[NumP];
            for (int i = 0; i < NumP; i++)
            {
                theta = StartAng + step * i;
                PointSet[i] = new PointD(radius * Cos(theta), radius * Sin(theta), 0.0);
            }

            //Plot Arc
            PlotLine(PointSet);
        }
        public static void PlotArcXY(PointD p1, PointD p2, double radius, short ctype)
        {
            double ra = Abs(radius);
            short Flag = (short)(radius / ra);

            //Center Point
            PointD pc = CalcuCenterXY(p1, p2, radius, ctype);

            //u Range
            PointD xa = new PointD(1.0, 0.0, 0.0);
            PointD ya = new PointD(0.0, 1.0, 0.0);

            double Ang1 = Atan2(Dot(p1 - pc, ya), Dot(p1 - pc, xa));
            double Ang2 = Atan2(Dot(p2 - pc, ya), Dot(p2 - pc, xa));

            double Ang3, Ang4;
            if (Ang1 < Ang2)
            {
                Ang3 = 2 * PI + Ang1;
                Ang4 = Ang2;
            }
            else
            {
                Ang3 = Ang1;
                Ang4 = 2 * PI + Ang2;
            }

            double DeltaAng1 = Abs(Ang2 - Ang1);
            double DeltaAng2 = Abs(Ang4 - Ang3);

            double AngS, AngE;
            if (Flag > 0)
            {
                if (DeltaAng1 <= PI)
                {
                    AngS = Ang1;
                    AngE = Ang2;
                }
                else
                {
                    AngS = Ang3;
                    AngE = Ang4;
                }
            }
            else
            {
                if (DeltaAng1 <= PI)
                {
                    AngS = Ang3;
                    AngE = Ang4;
                }
                else
                {
                    AngS = Ang1;
                    AngE = Ang2;
                }
            }

            //Point Number
            double step = DegToRad(1.0);
            int NumP = (int)Round((AngE - AngS) / step, 1.0);
            if (NumP < 2)
                NumP = 2;
            step = (AngE - AngS) / (NumP - 1);

            double theta = 0;

            //Points
            PointD[] PointSet = new PointD[NumP];
            for (int i = 0; i < NumP; i++)
            {
                theta = AngS + step * i;
                PointSet[i] = new PointD(ra * Cos(theta), ra * Sin(theta), 0.0);
            }

            //Plot Arc
            PlotLine(PointSet);
        }
        #endregion
   }
}
