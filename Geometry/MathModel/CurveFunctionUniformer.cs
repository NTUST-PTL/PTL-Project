using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry.MathModel
{
    public class CurveFunctionUniformer
    {
        public static Tuple<XYZ4[], object[][]> GetUniformedPoints(
             Func<double, XYZ4> curveFunc
            ,Func<XYZ4, double> targetFunc
            ,double start
            ,double end
            ,int pointNum
            ,params Func<double, object>[] getExtraCurveData)
        {
            double targetStart = targetFunc(curveFunc(start));
            double targetEnd = targetFunc(curveFunc(end));

            XYZ4[] uniformedPoints = new XYZ4[pointNum];
            object[][] extraDatas = getExtraCurveData != null ? new object[getExtraCurveData.Length][] : null;
            if (getExtraCurveData != null)
                for (int i = 0; i < getExtraCurveData.Length; i++)
                    extraDatas[i] = new object[pointNum];

            for (int i = 0; i < pointNum; i++)
            {
                double target = targetStart + i * (targetEnd - targetStart) / (pointNum - 1);

                Func<double, double> func =
                    (u) =>
                    {
                        XYZ4 p = curveFunc(u);
                        double value = targetFunc(p);
                        return value - target;
                    };
                double ans = MathNet.Numerics.RootFinding.Brent.FindRoot(func, start, end);

                uniformedPoints[i] = curveFunc(ans);

                if (getExtraCurveData != null)
                    for (int j = 0; j < getExtraCurveData.Length; j++)
                    {
                        extraDatas[j][i] = getExtraCurveData[j](ans);
                    }
            }

            return new Tuple<XYZ4[], object[][]>(uniformedPoints, extraDatas);
        }

        public static NUB_Curve GetUniformedNUBCurve(
              Func<double, XYZ4> curveFunc
            , Func<XYZ4, double> targetFunc
            , double start
            , double end
            , int pointNum)
        {
            XYZ4[] uniformedPoints = GetUniformedPoints(
                  curveFunc
                , targetFunc
                , start
                , end
                , pointNum
                ).Item1;


            NUB_Curve uniformCurve = new NUB_Curve(uniformedPoints);

            return uniformCurve;
        }

        public static NUB_Curve GetUniformedNUBCurve(
              Func<double, XYZ4> curveFunc
            , Func<double, XYZ4> curveTangentFunc
            , Func<XYZ4, double> targetFunc
            , double start
            , double end
            , int pointNum)
        {
            var result = GetUniformedPoints(
                  curveFunc
                , targetFunc
                , start
                , end
                , pointNum
                , curveTangentFunc);
            XYZ4[] uniformedPoints = result.Item1;
            XYZ3[] tengents = (from t in result.Item2[0] select (XYZ3)t).ToArray();

            NUB_Curve uniformCurve = new NUB_Curve(uniformedPoints, tengents.First(), tengents.Last());

            return uniformCurve;
        }
    }
}
