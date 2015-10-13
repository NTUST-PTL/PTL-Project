using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry.MathModel
{
    public class NUB_Curve : PTL.Mathematics.ProtectedPTLM
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

        public void Calculate_NURBS_Curve(XYZ4[] dataPoints, bool closed = false)
        {
            DataPoints = dataPoints.ToArray();
            int n = DataPoints.Length;
            this.LastSegmentIndex = n - 2;
            double[,] M = ZeroMatrix(n + 2, n + 2);
            XYZ4[,] R = new XYZ4[n + 2, 1];
            double[,] invM;

            #region 計算弦長
            Delta_i = new double[n + 3];
            Delta_i[0] = 0;
            Delta_i[1] = 0;
            Delta_i[n+1] = 0;
            Delta_i[n+1+1] = 0;
            for (int i = 0; i < n - 1; i++)
            {
                Delta_i[i + 2] = Norm((DataPoints[i + 1] - DataPoints[i]).Values);
            }
            #endregion

            #region 計算M矩陣
            M[0, 0] = -3;
            M[0, 1] = 3;
            M[1, 0] = 1;
            M[n, n+1] = 1;
            M[n+1, n] = -3;
            M[n+1, n+1] = 3;
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
            if (n == 2)
            {
                R[0, 0] = (DataPoints[1] - DataPoints[0]) / 3.0;
                R[n + 1, 0] = (DataPoints[0] - DataPoints[1]) / 3.0;
            }
            else if(closed)
            {
                throw new NotImplementedException();
            }
            else
            {
                XYZ4 a0 = DataPoints[1] - DataPoints[0];
                XYZ4 b0 = DataPoints[2] - DataPoints[0];
                XYZ4 c0 = Cross(a0, b0);
                XYZ4 r0 = (Norm(a0) * Norm(a0) * (XYZ4)Cross(b0, c0) + Norm(b0) * Norm(b0) * (XYZ4)Cross(c0, a0)) / (2 * GetLength(c0) * GetLength(c0));
                R[0, 0] = GetLength(a0) * (XYZ4)Cross(r0, c0) / GetLength(Cross(r0, c0));
                XYZ4 an = DataPoints[n - 2] - DataPoints[n - 1];
                XYZ4 bn = DataPoints[n - 3] - DataPoints[n - 1];
                XYZ4 cn = Cross(an, bn);
                XYZ4 rn = (GetLength(an) * GetLength(an) * (XYZ4)Cross(bn, cn) + GetLength(bn) * GetLength(bn) * (XYZ4)Cross(cn, an)) / (2 * GetLength(cn) * GetLength(cn));
                R[n + 1, 0] = -1.0 * GetLength(an) * (XYZ4)Cross(rn, cn) / GetLength(Cross(rn, cn));
            }

            for (int i = 0; i < n; i++)
            {
                int i_M = i + 1;
                R[i_M, 0] = DataPoints[i];
            }
            #endregion

            #region 計算每個線段的N矩陣
            Ni = new double[n-1][,];
            for (int i = 0; i < n-1; i++)
            {
                Ni[i] = GetNci(i, false);
            }
            #endregion

            #region 求解控制點
            invM = MatrixInverse(M);
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
                }
            }
            #endregion

            #region 長度計算
            //this.TotalChordLength = delta_i.Sum();
            ////this.TotalLength = GetTotalLength(10000);

            //this.segmentLengthes = new double[DataPoints.Length - 1];
            //for (int i = 0; i < DataPoints.Length - 1; i++)
            //{
            //    currentSegmentsIndex = i;
            //    this.segmentLengthes[i] = Integration.trapzd(SpeedFunc4CaIntegration, 0, 1, 10000);
            //    this.TotalLength += this.segmentLengthes[i];
            //}
            #endregion

            #region Debug
            //if (DataOutput)
            //{
            //    //Print Knot spans(chord length)
            //    Console.WriteLine("");
            //    Console.WriteLine("Knot spans = ");
            //    foreach (var item in delta_i)
            //    {
            //        Console.WriteLine(item.ToString("0.000000"));
            //    }
            //    //Print M
            //    Console.WriteLine("");
            //    Console.WriteLine("M = ");
            //    Console.WriteLine("{");
            //    for (int i = 0; i < M.GetLength(0); i++)
            //    {
            //        Console.Write("{");
            //        for (int j = 0; j < M.GetLength(1); j++)
            //        {
            //            Console.Write("\t");
            //            Console.Write(M[i, j].ToString("0.000000"));
            //            if (j != (M.GetLength(1) - 1))
            //            {
            //                Console.Write(",");
            //            }
            //        }
            //        Console.Write("}");
            //        if (i != (M.GetLength(0) - 1))
            //        {
            //            Console.WriteLine(",");
            //        }
            //    }
            //    Console.WriteLine("}");
            //    //Print R
            //    Console.WriteLine("");
            //    Console.WriteLine("R = ");
            //    foreach (var item in R)
            //    {
            //        Console.WriteLine("{" + item.X.ToString("0.000000") + ",\t\t" + item.Y.ToString("0.000000") + ",\t\t" + item.Z.ToString("0.000000") + "}");
            //    }
            //    Console.WriteLine("}");
            //    //Print invM
            //    Console.WriteLine("");
            //    Console.WriteLine("invM = ");
            //    Console.WriteLine("{");
            //    for (int i = 0; i < invM.GetLength(0); i++)
            //    {
            //        Console.Write("{");
            //        for (int j = 0; j < invM.GetLength(1); j++)
            //        {
            //            Console.Write("\t");
            //            Console.Write(invM[i, j].ToString("0.000000"));
            //            if (j != (invM.GetLength(1) - 1))
            //            {
            //                Console.Write(",");
            //            }
            //        }
            //        Console.Write("}");
            //        if (i != (invM.GetLength(0) - 1))
            //        {
            //            Console.WriteLine(",");
            //        }
            //    }
            //    Console.WriteLine("");
            //    Console.WriteLine("}");
            //    //Print Control Points
            //    Console.WriteLine("");
            //    Console.WriteLine("Control Points = ");
            //    foreach (var item in ControlPoints)
            //    {
            //        Console.WriteLine("{" + item.X.ToString("0.000000") + ",\t\t" + item.Y.ToString("0.000000") + ", \t\t" + item.Z.ToString("0.000000") + "}");
            //    }
            //}
            #endregion Debug
        }

        private double[,] GetNci(int segmentsIndex, bool print)
        {
            if (segmentsIndex >= 0 && segmentsIndex < DataPoints.Length - 1)
            {
                int i_d = segmentsIndex + 2;
                double[,] Nci = IdentityMatrix(4);
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
            double[] blending = MatrixDot(tou, Nci);
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
            double[] blending = MatrixDot(tou, Nci);
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
            double[] blending = MatrixDot(tou, Nci);
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
