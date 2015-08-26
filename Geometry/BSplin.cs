using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTL.Geometry
{
    class BSplin : Entity
    {
        public PointD[] DataPoints;
        public PointD[] ControlPoints;
        double[] delta_i;

        bool DataOutput = false;

        public BSplin()
        {
            
        }

        public void SetControllPoints(List<PointD> dataPoints)
        {
            DataPoints = dataPoints.ToArray();
            int n = DataPoints.Length;

            double[,] M = ZeroMatrix(n + 2, n + 2);
            PointD[,] R = new PointD[n + 2, 1];
            double[,] invM;

            #region 計算弦長
            delta_i = new double[n + 3];
            delta_i[0] = 0;
            delta_i[1] = 0;
            delta_i[n+1] = 0;
            delta_i[n+1+1] = 0;
            for (int i = 0; i < n - 1; i++)
            {
                delta_i[i + 2] = Norm(DataPoints[i + 1] - DataPoints[i]);
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
                PointD a0 = DataPoints[1] - DataPoints[0];
                PointD b0 = DataPoints[2] - DataPoints[0];
                PointD c0 = Cross(a0, b0);
                PointD r0 = (Norm(a0) * Norm(a0) * Cross(b0, c0) + Norm(b0) * Norm(b0) * Cross(c0, a0)) / (2 * Norm(c0) * Norm(c0));
                R[0, 0] = Norm(a0) * Cross(r0, c0) / Norm(Cross(r0, c0));
                PointD an = DataPoints[n - 2] - DataPoints[n - 1];
                PointD bn = DataPoints[n - 3] - DataPoints[n - 1];
                PointD cn = Cross(an, bn);
                PointD rn = (Norm(an) * Norm(an) * Cross(bn, cn) + Norm(bn) * Norm(bn) * Cross(cn, an)) / (2 * Norm(cn) * Norm(cn));
                R[n + 1, 0] = -1.0 * Norm(an) * Cross(rn, cn) / Norm(Cross(rn, cn));
            }
            for (int i = 0; i < n; i++)
            {
                int i_M = i + 1;
                R[i_M, 0] = DataPoints[i];
            }
            #endregion

            #region 求解控制點
            invM = MatrixInverse(M);
            if (n == 2)
            {
                ControlPoints = new PointD[] { DataPoints[0], DataPoints[0], DataPoints[1], DataPoints[1] };
            }
            else
            {
                ControlPoints = new PointD[n + 2];
                for (int i = 0; i < ControlPoints.GetLength(0); i++)
                {
                    ControlPoints[i] = new PointD();
                    for (int j = 0; j < ControlPoints.GetLength(0); j++)
                    {
                        ControlPoints[i] += invM[i, j] * R[j, 0];
                    }
                }
            }
            #endregion

            #region Debug
            if (DataOutput)
            {
                //Print Knot spans(chord length)
                Console.WriteLine("");
                Console.WriteLine("Knot spans = ");
                foreach (var item in delta_i)
                {
                    Console.WriteLine(item.ToString("0.000000"));
                }
                //Print M
                Console.WriteLine("");
                Console.WriteLine("M = ");
                Console.WriteLine("{");
                for (int i = 0; i < M.GetLength(0); i++)
                {
                    Console.Write("{");
                    for (int j = 0; j < M.GetLength(1); j++)
                    {
                        Console.Write("\t");
                        Console.Write(M[i, j].ToString("0.000000"));
                        if (j != (M.GetLength(1) - 1))
                        {
                            Console.Write(",");
                        }
                    }
                    Console.Write("}");
                    if (i != (M.GetLength(0) - 1))
                    {
                        Console.WriteLine(",");
                    }
                }
                Console.WriteLine("}");
                //Print R
                Console.WriteLine("");
                Console.WriteLine("R = ");
                foreach (var item in R)
                {
                    Console.WriteLine("{" + item.X.ToString("0.000000") + ",\t\t" + item.Y.ToString("0.000000") + ",\t\t" + item.Z.ToString("0.000000") + "}");
                }
                Console.WriteLine("}");
                //Print invM
                Console.WriteLine("");
                Console.WriteLine("invM = ");
                Console.WriteLine("{");
                for (int i = 0; i < invM.GetLength(0); i++)
                {
                    Console.Write("{");
                    for (int j = 0; j < invM.GetLength(1); j++)
                    {
                        Console.Write("\t");
                        Console.Write(invM[i, j].ToString("0.000000"));
                        if (j != (invM.GetLength(1) - 1))
                        {
                            Console.Write(",");
                        }
                    }
                    Console.Write("}");
                    if (i != (invM.GetLength(0) - 1))
                    {
                        Console.WriteLine(",");
                    }
                }
                Console.WriteLine("");
                Console.WriteLine("}");
                //Print Control Points
                Console.WriteLine("");
                Console.WriteLine("Control Points = ");
                foreach (var item in ControlPoints)
                {
                    Console.WriteLine("{" + item.X.ToString("0.000000") + ",\t\t" + item.Y.ToString("0.000000") + ", \t\t" + item.Z.ToString("0.000000") + "}");
                }
            }
            #endregion Debug
        }

        public PolyLine[] ToPolyline(int slice)
        {
            if (DataPoints != null && ControlPoints != null)
            {
                int n = DataPoints.Length;
                PointD[] p = new PointD[(n - 1) * slice + 1];
                for (int i = 0; i < n - 1; i++)
                {
                    PointD[] cP = new PointD[] { ControlPoints[i], ControlPoints[i + 1], ControlPoints[i + 2], ControlPoints[i + 3] };
                    //Nci
                    double[,] Nci = GetNci(i, DataOutput);

                    Console.WriteLine();
                    for (int j = 0; j < slice + 1; j++)
                    {
                        double tou = (double)j * 1.0 / (double)slice;
                        double[,] cc = MatrixDot(new Double[,] { { 1, tou, tou * tou, tou * tou * tou } }, Nci);
                        //Console.WriteLine(i + "th segments sum of Blenging sum:" + (cc[0, 0] + cc[0, 1] + cc[0, 2] + cc[0, 3]));
                        p[i * slice + j] = (cc[0, 0] * cP[0] + cc[0, 1] * cP[1] + cc[0, 2] * cP[2] + cc[0, 3] * cP[3]);
                    }
                }
                PolyLine aPlyLine = new PolyLine(p) { Color = this.Color, LineWidth = 2 };
                //aPlyLine = new PolyLine(ControlPoints) { Color = this.Color };
                return new PolyLine[] { aPlyLine };
            }
            else
                return null;
        }
        public PolyLine[] ToPolylines(int slice)
        {
            int n = DataPoints.Length;
            PolyLine[] polylines = new PolyLine[n - 1];
            for (int i = 0; i < polylines.Length; i++)
            {
                polylines[i] = GetSegmentsAsPL(i, slice);
            }
            return polylines;
        }
        public PolyLine GetSegmentsAsPL(int i, int slice)
        {
            Console.WriteLine();
            PointD[] p = new PointD[slice + 1];
            PointD[] cP = new PointD[] { ControlPoints[i], ControlPoints[i + 1], ControlPoints[i + 2], ControlPoints[i + 3] };
            //Nci
            double[,] Nci = GetNci(i, DataOutput);
            
            for (int j = 0; j < slice + 1; j++)
            {
                double tou = (double)j * 1.0 / (double)slice;
                Double[,] touM = new Double[,] { { 1, tou, tou * tou, tou * tou * tou } };
                double[,] cc = MatrixDot(touM, Nci);
                if (DataOutput)
                {
                    //Console.WriteLine(i + "tou:" + tou);
                    //foreach (var item in touM)
                    //{
                    //    Console.Write(item + ",");
                    //}
                    //Console.WriteLine("");
                    //foreach (var item in cc)
                    //{
                    //    Console.Write(item + ",");
                    //}
                    //Console.WriteLine("");
                    //Console.WriteLine(i + "th segments sum of Blenging sum:" + (cc[0, 0] + cc[0, 1] + cc[0, 2] + cc[0, 3]));
                }
                p[j] = (cc[0, 0] * cP[0] + cc[0, 1] * cP[1] + cc[0, 2] * cP[2] + cc[0, 3] * cP[3]);
            }
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            PolyLine aPlyLine = new PolyLine(p) { Color = System.Drawing.Color.FromArgb(255, rnd.Next(256), rnd.Next(256), rnd.Next(256)), LineWidth = 2 };
            //aPlyLine = new PolyLine(ControlPoints) { Color = this.Color };
            return aPlyLine;
        }

        public PolyLine ToControlPointLine(System.Drawing.Color color)
        {
            PointD[] p = new PointD[ControlPoints.Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = ControlPoints[i];
            }
            return new PolyLine(p) { Color = color, LineType = PTL.Definitions.LineType.DotDashed };
        }
        public Circle[] ToDataPointMarks()
        {
            int n = DataPoints.Length ;
            double markSize = 0.05;
            List<Circle> pointMarks = new List<Circle>();
            foreach (PointD p in DataPoints)
            {
                Circle aCircle = new Circle(p, markSize / 2) { Color = this.Color, LineWidth = 3};
                pointMarks.Add(aCircle);
            }
            for (int i = 0; i < n+2; i++)
            {
                Circle aCircle = new Circle(ControlPoints[i], markSize / 2);
                aCircle.Color = System.Drawing.Color.Blue;
                pointMarks.Add(aCircle);
            }
            //foreach (PointD p in ControlPoints)
            //{
            //    Circle aCircle = new Circle(p, markSize / 2);
            //    aCircle.Color = System.Drawing.Color.Blue;
            //    pointMarks.Add(aCircle);
                
            //}
            return pointMarks.ToArray();
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

        public override PointD[] Boundary
        {
            get { throw new NotImplementedException(); }
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void PlotInOpenGL()
        {
            throw new NotImplementedException();
        }

        public override void Transform(double[,] TransformMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
