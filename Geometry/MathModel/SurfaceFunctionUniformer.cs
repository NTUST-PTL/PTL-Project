using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.DebugTools;
using PTL.Mathematics;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Geometry.MathModel
{
    public class SurfaceFunctionUniformer
    {
        public static Tuple<XYZ4[,], object[][,]> GetUniformedPoints(
              Func<double, double, XYZ4> surfaceFunc
            , Func<XYZ4, double[]> targetFunc
            , double[,][] targetTopo
            , double[] initialGuess
            , params Func<double, double, object>[] getExtraSurfaceData
            )
        {
            int nRow = targetTopo.GetLength(0);
            int nCol = targetTopo.GetLength(1);

            XYZ4[,] uniformedPoints = new XYZ4[nRow, nCol];

            object[][,] extraData = null;
            if (getExtraSurfaceData != null)
            {
                extraData = new object[getExtraSurfaceData.Length][,];
                for (int i = 0; i < getExtraSurfaceData.Length; i++)
                {
                    extraData[i] = new object[nRow,nCol];
                }
            }
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nCol; j++)
                {
                    Vector target = targetTopo[i, j];

                    Func<double[], double[]> func =
                        (paras) =>
                        {
                            double u = paras[0];
                            double v = paras[1];
                            XYZ4 p = surfaceFunc(u, v);
                            Vector value = targetFunc(p);
                            Vector error = value - target;
                            return error;
                        };
                    double[] ans;
                    try
                    {
                        ans = MathNet.Numerics.RootFinding.Broyden.FindRoot(func, initialGuess, 1e-8, 100);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    ans = MathNet.Numerics.RootFinding.Broyden.FindRoot(func, initialGuess, 1e-8, 100);

                    uniformedPoints[i,j] = surfaceFunc(ans[0], ans[1]);

                    if (getExtraSurfaceData != null)
                        for (int k = 0; k < getExtraSurfaceData.Length; k++)
                        {
                            extraData[k][i,j] = getExtraSurfaceData[k](ans[0], ans[1]);
                        }
                }

            }
            return new Tuple<XYZ4[,], object[][,]>(uniformedPoints, extraData);
        }

        public static Tuple<XYZ4[,], object[][,]> GetUniformedPoints(
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
            Vector target00 = targetFunc(surfaceFunc(start1, start2));
            Vector target01 = targetFunc(surfaceFunc(start1, end2));
            Vector target10 = targetFunc(surfaceFunc(end1, start2));
            Vector target11 = targetFunc(surfaceFunc(end1, end2));

            XYZ4[,] uniformedPoints = new XYZ4[pointNum1, pointNum2];
            
            object[][,] extraData = null;
            if (getExtraSurfaceData != null)
            {
                extraData = new object[getExtraSurfaceData.Length][,];
                for (int i = 0; i < getExtraSurfaceData.Length; i++)
                {
                    extraData[i] = new object[pointNum1, pointNum2];
                }
            }
            for (int i = 0; i < pointNum1; i++)
            {
                for (int j = 0; j < pointNum2; j++)
                {
                    Vector targetU0 = target00 + (target10 - target00) / (pointNum1 - 1) * i;
                    Vector targetU1 = target01 + (target11 - target01) / (pointNum1 - 1) * i;
                    Vector target = targetU0 + (targetU1 - targetU0) / (pointNum2 - 1) * j;

                    Func<double[], double[]> func =
                        (paras) =>
                        {
                            double u = paras[0];
                            double v = paras[1];
                            XYZ4 p = surfaceFunc(u, v);
                            Vector value = targetFunc(p);
                            Vector error = value - target;
                            return error;
                        };
                    double[] guess =
                        new double[] {
                            start1 + i * ((end1 - start1) / (pointNum1 - 1)),
                            start2 + j * ((end2 - start2) / (pointNum2 - 1)) };
                    double[] ans = MathNet.Numerics.RootFinding.Broyden.FindRoot(func, guess, 1e-8, 100);

                    uniformedPoints[i,j] = surfaceFunc(ans[0], ans[1]);

                    if (getExtraSurfaceData != null)
                        for (int k = 0; k < getExtraSurfaceData.Length; k++)
                        {
                            extraData[k][i,j] = getExtraSurfaceData[k](ans[0], ans[1]);
                        }
                }
                
            }
            return new Tuple<XYZ4[,], object[][,]>(uniformedPoints, extraData);
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
            XYZ4[,] dataPoints = GetUniformedPoints(
                  surfaceFunc
                , targetFunc
                , start1
                , end1
                , pointNum1
                , start2
                , end2
                , pointNum2
                ).Item1;

            var nubs = new NUB_Surface(ToIrregularArray(dataPoints));
            return nubs;
        }

        public static NUB_Surface GetUniformedNUBSurface(
             Func<double, double, XYZ4> surfaceFunc
            , Func<double, double, XYZ3> surfaceTangentFunc
            , NUB_Surface.GivenTangentsDirections givenTangentsDirection
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
                , (u, v) => surfaceTangentFunc(u, v)
                );
            XYZ4[][] dataPoints = ToIrregularArray(results.Item1);
            object[][] tv2D = givenTangentsDirection == NUB_Surface.GivenTangentsDirections.VDirection ?
                ToIrregularArray(results.Item2[0])
                : ToIrregularArray(Transpose(results.Item2[0]));
            XYZ3[][] tv = (from ts in tv2D select new XYZ3[] { (XYZ3)ts.First(), (XYZ3)ts.Last() }).ToArray();
            var nubs = new NUB_Surface(dataPoints, tv, givenTangentsDirection);
            return nubs;
        }

        public static NUB_Surface GetUniformedNUBSurface(
             Func<double, double, XYZ4> surfaceFunc
            , Func<XYZ4, double[]> targetFunc
            , double[,][] targetTopo
            , double[] initialGuess
            )
        {
            XYZ4[,] dataPoints = GetUniformedPoints(
                  surfaceFunc
                , targetFunc
                , targetTopo
                , initialGuess
                ).Item1;

            //Plot plot = new Plot();
            //plot.Window.View.AutoScale = false;

            //int nRow = dataPoints.GetLength(0);
            //int nCol = dataPoints.GetLength(1);

            //for (int i = 0; i < nRow; i++)
            //{
            //    for (int j = 0; j < nCol; j++)
            //    {
            //        plot.AddSomethings(new PointD(dataPoints[i, j]));
            //    }
            //}

            var nubs = new NUB_Surface(ToIrregularArray(dataPoints));
            return nubs;
        }

        public static NUB_Surface GetUniformedNUBSurface(
            Func<double, double, XYZ4> surfaceFunc
            , Func<double, double, XYZ3> surfaceTangentFunc
            , NUB_Surface.GivenTangentsDirections givenTangentsDirection
            , Func<XYZ4, double[]> targetFunc
            , double[,][] targetTopo
            , double[] initialGuess
            )
        {
            var results = GetUniformedPoints(
                 surfaceFunc
               , targetFunc
               , targetTopo
               , initialGuess
               , (u, v) => surfaceTangentFunc(u, v)
               );

            XYZ4[][] dataPoints = ToIrregularArray(results.Item1);

            object[][] tv2D = givenTangentsDirection == NUB_Surface.GivenTangentsDirections.VDirection ?
                ToIrregularArray(results.Item2[0])
                : ToIrregularArray(Transpose(results.Item2[0]));
            XYZ3[][] tv = (from ts in tv2D select new XYZ3[] { (XYZ3)ts.First(), (XYZ3)ts.Last() }).ToArray();

            var nubs = new NUB_Surface(dataPoints, tv, givenTangentsDirection);
            return nubs;
        }
    }
}
