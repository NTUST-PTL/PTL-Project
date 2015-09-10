using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PTL.FileOperation
{
    public static class StringFormat
    {
        public static string StringStr(string value)
        {
            string str = String.Format("{0,16}", value);
            return str;
        }
        public static string StringDouble(double value)
        {
            string str = String.Format("{0,16}", String.Format("{0:0.000}", value));
            return str;
        }
        public static string StringDouble(double value,int tdecimal,int space)
        {
            string strdecimal = StringDecimalFormate(tdecimal);
            string strspace = "{0," + Convert.ToString(space) + "}";

            string str = String.Format(strspace, String.Format("{0:0.000}", value));
            return str;
        }
        public static string StringDouble(double value, int tdecimal)
        {
            string strdecimal = StringDecimalFormate(tdecimal);

            string str = String.Format("{0,16}", String.Format(strdecimal, value));
            return str;
        }
        public static string StringInt(int value)
        {
            string str = String.Format("{0,16}", String.Format("{0:0}", value));
            return str;
        }

        public static string StringDecimalFormate(int tdecimal)
        {
            string strdecimal = "{0:0.";
            for (short k = 0; k < tdecimal; k++)
                strdecimal = strdecimal + "0";

            strdecimal = strdecimal + "}";

            return strdecimal;
        }

        public static String[,] AlignString(String[,] Data, String[] Filler,int[] columnSpace , Font Font)
        {
            int NRow = Data.GetLength(0);
            int NCol = Data.GetLength(1);
            String[,] result = new String[NRow, NCol];

            System.Windows.Forms.Button aButton = new System.Windows.Forms.Button();
            Graphics G = aButton.CreateGraphics();
            System.Drawing.StringFormat sf = System.Drawing.StringFormat.GenericTypographic;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            for (int j = 0; j < NCol; j++)
            {
                //計算此欄字串的最大長度
                SizeF maxSize = G.MeasureString(Data[0, j], Font, 0, sf);
                for (int i = 1; i < NRow; i++)
                {
                    SizeF sizeF = G.MeasureString(Data[i, j], Font, 0, sf);
                    if (sizeF.Width > maxSize.Width)
                        maxSize = sizeF;
                }

                SizeF fillerSizeF = G.MeasureString(Filler[j], Font, 0, sf);
                SizeF ColSize = new SizeF(maxSize.Width + fillerSizeF.Width * columnSpace[j], maxSize.Height);

                for (int i = 0; i < NRow; i++)
                {
                    result[i, j] = Data[i, j];
                    while (G.MeasureString(result[i, j], Font, 0, sf).Width < ColSize.Width)
                        result[i, j] += Filler[j];
                }
            }

            return result;
        }

        public static String[,] AddLineNumber(String[,] Data, int StartIndex)
        {
            int NRow = Data.GetLength(0);
            int NCol = Data.GetLength(1);
            String[,] result = new String[NRow, NCol + 1];

            for (int i = 0; i < NRow; i++)
            {
                result[i, 0] = (StartIndex + i).ToString();
                for (int j = 0; j < NCol; j++)
                    result[i, j + 1] = Data[i, j];
            }

            return result;
        }
    }
}
