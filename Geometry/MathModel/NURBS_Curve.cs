using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleArrayExtension
{
    
}

namespace PTL.Geometry.MathModel
{
    public class NURBS_Curve : PTL.Mathematics.Math2
    {
        public Coordinate[] DataPoints;
        public Coordinate[] ControlPoints;
        public double[][,] Ni;
        public double[] delta_i;
        bool DataOutput = false;

        public NURBS_Curve(Coordinate[]dataPoints)
        {
            Calculate_NURBS_Curve(dataPoints);
        }

        public void Calculate_NURBS_Curve(Coordinate[] dataPoints)
        {
            DataPoints = dataPoints.ToArray();
            int n = DataPoints.Length;

            double[,] M = ZeroMatrix(n + 2, n + 2);
            Coordinate[,] R = new Coordinate[n + 2, 1];
            double[,] invM;

            #region 計算弦長
            delta_i = new double[n + 3];
            delta_i[0] = 0;
            delta_i[1] = 0;
            delta_i[n+1] = 0;
            delta_i[n+1+1] = 0;
            for (int i = 0; i < n - 1; i++)
            {
                delta_i[i + 2] = Norm((DataPoints[i + 1] - DataPoints[i]).Value);
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
                M[i_M, i] = Pow(delta_i[i_d], 2)
                            / (delta_i[i_d - 1] + delta_i[i_d])
                            / (delta_i[i_d - 2] + delta_i[i_d - 1] + delta_i[i_d]);
                //gi
                M[i_M, i + 2] = Pow(delta_i[i_d - 1], 2)
                                / (delta_i[i_d - 1] + delta_i[i_d] + delta_i[i_d + 1])
                                / (delta_i[i_d - 1] + delta_i[i_d]);
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
            else
            {
                Coordinate a0 = DataPoints[1] - DataPoints[0];
                Coordinate b0 = DataPoints[2] - DataPoints[0];
                Coordinate c0 = new Coordinate(Cross(a0.Value, b0.Value));
                Coordinate r0 = (Norm(a0) * Norm(a0) * Cross(b0, c0) + Norm(b0) * Norm(b0) * Cross(c0, a0)) / (2 * GetLength(c0) * GetLength(c0));
                R[0, 0] = GetLength(a0) * Cross(r0, c0) / GetLength(Cross(r0, c0));
                Coordinate an = DataPoints[n - 2] - DataPoints[n - 1];
                Coordinate bn = DataPoints[n - 3] - DataPoints[n - 1];
                Coordinate cn = Cross(an, bn);
                Coordinate rn = (GetLength(an) * GetLength(an) * Cross(bn, cn) + GetLength(bn) * GetLength(bn) * Cross(cn, an)) / (2 * GetLength(cn) * GetLength(cn));
                R[n + 1, 0] = -1.0 * GetLength(an) * Cross(rn, cn) / GetLength(Cross(rn, cn));
            }
            for (int i = 0; i < n; i++)
            {
                int i_M = i + 1;
                R[i_M, 0] = DataPoints[i];
            }
            #endregion

            #region 計算每個線段的N矩陣
            Ni = new double[n][,];
            for (int i = 0; i < n; i++)
            {
                Ni[i] = GetNci(i, false);
            }
            #endregion

            #region 求解控制點
            invM = MatrixInverse(M);
            if (n == 2)
            {
                ControlPoints = new Coordinate[] { DataPoints[0], DataPoints[0], DataPoints[1], DataPoints[1] };
            }
            else
            {
                ControlPoints = new Coordinate[n + 2];
                for (int i = 0; i < ControlPoints.GetLength(0); i++)
                {
                    ControlPoints[i] = new Coordinate();
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
                Nci[0, 0] = Pow(delta_i[i_d], 2)
                            / (delta_i[i_d - 1] + delta_i[i_d])
                            / (delta_i[i_d - 2] + delta_i[i_d - 1] + delta_i[i_d]);
                Nci[0, 2] = Pow(delta_i[i_d - 1], 2)
                            / (delta_i[i_d - 1] + delta_i[i_d] + delta_i[i_d + 1])
                            / (delta_i[i_d - 1] + delta_i[i_d]);
                Nci[1, 2] = 3 * delta_i[i_d] * delta_i[i_d - 1]
                            / (delta_i[i_d - 1] + delta_i[i_d] + delta_i[i_d + 1])
                            / (delta_i[i_d - 1] + delta_i[i_d]);
                Nci[2, 2] = 3 * Pow(delta_i[i_d], 2)
                            / (delta_i[i_d - 1] + delta_i[i_d] + delta_i[i_d + 1])
                            / (delta_i[i_d - 1] + delta_i[i_d]);
                Nci[3, 3] = Pow(delta_i[i_d], 2)
                            / (delta_i[i_d] + delta_i[i_d + 1] + delta_i[i_d + 2])
                            / (delta_i[i_d] + delta_i[i_d + 1]);
                Nci[3, 2] = -1 *
                            (
                                  1.0 / 3.0 * Nci[2, 2]
                                + Nci[3, 3]
                                + Pow(delta_i[i_d], 2)
                                  / (delta_i[i_d] + delta_i[i_d + 1])
                                  / (delta_i[i_d - 1] + delta_i[i_d] + delta_i[i_d + 1])
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

        public Coordinate CurveFunc(double para)
        {
            double globalPara = para * (DataPoints.Length - 1);
            double localPara = globalPara % 1;
            int sIndex = (int)(globalPara - localPara);


        }
    }
}
