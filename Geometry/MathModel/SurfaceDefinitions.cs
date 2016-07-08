using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry.MathModel
{
    public class SurfaceDefinitions
    {
        /// <summary>
        /// 位置方程式
        /// </summary>
        public Func<double, double, double[]> P { get; set; }
        /// <summary>
        /// 切線向量1
        /// </summary>
        public Func<double, double, double[]> dU { get; set; }
        /// <summary>
        /// 切線向量2
        /// </summary>
        public Func<double, double, double[]> dV { get; set; }
        /// <summary>
        /// 單位法向量
        /// </summary>
        public Func<double, double, double[]> n { get; set; }

        public TopoFace ToTopoFace(double u0 = 0, double u1  =1, int nRow = 21, double v0 = 0, double v1 = 1, int nCol = 21, Action<TopoFace> topoFaceSetter = null)
        {
            TopoFace tf = new TopoFace(nRow, nCol);
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nCol; j++)
                {
                    double u = u0 + (u1 - u0) / (nRow - 1) * i;
                    double v = v0 + (v1 - v0) / (nCol - 1) * j;
                    tf.Points[i, j] = P(u, v);
                    tf.Normals[i, j] = n(u, v);
                }
            }
            topoFaceSetter?.Invoke(tf);
            return tf;
        }
    }
}
