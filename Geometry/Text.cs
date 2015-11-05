using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using CsGL.OpenGL;
using PTL.Definitions;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class Text : Entity, IHaveColor, ICanBeWritedToDXFFile, IScriptFile
    {
        public string Value = "Input Text";//Text

        public string FontStyle = "Times New Roman";//Text Style Name
        public double TextHieght = 22;
        public uint FontWeight = 500;
        public uint Italic = 0;
        public uint Underline = 0;

        public PointD RefPoint = new PointD();//First alignment point
        public string JustType = "Center";//Justification Type
        public double Dist = 10;
        public double Orietation = 0; //Text rotation


        #region Constructor and Destructor
        public Text(string value, PointD point, Color color, string justtype, string style, int rotateang, uint fontWeight)
        {
            Value = value;
            RefPoint = (PointD)point.Clone();
            Color = color;
            JustType = justtype;
            FontStyle = style;
            Orietation = rotateang;
            FontWeight = fontWeight;
        }
        public Text(string value, PointD point, Color color, String style, double textHieght)
        {
            this.Value = value;
            this.RefPoint = (PointD)point.Clone();
            this.Color = color;
            this.FontStyle = style;
            this.TextHieght = textHieght;
        }
        public Text(string value, PointD point, Color color)
        {
            this.Value = value;
            this.RefPoint = (PointD)point.Clone();
            this.Color = color;
        }
        public Text(string value, PointD point)
        {
            Value = value;
            RefPoint = (PointD)point.Clone();
        }
        public Text()
        {
        }
        #endregion

        #region DXFEntity
        public override XYZ4[] Boundary
        {
            get
            {
                XYZ4[] boundary;
                if (this.CoordinateSystem != null)
                    boundary = new XYZ4[2] { Transport4(this.CoordinateSystem, this.RefPoint), Transport4(this.CoordinateSystem, this.RefPoint) };
                else
                    boundary = new XYZ4[2] { (PointD)this.RefPoint.Clone(), (PointD)this.RefPoint.Clone() };
                return boundary;
            }
        }
        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            sw.WriteLine("0");
            sw.WriteLine("TEXT");
            sw.WriteLine("8");
            sw.WriteLine(this.Parent.Name);//圖層名稱
            sw.WriteLine("62");
            sw.WriteLine(Part.SystemColor2DXFColorIndex(this.Color));//顏色號碼
            sw.WriteLine("10");
            sw.WriteLine(this.RefPoint.X.ToString());//定義點X座標
            sw.WriteLine("20");
            sw.WriteLine(this.RefPoint.Y.ToString());//定義點Y座標
            sw.WriteLine("30");
            sw.WriteLine(this.RefPoint.Z.ToString());//定義點Z座標
            sw.WriteLine("40");
            sw.WriteLine(this.TextHieght.ToString());//文字高度
            sw.WriteLine("50");
            sw.WriteLine(this.Orietation.ToString());//文字旋轉
            sw.WriteLine("1");
            //對中文進行處理
            String chineseString = "";
            String finalString = "";
            for (int i = 0; i < this.Value.Length; i++)
            {
                if (System.Text.Encoding.Default.GetByteCount(this.Value[i].ToString()) > 1 && i != this.Value.Length - 1)
                    chineseString += this.Value[i];
                else
                {
                    if (chineseString != "")
                    {
                        finalString += "{\\fPMingLiU|b0|i0|c136|p18;" + chineseString + "}" + this.Value[i];
                        chineseString = "";
                    }
                    else
                        finalString += this.Value[i];
                }
            }
            sw.WriteLine(finalString);//字串本身
        }
        public void WriteToFileInScriptFormat(StreamWriter sw)
        {
            throw new NotImplementedException();
        }
        public override object Clone()
        {
            Text aText = new Text();
            if (this.Name != null)
                aText.Name = this.Name;
            aText.Value = this.Value;
            aText.TextHieght = this.TextHieght;
            aText.RefPoint = (PointD)this.RefPoint.Clone();
            aText.JustType = this.JustType;
            aText.FontStyle = this.FontStyle;
            aText.Orietation = this.Orietation;
            aText.FontWeight = this.FontWeight;
            aText.Color = this._color;
            return aText;
        }
        #endregion

        public override void PlotInOpenGL()
        {
            if (this.Visible == true)
            {
                if (this.CoordinateSystem != null)
                {
                    GL.glPushMatrix();
                    GL.glMatrixMode(GL.GL_MODELVIEW);
                    MultMatrixd(this.CoordinateSystem);
                }

                WriteString(this.Value, this.RefPoint, this.Color);

                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public override void Transform(double[,] TransformMatrix)
        {
            this.RefPoint.Transform(TransformMatrix);
        }
    }
}
