using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry.MathModel
{
    public class NUB_Curve
    {
        public XYZ4[] DataPoints;
        public XYZ4[] ControlPoints;
        public double[][,] Ni;
        public double[] Delta_i;
        public int LastSegmentIndex;

        public NUB_Curve(XYZ4[]dataPoints, bool closed = false)
        {
            Calculate_NURBS_Curve(dataPoints, closed);
        }

        public NUB_Curve(XYZ4[] dataPoints, XYZ3 startTangent, XYZ3 endTangent)
        {
            Calculate_NURBS_Curve(dataPoints, startTangent, endTangent);
        }

        public void Calculate_NURBS_Curve(XYZ4[] dataPoints, bool closed = false)
        {
            #region 計算R矩陣
            int n = dataPoints.Length;
            XYZ3 T1, T2;
            if (n == 2)
            {
                T1 = (dataPoints[1] - dataPoints[0]) / 3.0;
                T2 = (dataPoints[0] - dataPoints[1]) / 3.0;
            }
            else if (closed)
            {
                //XYZ4 a0 = dataPoints[1] - dataPoints[0];
                //XYZ4 b0 = dataPoints[n - 2] - dataPoints[0];
                //XYZ4 c0 = Cross(a0, b0);
                //XYZ4 r0 = (Norm(a0) * Norm(a0) * (XYZ4)Cross(b0, c0) + Norm(b0) * Norm(b0) * (XYZ4)Cross(c0, a0)) / (2 * GetLength(c0) * GetLength(c0));
                //T1 = GetLength(a0) * (XYZ4)Cross(r0, c0) / GetLength(Cross(r0, c0));
                //T2 = -1 * T1;
                T1 = GetCircularTangent(dataPoints[n - 2], dataPoints[0], dataPoints[1], GetCircularTangentAt.Middle);
                T2 = -1 * T1;
            }
            else
            {
                //XYZ4 a0 = dataPoints[1] - dataPoints[0];
                //XYZ4 b0 = dataPoints[2] - dataPoints[0];
                //XYZ4 c0 = Cross(a0, b0);
                //XYZ4 r0 = (Norm(a0) * Norm(a0) * (XYZ4)Cross(b0, c0) + Norm(b0) * Norm(b0) * (XYZ4)Cross(c0, a0)) / (2 * GetLength(c0) * GetLength(c0));
                //T1 = GetLength(a0) * (XYZ4)Cross(r0, c0) / GetLength(Cross(r0, c0));
                //XYZ4 an = dataPoints[n - 2] - dataPoints[n - 1];
                //XYZ4 bn = dataPoints[n - 3] - dataPoints[n - 1];
                //XYZ4 cn = Cross(an, bn);
                //XYZ4 rn = (GetLength(an) * GetLength(an) * (XYZ4)Cross(bn, cn) + GetLength(bn) * GetLength(bn) * (XYZ4)Cross(cn, an)) / (2 * GetLength(cn) * GetLength(cn));
                //T2 = -1.0 * GetLength(an) * (XYZ4)Cross(rn, cn) / GetLength(Cross(rn, cn));
                T1 = GetCircularTangent(dataPoints[0], dataPoints[1], dataPoints[2], GetCircularTangentAt.Start);
                T2 = GetCircularTangent(dataPoints[n - 3], dataPoints[n - 2], dataPoints[n - 1], GetCircularTangentAt.End);
            }
            #endregion
            Calculate_NURBS_Curve(dataPoints, T1, T2);
        }

        public enum GetCircularTangentAt
        {
            Start,
            Middle,
            End
        };
        public static XYZ3 GetCircularTangent(XYZ4 p1, XYZ4 p2, XYZ4 p3, GetCircularTangentAt at)
        {
            switch (at)
            {
                case GetCircularTangentAt.Start:
                    {
                        XYZ4 a0 = p2 - p1;
                        XYZ4 b0 = p3 - p1;
                        XYZ4 c0 = Cross(a0, b0);
                        if (Norm(c0) < 1e-12)
                            return a0;
                        XYZ4 r0 = (Norm(a0) * Norm(a0) * (XYZ4)Cross(b0, c0) + Norm(b0) * Norm(b0) * (XYZ4)Cross(c0, a0)) / (2 * GetLength(c0) * GetLength(c0));
                        XYZ3 T1 = GetLength(a0) * (XYZ4)Cross(r0, c0) / GetLength(Cross(r0, c0));
                        return T1;
                    }
                case GetCircularTangentAt.Middle:
                    {
                        XYZ4 a0 = p3 - p2;
                        XYZ4 b0 = p1 - p2;
                        XYZ4 c0 = Cross(a0, b0);
                        if (Norm(c0) < 1e-12)
                            return a0;
                        XYZ4 r0 = (Norm(a0) * Norm(a0) * (XYZ4)Cross(b0, c0) + Norm(b0) * Norm(b0) * (XYZ4)Cross(c0, a0)) / (2 * GetLength(c0) * GetLength(c0));
                        XYZ3 T1 = GetLength(a0) * (XYZ4)Cross(r0, c0) / GetLength(Cross(r0, c0));
                        return T1;
                    }
                case GetCircularTangentAt.End:
                    {
                        XYZ4 an = p2 - p3;
                        XYZ4 bn = p1 - p3;
                        XYZ4 cn = Cross(an, bn);
                        if (Norm(cn) < 1e-12)
                            return -1.0 * an;
                        XYZ4 rn = (GetLength(an) * GetLength(an) * (XYZ4)Cross(bn, cn) + GetLength(bn) * GetLength(bn) * (XYZ4)Cross(cn, an)) / (2 * GetLength(cn) * GetLength(cn));
                        XYZ3 T3 = -1.0 * GetLength(an) * (XYZ4)Cross(rn, cn) / GetLength(Cross(rn, cn));
                        return T3;
                    }
                default:
                    {
                        XYZ4 a0 = p2 - p1;
                        XYZ4 b0 = p3 - p1;
                        XYZ4 c0 = Cross(a0, b0);
                        if (Norm(c0) < 1e-12)
                            return a0;
                        XYZ4 r0 = (Norm(a0) * Norm(a0) * (XYZ4)Cross(b0, c0) + Norm(b0) * Norm(b0) * (XYZ4)Cross(c0, a0)) / (2 * GetLength(c0) * GetLength(c0));
                        XYZ3 T1 = GetLength(a0) * (XYZ4)Cross(r0, c0) / GetLength(Cross(r0, c0));
                        return T1;
                    }
            }
        }

        public void Calculate_NURBS_Curve(XYZ4[] dataPoints, XYZ3 startTangent, XYZ3 endTangent)
        {
            DataPoints = dataPoints;
            int n = DataPoints.Length;
            this.LastSegmentIndex = n - 2;
            double[,] M = NewZeroMatrix(n + 2, n + 2);
            
            double[,] invM;

            #region 計算弦長
            Delta_i = new double[n + 3];
            Delta_i[0] = 0;
            Delta_i[1] = 0;
            Delta_i[n + 1] = 0;
            Delta_i[n + 1 + 1] = 0;
            for (int i = 0; i < n - 1; i++)
            {
                Delta_i[i + 2] = Norm((DataPoints[i + 1] - DataPoints[i]).Values);
            }
            #endregion

            #region 計算M矩陣
            M[0, 0] = -3;
            M[0, 1] = 3;
            M[1, 0] = 1;
            M[n, n + 1] = 1;
            M[n + 1, n] = -3;
            M[n + 1, n + 1] = 3;
            for (int i = 1; i < n - 1; i++)
            {
                int i_d = i + 2;
                int i_M = i + 1;
                //fi
                M[i_M, i] = Pow(Delta_i[i_d], 2)
                            / (Delta_i[i_d - 1] + Delta_i[i_d])
                            / (Delta_i[i_d - 2] + Delta_i[i_d - 1] + Delta_i[i_d]);
                //gi
                M[i_M, i + 2] = Pow(Delta_i[i_d - 1], 2)
                                / (Delta_i[i_d - 1] + Delta_i[i_d] + Delta_i[i_d + 1])
                                / (Delta_i[i_d - 1] + Delta_i[i_d]);
                //hi
                M[i_M, i + 1] = 1 - M[i_M, i] - M[i_M, i + 2];
            }
            #endregion

            #region 計算R矩陣
            XYZ4[,] R = new XYZ4[n + 2, 1];
            R[0, 0] = startTangent != null ? startTangent : GetCircularTangent(dataPoints[0], dataPoints[1], dataPoints[2], GetCircularTangentAt.Start);
            R[n + 1, 0] = endTangent != null ? endTangent : GetCircularTangent(dataPoints[n-3], dataPoints[n-2], dataPoints[n-1], GetCircularTangentAt.End);
            for (int i = 0; i < n; i++)
            {
                int i_M = i + 1;
                R[i_M, 0] = DataPoints[i];
            }
            #endregion

            #region 計算每個線段的N矩陣
            Ni = new double[n - 1][,];
            for (int i = 0; i < n - 1; i++)
            {
                Ni[i] = GetNci(i, false);
            }
            #endregion

            #region 求解控制點
            invM = Inverse(M);
            //for (int i = 0; i < invM.GetLength(0); i++)
            //{
            //    for (int j = 0; j < invM.GetLength(1); j++)
            //    {
            //        if (double.IsNaN(invM[i, j])
            //            || double.IsInfinity(invM[i, j]))
            //        {

            //        }
            //    }
            //}
            
            if (n == 2)
            {
                ControlPoints = new XYZ4[] { DataPoints[0], DataPoints[0], DataPoints[1], DataPoints[1] };
            }
            else
            {
                ControlPoints = new XYZ4[n + 2];
                for (int i = 0; i < ControlPoints.GetLength(0); i++)
                {
                    ControlPoints[i] = new XYZ4();
                    for (int j = 0; j < ControlPoints.GetLength(0); j++)
                    {
                        ControlPoints[i] += invM[i, j] * R[j, 0];
                    }
                    if (double.IsNaN(ControlPoints[i].X)
                        || double.IsInfinity(ControlPoints[i].X)
                        || double.IsNaN(ControlPoints[i].Y)
                        || double.IsInfinity(ControlPoints[i].Y)
                        || double.IsNaN(ControlPoints[i].Z)
                        || double.IsInfinity(ControlPoints[i].Z)
                        )
                    {

                    }
                }
            }
            #endregion
        }

        private double[,] GetNci(int segmentsIndex, bool print)
        {
            if (segmentsIndex >= 0 && segmentsIndex < DataPoints.Length - 1)
            {
                int i_d = segmentsIndex + 2;
                double[,] Nci = NewIdentityMatrix(4);
                Nci[0, 0] = Pow(Delta_i[i_d], 2)
                            / (Delta_i[i_d - 1] + Delta_i[i_d])
                            / (Delta_i[i_d - 2] + Delta_i[i_d - 1] + Delta_i[i_d]);
                Nci[0, 2] = Pow(Delta_i[i_d - 1], 2)
                            / (Delta_i[i_d - 1] + Delta_i[i_d] + Delta_i[i_d + 1])
                            / (Delta_i[i_d - 1] + Delta_i[i_d]);
                Nci[1, 2] = 3 * Delta_i[i_d] * Delta_i[i_d - 1]
                            / (Delta_i[i_d - 1] + Delta_i[i_d] + Delta_i[i_d + 1])
                            / (Delta_i[i_d - 1] + Delta_i[i_d]);
                Nci[2, 2] = 3 * Pow(Delta_i[i_d], 2)
                            / (Delta_i[i_d - 1] + Delta_i[i_d] + Delta_i[i_d + 1])
                            / (Delta_i[i_d - 1] + Delta_i[i_d]);
                Nci[3, 3] = Pow(Delta_i[i_d], 2)
                            / (Delta_i[i_d] + Delta_i[i_d + 1] + Delta_i[i_d + 2])
                            / (Delta_i[i_d] + Delta_i[i_d + 1]);
                Nci[3, 2] = -1 *
                            (
                                  1.0 / 3.0 * Nci[2, 2]
                                + Nci[3, 3]
                                + Pow(Delta_i[i_d], 2)
                                  / (Delta_i[i_d] + Delta_i[i_d + 1])
                                  / (Delta_i[i_d - 1] + Delta_i[i_d] + Delta_i[i_d + 1])
                            );
                Nci[1, 0] = -3 * Nci[0, 0];
                Nci[2, 0] = 3 * Nci[0, 0];
                Nci[3, 0] = -Nci[0, 0];
                Nci[0, 1] = 1 - Nci[0, 0] - Nci[0, 2];
                Nci[1, 1] = 3 * Nci[0, 0] - Nci[1, 2];
                Nci[2, 1] = -(3 * Nci[0, 0] + Nci[2, 2]);
                Nci[3, 1] = Nci[0, 0] - Nci[3, 2] - Nci[3, 3];

                //Print Nci
                if (print)
                {
                    Console.WriteLine("i = " + segmentsIndex);
                    Console.WriteLine("Nci = ");
                    Console.WriteLine("{");
                    for (int ii = 0; ii < Nci.GetLength(0); ii++)
                    {
                        Console.Write("{");
                        for (int j = 0; j < Nci.GetLength(1); j++)
                        {
                            Console.Write("\t");
                            Console.Write(Nci[ii, j].ToString("0.000000"));
                            if (j != (Nci.GetLength(1) - 1))
                                Console.Write(",");
                        }
                        Console.Write("}");
                        if (ii != (Nci.GetLength(0) - 1))
                            Console.WriteLine(",");
                        else
                            Console.WriteLine("");
                    }
                    Console.WriteLine("}");
                }
                return Nci;
            }
            return null;
        }

        public static XYZ4 Blending(double u, double[,] Nci, XYZ4[] cp)
        {
            //tou
            double[] tou = new Double[] { 1, u, u * u, u * u * u };
            //bending
            double[] blending = Dot(tou, Nci);
            //ControlPoint
            XYZ4 p = new XYZ4();
            for (int i = 0; i < blending.Length; i++)
            {
                p += blending[i] * cp[i];
            }
            return p;
        }

        public static XYZ3 dU_Blending(double u, double[,] Nci, XYZ4[] cp)
        {
            //tou
            double[] tou = new Double[] { 0, 1, 2.0 * u, 3.0 * u * u };
            //bending
            double[] blending = Dot(tou, Nci);
            //ControlPoint
            XYZ3 p = new XYZ3();
            for (int i = 0; i < blending.Length; i++)
            {
                p = p + (blending[i] * cp[i]);
            }
            return p;
        }

        public static XYZ3 dU2_Blending(double u, double[,] Nci, XYZ4[] cp)
        {
            //tou
            double[] tou = new Double[] { 0, 0, 2.0, 6.0 * u };
            //bending
            double[] blending = Dot(tou, Nci);
            //ControlPoint
            XYZ3 p = new XYZ3();
            for (int i = 0; i < blending.Length; i++)
            {
                p = p + (blending[i] * cp[i]);
            }
            return p;
        }

        public Tuple<int, double> Param_Mapping(double u)
        {
            double globalU = u * (DataPoints.Length - 1);
            double localU = globalU % 1;
            int sIndexU = (int)(globalU > 0 ? System.Math.Floor(globalU) : System.Math.Ceiling(globalU));
            if (sIndexU > LastSegmentIndex)
            {
                int deltaIndex = sIndexU - LastSegmentIndex;
                localU += deltaIndex;
                sIndexU = LastSegmentIndex;
            }
            else if (sIndexU < 0)
            {
                localU += sIndexU;
                sIndexU = 0;
            }

            return new Tuple<int, double>(sIndexU, localU);
        }

        public XYZ4 P(double u)
        {
            Tuple<int, double> mappedParas = Param_Mapping(u);
            int sIndexU = mappedParas.Item1;
            double localU = mappedParas.Item2;

            //Control Points
            XYZ4[] cP = new XYZ4[] { ControlPoints[sIndexU], ControlPoints[sIndexU + 1], ControlPoints[sIndexU + 2], ControlPoints[sIndexU + 3] };
            //Nci
            double[,] Nci = Ni[sIndexU];

            XYZ4 p = Blending(localU, Nci, cP);
            return p;
        }

        public XYZ3 dU(double u)
        {
            Tuple<int, double> mappedParas = Param_Mapping(u);
            int sIndexU = mappedParas.Item1;
            double localU = mappedParas.Item2;

            //Control Points
            XYZ4[] cP = new XYZ4[] { ControlPoints[sIndexU], ControlPoints[sIndexU + 1], ControlPoints[sIndexU + 2], ControlPoints[sIndexU + 3] };
            //Nci
            double[,] Nci = Ni[sIndexU];

            XYZ3 p = dU_Blending(localU, Nci, cP);
            return p;
        }

        public XYZ3 dU2(double u)
        {
            Tuple<int, double> mappedParas = Param_Mapping(u);
            int sIndexU = mappedParas.Item1;
            double localU = mappedParas.Item2;

            //Control Points
            XYZ4[] cP = new XYZ4[] { ControlPoints[sIndexU], ControlPoints[sIndexU + 1], ControlPoints[sIndexU + 2], ControlPoints[sIndexU + 3] };
            //Nci
            double[,] Nci = Ni[sIndexU];

            XYZ3 p = dU2_Blending(localU, Nci, cP);
            return p;
        }
    }
}
