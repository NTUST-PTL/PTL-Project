using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Data;

namespace PTL.Geometry.MathModel
{
    public class SurfaceDefinitions : ITag
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
        public Func<double, double, double[]> N { get; set; }
        /// <summary>
        /// 標籤
        /// </summary>
        public object Tag { get; set; }

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
                    tf.Normals[i, j] = dV(u, v);
                }
            }
            topoFaceSetter?.Invoke(tf);
            return tf;
        }

        public SurfaceDefinitions GetTransformed(double[,] matrix4)
        {
            SurfaceDefinitions newSurf = new SurfaceDefinitions();
            newSurf.Tag = Tag + "-Transported";
            newSurf.P = (u, v) =>
            {
                XYZ4 p = P(u, v);
                p.Transform(matrix4);
                return p;
            };
            newSurf.dU = (u, v) =>
            {
                XYZ3 du = dU(u, v);
                du.Transform(matrix4);
                return du;
            };
            newSurf.dV = (u, v) =>
            {
                XYZ3 dv = dV(u, v);
                dv.Transform(matrix4);
                return dv;
            };
            newSurf.N = (u, v) =>
            {
                XYZ3 n = N(u, v);
                n.Transform(matrix4);
                return n;
            };

            return newSurf;
        }
    }
}
