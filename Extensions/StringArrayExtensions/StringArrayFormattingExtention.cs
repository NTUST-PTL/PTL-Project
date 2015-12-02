using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PTL.Extensions.StringArrayExtensions
{
    public static class StringArrayFormattingExtention
    {
        public enum HorizontalAlignmentTypes
        {
            Default,
            Left,
            Center,
            Right
        }

        public static string[,] AlignString(this string[,] Data, string[] Filler, int[] columnWidth, Font Font, HorizontalAlignmentTypes[] HorizontalAlignments = null)
        {
            int NRow = Data.GetLength(0);
            int NCol = Data.GetLength(1);
            String[,] result = new String[NRow, NCol];

            if (HorizontalAlignments == null)
            {
                HorizontalAlignments = new HorizontalAlignmentTypes[NCol];
                for (int i = 0; i < NCol; i++)
                {
                    HorizontalAlignments[i] = HorizontalAlignmentTypes.Default;
                }
            }

            System.Windows.Forms.Button aButton = new System.Windows.Forms.Button();
            Graphics G = aButton.CreateGraphics();
            System.Drawing.StringFormat sf = System.Drawing.StringFormat.GenericTypographic;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            for (int j = 0; j < NCol; j++)
            {
                SizeF fillerSizeF = G.MeasureString(Filler[j], Font, 0, sf);
                float trueColWidth = fillerSizeF.Width * columnWidth[j];

                for (int i = 0; i < NRow; i++)
                {
                    result[i, j] = Data[i, j];
                    if (HorizontalAlignments[j] == HorizontalAlignmentTypes.Left || HorizontalAlignments[j] == HorizontalAlignmentTypes.Default)
                    {
                        while (G.MeasureString(result[i, j], Font, 0, sf).Width < trueColWidth)
                        {
                            result[i, j] += Filler[j];
                        }
                    }
                    else if (HorizontalAlignments[j] == HorizontalAlignmentTypes.Right)
                    {
                        while (G.MeasureString(result[i, j], Font, 0, sf).Width < trueColWidth)
                        {
                            result[i, j] = Filler[j] + result[i, j];
                        } 
                    }
                    else if (HorizontalAlignments[j] == HorizontalAlignmentTypes.Center)
                    {
                        int k = 0;
                        while (G.MeasureString(result[i, j], Font, 0, sf).Width < trueColWidth)
                        {
                            result[i, j] = k % 2 == 0? Filler[j] + result[i, j] : result[i, j] + Filler[j];
                            k++;
                        }
                    }
                }
            }

            return result;
        }

        public static string[,] AppendToEach(this string[,] Data, string added, bool appendToFront = false)
        {
            int NRow = Data.GetLength(0);
            int NCol = Data.GetLength(1);
            String[,] result = new String[NRow, NCol];

            for (int i = 0; i < NRow; i++)
            {
                for (int j = 0; j < NCol; j++)
                {
                    if (appendToFront)
                    {
                        result[i, j] = added + Data[i, j];
                    }
                    else
                    {
                        result[i, j] = Data[i, j] + added;
                    }
                }
            }
            return result;
        }

        public static string[,] AddLineNumber(this string[,] Data, int StartIndex)
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

        public static string MergeString(this string[,] strArray, string spliter = null)
        {
            int NRow = strArray.GetLength(0);
            int NCol = strArray.GetLength(1);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < NRow; i++)
            {
                for (int j = 0; j < NCol; j++)
                {
                    result.Append(strArray[i, j]);
                    if (!string.IsNullOrEmpty(spliter) && j != NCol - 1)
                    {
                        result.Append(spliter);
                    }
                }
                result.Append("\r\n");
            }
            return result.ToString();
        }
    }
}
