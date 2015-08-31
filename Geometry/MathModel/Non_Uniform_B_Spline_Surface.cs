﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;

namespace PTL.Geometry.MathModel
{
    public class Non_Uniform_B_Spline_Surface
    {
        public XYZ[][] DataPoints;
        public Non_Uniform_B_Spline_Curve[] uCurve;
        public Non_Uniform_B_Spline_Curve[] vCurve;
        public int LastSegmentIndexU;
        public int LastSegmentIndexV;

        public Non_Uniform_B_Spline_Surface(XYZ[][] dataPoints)
        {
            Calulate(dataPoints);
        }

        public void Calulate(XYZ[][] dataPoints)
        {
            this.DataPoints = dataPoints;
            this.LastSegmentIndexU = dataPoints[0].Length - 2;
            this.LastSegmentIndexV = dataPoints.Length - 2;

            //計算uCurve
            uCurve = new Non_Uniform_B_Spline_Curve[dataPoints.Length];
            for (int i = 0; i < dataPoints.Length; i++)
               uCurve[i] = new Non_Uniform_B_Spline_Curve(dataPoints[i]);

            //計算vCurve
            vCurve = new Non_Uniform_B_Spline_Curve[uCurve[0].ControlPoints.Length];
            for (int i = 0; i < uCurve[0].ControlPoints.Length; i++)
            {
                XYZ[] vDatapoints = (from curve in uCurve
                                    select curve.ControlPoints[i]).ToArray();
                vCurve[i] = new Non_Uniform_B_Spline_Curve(vDatapoints);
            }
        }

        public XYZ SurfaceFunc(double u, double v)
        {
            //u = u >= 1 ? 0.999999999999999 : u;
            //u = u < 0 ? 0 : u;
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
            //v = v >= 1 ? 0.999999999999999 : v;
            //v = v < 0 ? 0 : v;
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


            XYZ[] uControlPoint = new XYZ[4];
            for (int i = 0; i < 4; i++)
            {
                uControlPoint[i] = vCurve[sIndexU + i].CurveFunc(v);
            }

            double[,] Nc = uCurve[sIndexV].Ni[sIndexU];

            XYZ p = Non_Uniform_B_Spline_Curve.Blending(uControlPoint, Nc, localU);

            return p;
        }
    }
}
