using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry;
using PTL.Extensions.StringArrayExtensions;

namespace PTL.Extensions.TopoFaceOutputExtensions
{
    public static class TopoFaceStringOuputExtension
    {
        public static String DataOutputToString(this TopoFace tf)
        {
            int nRow = tf.Dim1Length;
            int nCol = tf.Dim2Length;

            string[,] header = new string[,] { { "i", "j", "    X", "    Y", "    Z", "    NX", "     NY", "    NZ" } };
            header = header.AlignString(
                new string[] { " ", " ", " ", " ", " ", " ", " ", " " },
                new int[] { 5, 3, 12, 12, 12, 12, 12, 12 },
                new System.Drawing.Font("MingLiU", 12),
                new StringArrayFormattingExtention.HorizontalAlignmentTypes[]
                {
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Center,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Center,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Center,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Center,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Center,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Center
                });

            string[,] datas = new string[nRow * nCol, 8];
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nCol; j++)
                {
                    int index = i * nRow + j;
                    datas[index, 0] = (i + 1).ToString();
                    datas[index, 1] = (j + 1).ToString();
                    datas[index, 2] = tf.Points[i, j].X.ToString("F4");
                    datas[index, 3] = tf.Points[i, j].Y.ToString("F4");
                    datas[index, 4] = tf.Points[i, j].Z.ToString("F4");
                    datas[index, 5] = tf.Normals[i, j].X.ToString("F4");
                    datas[index, 6] = tf.Normals[i, j].Y.ToString("F4");
                    datas[index, 7] = tf.Normals[i, j].Z.ToString("F4");
                }
            }
            datas = datas.AlignString(
                new string[] { " ", " ", " ", " ", " ", " ", " ", " " },
                new int[] { 5, 3, 12, 12, 12, 12, 12, 12 },
                new System.Drawing.Font("MingLiU", 12),
                new StringArrayFormattingExtention.HorizontalAlignmentTypes[]
                {
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right,
                    StringArrayFormattingExtention.HorizontalAlignmentTypes.Right
                });

            string[,] total = (String[,])PTL.Mathematics.PTLM.ArratJoin(header, datas);
            String output = total.MergeString();
            return output;
        }
    }
}
