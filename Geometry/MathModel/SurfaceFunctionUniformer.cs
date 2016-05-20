using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Geometry.MathModel
{
    public class SurfaceFunctionUniformer
    {
        public static Tuple<XYZ4[][], object[][][]> GetUniformedPoints(
              Func<double, double, XYZ4> surfaceFunc
            , Func<XYZ4, double[]> targetFunc
            , double start1
            , double end1
            , int pointNum1
            , double start2
            , double end2
            , int pointNum2
            , params Func<double, double, object>[] getExtraSurfaceData
            )
        {
            double[] target00 = targetFunc(surfaceFunc(start1, start2));
            double[] target01 = targetFunc(surfaceFunc(start1, end2));
            double[] target10 = targetFunc(surfaceFunc(end1, start2));
            double[] target11 = targetFunc(surfaceFunc(end1, end2));

            XYZ4[][] uniformedPoints = new XYZ4[pointNum2][];
            object[][][] extraData = null;
            if (getExtraSurfaceData != null)
            {
                extraData = new object[getExtraSurfaceData.Length][][];
                for (int i = 0; i < getExtraSurfaceData.Length; i++)
                {
                    extraData[i] = new object[pointNum2][];
                    for (int j = 0; j < pointNum2; j++)
                    {
                        extraData[i][j] = new object[pointNum1];
                    }
                }
            }
            for (int j = 0; j < pointNum2; j++)
            {
                uniformedPoints[j] = new XYZ4[pointNum1];
                for (int i = 0; i < pointNum1; i++)
                {
                    double[] targetU0 = Add(target00, Mult(Mult(Substract(target10, target00), 1.0 / (pointNum1 - 1)), i));
                    double[] targetU1 = Add(target01, Mult(Mult(Substract(target11, target01), 1.0 / (pointNum1 - 1)), i));
                    double[] target = Add(targetU0, Mult(Mult(Substract(targetU1, targetU0), 1.0 / (pointNum2 - 1)), j));

                    Func<double[], double[]> func =
                        (paras) =>
                        {
                            double u = paras[0];
                            double v = paras[1];
                            XYZ4 p = surfaceFunc(u, v);
                            double[] value = targetFunc(p);
                            return Substract(value, target);
                        };
                    double[] guess =
                        new double[] {
                            start1 + i * ((end1 - start1) / (pointNum1 - 1)),
                            start2 + j * ((end2 - start2) / (pointNum2 - 1)) };
                    double[] ans = MathNet.Numerics.RootFinding.Broyden.FindRoot(func, guess, 1e-8, 100);

                    uniformedPoints[j][i] = surfaceFunc(ans[0], ans[1]);

                    if (getExtraSurfaceData != null)
                        for (int k = 0; k < getExtraSurfaceData.Length; i++)
                        {
                            extraData[k][j][i] = getExtraSurfaceData[k](ans[0], ans[1]);
                        }
                }
                
            }
            return new Tuple<XYZ4[][], object[][][]>(uniformedPoints, extraData);
        }

        public static NUB_Surface GetUniformedNUBSurface(
             Func<double, double, XYZ4> surfaceFunc
            , Func<XYZ4, double[]> targetFunc
            , double start1
            , double end1
            , int pointNum1
            , double start2
            , double end2
            , int pointNum2
            )
        {
            XYZ4[][] dataPoints = GetUniformedPoints(
                  surfaceFunc
                , targetFunc
                , start1
                , end1
                , pointNum1
                , start2
                , end2
                , pointNum2
                ).Item1;

            var nubs = new NUB_Surface(dataPoints);
            return nubs;
        }

        public static NUB_Surface GetUniformedNUBSurface(
             Func<double, double, XYZ4> surfaceFunc
            , Func<double, double, XYZ4> surfaceVTangentFunc
            , Func<XYZ4, double[]> targetFunc
            , double start1
            , double end1
            , int pointNum1
            , double start2
            , double end2
            , int pointNum2
            )
        {
             var results = GetUniformedPoints(
                  surfaceFunc
                , targetFunc
                , start1
                , end1
                , pointNum1
                , start2
                , end2
                , pointNum2
                , (u, v) => surfaceVTangentFunc(u, v)
                );
            XYZ4[][] dataPoints = results.Item1;
            object[][] tv2D = results.Item2;
            XYZ3[][] tv = (from ts in tv2D select new XYZ3[] { (XYZ3)ts.First(), (XYZ3)ts.Last() }).ToArray();
            var nubs = new NUB_Surface(dataPoints, tv);
            return nubs;
        }
    }
}
