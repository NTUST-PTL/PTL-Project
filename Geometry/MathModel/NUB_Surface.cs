using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BaseFunctions;

namespace PTL.Geometry.MathModel
{
    public class NUB_Surface : IParametricSurface
    {
        public XYZ4[][] DataPoints;
        public NUB_Curve[] UCurves;
        public NUB_Curve[] VCurves;
        public int LastSegmentIndexU;
        public int LastSegmentIndexV;
        public bool ReverseNormalVectorDirection = false;

        public NUB_Surface(XYZ4[][] dataPoints, XYZ3[][] VTangents = null)
        {
            Calulate(dataPoints, VTangents);
        }

        public NUB_Surface(NUB_Curve[] UCurves)
        {
            Calulate(UCurves);
        }

        public void Calulate(XYZ4[][] dataPoints, XYZ3[][] VTangents = null)
        {
            this.DataPoints = dataPoints;
            this.LastSegmentIndexU = dataPoints[0].Length - 2;
            this.LastSegmentIndexV = dataPoints.Length - 2;

            //計算vCurve
            VCurves = new NUB_Curve[dataPoints.Length];
            for (int i = 0; i < dataPoints.Length; i++)
                VCurves[i] = new NUB_Curve(dataPoints[i], VTangents[i][0], VTangents[i][1]);

            //計算uCurve
            UCurves = new NUB_Curve[VCurves[0].ControlPoints.Length];
            for (int i = 0; i < VCurves[0].ControlPoints.Length; i++)
            {
                XYZ4[] vDatapoints = (from curve in VCurves
                                    select curve.ControlPoints[i]).ToArray();
                UCurves[i] = new NUB_Curve(vDatapoints);
            }
        }

        public void Calulate(NUB_Curve[] vCurves)
        {
            this.DataPoints = (from curve in vCurves select curve.DataPoints).ToArray();
            this.LastSegmentIndexU = vCurves[0].LastSegmentIndex;
            this.LastSegmentIndexV = vCurves.Length - 2;

            VCurves = vCurves;
            //計算vCurve
            UCurves = new NUB_Curve[vCurves[0].ControlPoints.Length];
            for (int i = 0; i < vCurves[0].ControlPoints.Length; i++)
            {
                XYZ4[] vDatapoints = (from curve in vCurves
                                      select curve.ControlPoints[i]).ToArray();
                UCurves[i] = new NUB_Curve(vDatapoints);
            }
        }

        public Tuple<int, double, int, double> Param_Mapping(double u, double v)
        {
            Tuple<int,double> uMapping = UCurves[0].Param_Mapping(u);
            Tuple<int, double> vMapping = VCurves[0].Param_Mapping(v);
            
            return new Tuple<int, double, int, double>(uMapping.Item1, uMapping.Item2, vMapping.Item1, vMapping.Item2);
        }

        public XYZ4 P(double u, double v)
        {
            Tuple<int,double,int,double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] vControlPoints = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                vControlPoints[i] = UCurves[sIndexV + i].P(u);
            }

            double[,] Nc = VCurves[sIndexU].Ni[sIndexV];

            XYZ4 p = NUB_Curve.Blending(localV, Nc, vControlPoints);

            return p;
        }

        public XYZ3 dU(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] vControlPoints = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                vControlPoints[i] = UCurves[sIndexV + i].P(u);
            }

            double[,] Nc = VCurves[sIndexU].Ni[sIndexV];

            XYZ3 p = NUB_Curve.dU_Blending(localU, Nc, vControlPoints);

            return p;
        }

        public XYZ3 dU2(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] vControlPoints = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                vControlPoints[i] = UCurves[sIndexV + i].P(u);
            }

            double[,] Nc = VCurves[sIndexU].Ni[sIndexV];

            XYZ3 p = NUB_Curve.dU2_Blending(localU, Nc, vControlPoints);

            return p;
        }

        public XYZ3 dV(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] vControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                vControlPoint[i] = UCurves[sIndexV + i].dU(u);
            }

            double[,] Nc = VCurves[sIndexU].Ni[sIndexV];

            XYZ3 p = NUB_Curve.Blending(localV, Nc, vControlPoint);

            return p;
        }

        public XYZ3 dV2(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] vControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                vControlPoint[i] = UCurves[sIndexV + i].dU2(u);
            }

            double[,] Nc = VCurves[sIndexU].Ni[sIndexV];

            XYZ3 p = NUB_Curve.Blending(localV, Nc, vControlPoint);

            return p;
        }

        public XYZ3 dUdV(double u, double v)
        {
            Tuple<int, double, int, double> mappedPara = Param_Mapping(u, v);
            int sIndexU = mappedPara.Item1;
            double localU = mappedPara.Item2;
            int sIndexV = mappedPara.Item3;
            double localV = mappedPara.Item4;

            XYZ4[] vControlPoint = new XYZ4[4];
            for (int i = 0; i < 4; i++)
            {
                vControlPoint[i] = UCurves[sIndexV + i].dU(u);
            }

            double[,] Nc = VCurves[sIndexU].Ni[sIndexV];

            XYZ3 p = NUB_Curve.dU_Blending(localV, Nc, vControlPoint);

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
