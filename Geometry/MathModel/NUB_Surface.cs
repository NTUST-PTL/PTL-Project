using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;

namespace PTL.Geometry.MathModel
{
    public class NUB_Surface : PTL.Mathematics.ProtectedPTLM, IParametricSurface
    {
        public XYZ4[][] DataPoints;
        public NUB_Curve[] uCurves;
        public NUB_Curve[] vCurves;
        public int LastSegmentIndexU;
        public int LastSegmentIndexV;
        public bool ReverseNormalVectorDirection = false;

        public NUB_Surface(XYZ4[][] dataPoints)
        {
            Calulate(dataPoints);
        }

        public void Calulate(XYZ4[][] dataPoints)
        {
            this.DataPoints = dataPoints;
            this.LastSegmentIndexU = dataPoints[0].Length - 2;
            this.LastSegmentIndexV = dataPoints.Length - 2;

            //計算uCurve
            uCurves = new NUB_Curve[dataPoints.Length];
            for (int i = 0; i < dataPoints.Length; i++)
               uCurves[i] = new NUB_Curve(dataPoints[i]);

            //計算vCurve
            vCurves = new NUB_Curve[uCurves[0].ControlPoints.Length];
            for (int i = 0; i < uCurves[0].ControlPoints.Length; i++)
            {
                XYZ4[] vDatapoints = (from curve in uCurves
                                    select curve.ControlPoints[i]).ToArray();
                vCurves[i] = new NUB_Curve(vDatapoints);
            }
        }

        public Tuple<int, double, int, double> Param_Mapping(double u, double v)
        {
            #region u, v mapping


            double globalU = u * (DataPoints[0].Length - 1);
            double localU = globalU % 1;
            int sIndexU = (int)(globalU > 0 ? System.Math.Floor(globalU) : System.Math.Ceiling(globalU));
            if (sIndexU > LastSegmentIndexU)
            {
                int deltaIndex = sIndexU - LastSegmentIndexU;
                localU += deltaIndex;
                sIndexU = LastSegmentIndexU;
            }
            else if (sIndexU < 0)
            {
                localU += sIndexU;
                sIndexU = 0;
            }


            double globalV = v * (DataPoints.Length - 1);
            double localV = globalV % 1;
            int sIndexV = (int)(globalV > 0 ? System.Math.Floor(globalV) : System.Math.Ceiling(globalV));
            if (sIndexV > LastSegmentIndexV)
            {
                int deltaIndex = sIndexV - LastSegmentIndexV;
                localV += deltaIndex;
                sIndexV = LastSegmentIndexV;
            }
            else if (sIndexV < 0)
            {
                localV += sIndexV;
                sIndexV = 0;
            }


            #endregion u, v mapping

            return new Tuple<int,double,int,double>(sIndexU, localU, sIndexV, localV);
        }

        public XYZ4 R(double u, double v)
        {
            Tuple<int,double,int,double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] uControlPoints = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                uControlPoints[i] = vCurves[sIndexU + i].R(v);
            }

            double[,] Nc = uCurves[sIndexV].Ni[sIndexU];

            XYZ4 p = NUB_Curve.Blending(localU, Nc, uControlPoints);

            return p;
        }

        public XYZ3 dU(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] uControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                uControlPoint[i] = vCurves[sIndexU + i].R(v);
            }

            double[,] Nc = uCurves[sIndexV].Ni[sIndexU];

            XYZ3 p = NUB_Curve.dU_Blending(localU, Nc, uControlPoint);

            return p;
        }

        public XYZ3 dU2(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] uControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                uControlPoint[i] = vCurves[sIndexU + i].R(v);
            }

            double[,] Nc = uCurves[sIndexV].Ni[sIndexU];

            XYZ3 p = NUB_Curve.dU2_Blending(localU, Nc, uControlPoint);

            return p;
        }

        public XYZ3 dV(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] uControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                uControlPoint[i] = vCurves[sIndexU + i].dU(v);
            }

            double[,] Nc = uCurves[sIndexV].Ni[sIndexU];

            XYZ3 p = NUB_Curve.Blending(localU, Nc, uControlPoint);

            return p;
        }

        public XYZ3 dV2(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] uControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                uControlPoint[i] = vCurves[sIndexU + i].dU2(v);
            }

            double[,] Nc = uCurves[sIndexV].Ni[sIndexU];

            XYZ3 p = NUB_Curve.Blending(localU, Nc, uControlPoint);

            return p;
        }

        public XYZ3 dUdV(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] uControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                uControlPoint[i] = vCurves[sIndexU + i].dU(v);
            }

            double[,] Nc = uCurves[sIndexV].Ni[sIndexU];

            XYZ3 p = NUB_Curve.dU_Blending(localU, Nc, uControlPoint);

            return p;
        }

        public XYZ3 N(double u, double v)
        {
            XYZ3 normal;
            if (ReverseNormalVectorDirection)
                normal = Cross(dU(u, v), dV(u, v));
            else
                normal = Cross(dV(u, v), dU(u, v));
            return normal;
        }

        public XYZ3 n(double u, double v)
        {
            XYZ3 normal = N(u, v);
            return normal / Norm(normal);
        }
    }
}
