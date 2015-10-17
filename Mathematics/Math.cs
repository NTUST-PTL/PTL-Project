using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PTL.Geometry;
using PTL.Definitions;
using PTL.Geometry.MathModel;

namespace PTL.Mathematics
{
    public class PTLM
    {
        #region 三角函數
        public static double Cos(double trad)
        {
            double tvalue;
            tvalue = System.Math.Cos(trad);
            return tvalue;
        }
        public static double Sin(double trad)
        {
            double tvalue;
            tvalue = System.Math.Sin(trad);
            return tvalue;
        }
        public static double Tan(double trad)
        {
            double tvalue;
            tvalue = System.Math.Tan(trad);
            return tvalue;
        }
        public static double Sec(double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Cos(trad);
            return tvalue;
        }
        public static double Acos(double tv)
        {
            double tvalue;
            tvalue = System.Math.Acos(tv);
            return tvalue;
        }
        public static double Atan2(double y, double x)
        {
            double tvalue;
            tvalue = System.Math.Atan2(y, x);
            return tvalue;
        }
        public static double Atan(double tv)
        {
            double tvalue;
            tvalue = System.Math.Atan(tv);
            return tvalue;
        }
        public static double Asin(double tv)
        {
            double tvalue;
            tvalue = System.Math.Asin(tv);
            return tvalue;
        }
        public static double Asec(double tv)
        {
            double tvalue;
            tvalue = System.Math.Acos(1.0 / tv);
            return tvalue;
        }
        public static double Csc(double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Sin(trad);
            return tvalue;
        }
        public static double Cot(double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Tan(trad);
            return tvalue;
        }
        #endregion

        #region 單位轉換
        public static double DegToRad(double tdeg)
        {
            double trad;
            trad = tdeg / 180.0 * System.Math.PI;
            return trad;
        }
        public static double RadToDeg(double trad)
        {
            double tdeg;
            tdeg = trad / System.Math.PI * 180.0;
            return tdeg;
        }
        public static double mmToInch(double tmm)
        {
            double tInch;
            tInch = tmm / 25.4;
            return tInch;
        }
        public static double InchTomm(double tInch)
        {
            double tmm;
            tmm = tInch * 25.4;
            return tmm;
        }
        public static string DegToDMS(double x)
        {
            int x1 = (int)x;
            int x2 = (int)((x - x1) * 60);
            int x3 = (int)((((x - x1) * 60) - x2) * 60);
            string ss = "";
            ss = String.Format("{0}", x1) + "d" + String.Format("{0,3}", x2) + "m" + String.Format("{0,3}", x3) + "s";
            return ss;
        }
        public static string DegToDMS(double x, short n)
        {
            double absx = Abs(x);
            int x1 = (int)absx;
            int x2 = (int)((absx - x1) * 60);
            int x3 = (int)((((absx - x1) * 60) - x2) * 60);
            string ss = "";

            if (n == 3)
            {
                ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
            }
            if (n == 2)
            {
                if (x1 != 0)
                    ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
                else
                    ss = String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
            }
            if (n == 1)
            {
                if (x1 != 0)
                {
                    ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
                }
                else
                {
                    if (x2 != 0)
                        ss = String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
                    else
                        ss = String.Format("{0,3}", x3) + "S";
                }
            }

            if (absx / x == -1)
                ss = "-  " + ss;

            return ss;
        }
        public static string RadToDMS(double y)
        {
            string ss = DegToDMS(RadToDeg(y));
            return ss;
        }
        public static string RadToDMS(double y, short n)
        {
            string ss = DegToDMS(RadToDeg(y), n);
            return ss;
        }
        public static double DMSToDeg(double dd, double mm, double ss)
        {
            double deg = dd + mm / 60.0 + ss / 3600.0;
            return deg;
        }
        public static double DMSToRad(double dd, double mm, double ss)
        {
            double rad = DegToRad(DMSToDeg(dd, mm, ss));
            return rad;
        }
        #endregion

        #region Matrix & Vector
        public static double[] Normalize(double[] r)
        {
            double rr = 0;
            for (int i = 0; i < r.Length; i++)
                rr += r[i] * r[i];
            double len = 1 / Sqrt(rr);
            double[] nr = new double[r.Length];
            for (int i = 0; i < r.Length; i++)
                nr[i] = r[i] * len;
            return nr;
        }
        public static double GetLength(double[] input)
        {
            double sum = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                sum = sum + System.Math.Pow(input[i], 2);
            }
            double len = Sqrt(sum);

            return len;
        }
        public static double Norm(double[] input)
        {
            double sum = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                sum = sum + System.Math.Pow(input[i], 2);
            }
            double len = Sqrt(sum);

            return len;
        }

        public static double[] MatrixDot(double[] array1, double[] array2)
        {
            int r1 = array1.Length;  // UBound

            double[] outptr = new double[r1];
            for (int i = 0; i < r1; i++)
            {
                outptr[i] = array1[i] * array2[i];
            }
            return outptr;
        }
        public static double[] MatrixDot(double[] array1, double[,] array2)
        {
            int r2 = array2.GetLength(0);  // UBound
            int c2 = array2.GetLength(1);  // UBound

            double[] outptr = new double[r2];
            for (int i = 0; i < r2; i++)
                outptr[i] = 0.0;

            for (int i = 0; i < c2; i++)
            {
                for (int k = 0; k < r2; k++)
                {
                    outptr[i] = outptr[i] + array1[k] * array2[k, i];
                }
            }
            return outptr;
        }
        public static double[] MatrixDot(double[,] array1, double[] array2)
        {
            int r1, c1, i, k;
            r1 = array1.GetUpperBound(0);  // UBound
            c1 = array1.GetUpperBound(1);  // UBound

            double[] outptr = new double[r1 + 1];
            //r1 = UBound(ptr1,1);
            //c1 = UBound(ptr1, 2);
            //c2 = UBound(ptr2, 2);
            for (i = 0; r1 >= i; i++)
            {
                outptr[i] = 0.0;
            }
            for (i = 0; r1 >= i; i++)
            {
                for (k = 0; c1 >= k; k++)
                {
                    outptr[i] = outptr[i] + array1[i, k] * array2[k];
                }
            }
            return outptr;
        }
        public static double[,] MatrixDot(double[,] array1, double[,] array2)
        {
            int r1, c1, c2, i, j, k;
            r1 = array1.GetUpperBound(0);  // UBound
            c1 = array1.GetUpperBound(1);  // UBound
            c2 = array2.GetUpperBound(1);  // UBound

            double[,] outptr = new double[r1 + 1, c2 + 1];
            //r1 = UBound(ptr1,1);
            //c1 = UBound(ptr1, 2);
            //c2 = UBound(ptr2, 2);
            for (i = 0; r1 >= i; i++)
            {
                for (j = 0; c2 >= j; j++)
                {
                    outptr[i, j] = 0.0;
                }
            }
            for (i = 0; r1 >= i; i++)
            {
                for (j = 0; c2 >= j; j++)
                {
                    for (k = 0; c1 >= k; k++)
                    {
                        outptr[i, j] = outptr[i, j] + array1[i, k] * array2[k, j];
                    }
                }
            }
            return outptr;
        }
        public static double[,] MatrixDot(params double[][,] arrarys)
        {
            for (int i = 0; i < arrarys.Length - 1; i++)
                if (arrarys[i].GetLength(1) != arrarys[i + 1].GetLength(0))
                    return null;
            double[,] outputArray = arrarys[0];
            for (int i = 1; i < arrarys.Length; i++)
            {
                outputArray = MatrixDot(outputArray, arrarys[i]);
            }
            return outputArray;
        }
        public static double[,] MatrixAdd(params double[][,] arrarys)
        {
            for (int i = 0; i < arrarys.Length - 1; i++)
                if (arrarys[i].GetLength(0) != arrarys[i + 1].GetLength(0) && arrarys[i].GetLength(1) != arrarys[i + 1].GetLength(1))
                    return null;

            Func<double[,], double[,], double[,]> AddFunc = (double[,] ptr1, double[,] ptr2) =>
            #region
            {
                int r1, c1;
                r1 = ptr1.GetUpperBound(0);  // UBound
                c1 = ptr1.GetUpperBound(1);  // UBound
                                             // r1 = UBound(ptr1,1);
                                             // c1 = UBound(ptr1,2);
                double[,] outptr = new double[r1 + 1, c1 + 1];
                for (int i = 0; r1 >= i; i++)
                {
                    for (int j = 0; c1 >= j; j++)
                    {
                        outptr[i, j] = ptr1[i, j] + ptr2[i, j];
                    }
                }
                return outptr;
            };
            #endregion
            double[,] outputArray = arrarys[0];
            for (int i = 1; i < arrarys.Length; i++)
            {
                outputArray = AddFunc(outputArray, arrarys[i]);
            }
            return outputArray;
        }
        public static double[] Cross(double[] ptr1, double[] ptr2)
        {
            double[] outptr = new double[3];
            outptr[0] = ptr1[1] * ptr2[2] - ptr1[2] * ptr2[1];
            outptr[1] = ptr1[2] * ptr2[0] - ptr1[0] * ptr2[2];
            outptr[2] = ptr1[0] * ptr2[1] - ptr1[1] * ptr2[0];
            return outptr;
        }
        public static double Dot(double[] ptr1, double[] ptr2)
        {
            double value = 0.0;

            for (int i = 0; i <= ptr1.GetUpperBound(0); i++)
            {
                value = value + ptr1[i] * ptr2[i];
            }
            return value;
        }
        public static double[,] MatrixScale(double[,] tMatrix, double tscale)
        {
            PointD p2 = new PointD();

            double[,] MatrixNew = new double[tMatrix.GetLength(0), tMatrix.GetLength(1)];

            for (int i = 0; i < tMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < tMatrix.GetLength(1); j++)
                {
                    MatrixNew[i, j] = tscale * tMatrix[i, j];
                }
            }
            return MatrixNew;
        }

        public static double[,] IdentityMatrix(int tn)
        {
            double[,] tMatrix = new double[tn, tn];

            for (int i = 0; i < tn; i++)
            {
                for (int j = 0; j < tn; j++)
                {
                    tMatrix[i, j] = 0.0;
                }
            }

            for (int i = 0; i < tn; i++)
            {
                tMatrix[i, i] = 1.0;
            }
            return tMatrix;
        }
        public static double[,] ZeroMatrix(int dimension1, int dimention2)
        {
            double[,] matrix = new double[dimension1, dimention2];
            for (int i = 0; i < dimension1; i++)
            {
                for (int j = 0; j < dimention2; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            return matrix;
        }
        public static System.Array InitializedMatrix<T>(T value, params int[] dimensions)
        {
            //定義Array每個維度的起始索引值(起始值可以不為0)
            int[] lowerBounds = new int[dimensions.Length];
            for (int i = 0; i < lowerBounds.Length; i++)
                lowerBounds[i] = 0;
            //建立新多維Array
            System.Array newArray = System.Array.CreateInstance(typeof(T), dimensions, lowerBounds);
            //擺入元素
            int totalNum = 1;
            foreach (var item in dimensions)
                totalNum *= item;
            for (int index = 0; index < totalNum; index++)
            {
                int[] indices = new int[dimensions.Length];
                int residue = index;
                for (int i = 0; i < dimensions.Length; i++)
                {
                    int num = 1;
                    for (int j = i + 1; j < dimensions.Length; j++)
                        num *= dimensions[j];
                    indices[i] = residue / num;
                    residue -= indices[i] * num;
                }
                newArray.SetValue(Activator.CreateInstance(value.GetType()), indices);
                index++;
            }
            return newArray;
        }
        public static int[] ArrayGetDimension(System.Array Array)
        {
            int[] dimensions = new int[Array.Rank];
            for (int i = 0; i < Array.Rank; i++)
                dimensions[i] = Array.GetLength(i);
            return dimensions;
        }
        public static System.Array ArrayReshape(System.Array Matrix, params int[] dimensions)
        {
            System.Type elementType = Matrix.GetType().GetElementType();
            //定義Array每個維度的起始索引值(起始值可以不為0)
            int[] lowerBounds = new int[dimensions.Length];
            for (int i = 0; i < lowerBounds.Length; i++)
                lowerBounds[i] = 0;
            //建立新多維Array
            System.Array newArray = System.Array.CreateInstance(elementType, dimensions, lowerBounds);
            //擺入元素
            int index = 0;
            foreach (var item in Matrix)
            {
                int[] indices = new int[dimensions.Length];
                int residue = index;
                for (int i = 0; i < dimensions.Length; i++)
                {
                    int num = 1;
                    for (int j = i + 1; j < dimensions.Length; j++)
                        num *= dimensions[j];
                    indices[i] = residue / num;
                    residue -= indices[i] * num;
                }
                newArray.SetValue(item, indices);
                index++;
            }
            return newArray;
        }
        public static System.Array ArrayTake(System.Array Matrix, int[] startIndex, int[] endIndex)
        {
            System.Type elementType = Matrix.GetType().GetElementType();
            //定義Array每個維度的起始索引值(起始值可以不為0)
            int[] lowerBounds = new int[Matrix.Rank];
            for (int i = 0; i < lowerBounds.Length; i++)
                lowerBounds[i] = 0;

            //建立新多維Array
            int[] dimensions = new int[Matrix.Rank];
            for (int i = 0; i < Matrix.Rank; i++)
                dimensions[i] = endIndex[i] - startIndex[i] + 1;
            System.Array newArray = System.Array.CreateInstance(elementType, dimensions, lowerBounds);

            //Take
            int totalNum = 1;
            for (int i = 0; i < dimensions.Length; i++)
                totalNum *= dimensions[i];
            int index = 0;
            for (int n = 0; n < totalNum; n++)
            {
                int[] indices = new int[dimensions.Length];
                int residue = index;
                for (int i = 0; i < dimensions.Length; i++)
                {
                    int num = 1;
                    for (int j = i + 1; j < dimensions.Length; j++)
                        num *= dimensions[j];
                    indices[i] = residue / num;
                    residue -= indices[i] * num;
                }

                int[] orgIndices = new int[dimensions.Length];
                for (int i = 0; i < dimensions.Length; i++)
                    orgIndices[i] = startIndex[i] + indices[i];
                newArray.SetValue(Matrix.GetValue(orgIndices), indices);
                index++;
            }
            return newArray;
        }
        public static void ArrayPut(System.Array Matrix, System.Array TargetMatrix, int[] startIndex)
        {
            if (Matrix != null && TargetMatrix != null)
            {
                System.Type elementType = Matrix.GetType().GetElementType();
                //定義Array每個維度的起始索引值(起始值可以不為0)
                int[] lowerBounds = new int[Matrix.Rank];
                for (int i = 0; i < lowerBounds.Length; i++)
                    lowerBounds[i] = 0;

                //確認數目
                int[] dimensions = new int[Matrix.Rank];
                for (int i = 0; i < Matrix.Rank; i++)
                    dimensions[i] = Matrix.GetLength(i);

                //Take
                int totalNum = 1;
                for (int i = 0; i < dimensions.Length; i++)
                    totalNum *= dimensions[i];
                int index = 0;
                for (int n = 0; n < totalNum; n++)
                {
                    int[] indices = new int[dimensions.Length];
                    int residue = index;
                    for (int i = 0; i < dimensions.Length; i++)
                    {
                        int num = 1;
                        for (int j = i + 1; j < dimensions.Length; j++)
                            num *= dimensions[j];
                        indices[i] = residue / num;
                        residue -= indices[i] * num;
                    }

                    int[] orgIndices = new int[dimensions.Length];
                    for (int i = 0; i < dimensions.Length; i++)
                        orgIndices[i] = startIndex[i] + indices[i];
                    TargetMatrix.SetValue(Matrix.GetValue(indices), orgIndices);
                    index++;
                }
            }
        }

        public static double[,] RotateMatrix(Axis axis, double theta)
        {
            double[,] Mr = IdentityMatrix(4);
            switch (axis)
            {
                case Axis.X:
                    Mr[1, 1] = Cos(theta);
                    Mr[2, 2] = Cos(theta);
                    Mr[1, 2] = -Sin(theta);
                    Mr[2, 1] = Sin(theta);
                    break;
                case Axis.Y:
                    Mr[0, 0] = Cos(theta);
                    Mr[2, 2] = Cos(theta);
                    Mr[0, 2] = Sin(theta);
                    Mr[2, 0] = -Sin(theta);
                    break;
                case Axis.Z:
                    Mr[0, 0] = Cos(theta);
                    Mr[1, 1] = Cos(theta);
                    Mr[0, 1] = -Sin(theta);
                    Mr[1, 0] = Sin(theta);
                    break;
            }
            return Mr;
        }
        public static double[,] RotateMatrix(double thetax, double thetay, double thetaz)
        {
            double[,] MRx = RotateMatrix(Axis.X, thetax);
            double[,] MRy = RotateMatrix(Axis.Y, thetay);
            double[,] MRz = RotateMatrix(Axis.Z, thetaz);

            double[,] Mr = MatrixDot(MRx, MRy, MRz);

            return Mr;
        }
        public static double[,] RotateMatrix(double[] tRotateAxis, double theta)
        {
            double angleZ = Atan2(tRotateAxis[1], tRotateAxis[0]);
            double angleY = Atan2(tRotateAxis[2], Sqrt(tRotateAxis[0] * tRotateAxis[0] + tRotateAxis[1] * tRotateAxis[1]));
            double[,] A21 = RotateMatrix(Axis.Z, -angleZ);
            double[,] A32 = RotateMatrix(Axis.Y, -angleY);
            double[,] ARotate = RotateMatrix(Axis.X, theta);
            double[,] A23 = RotateMatrix(Axis.Y, angleY);
            double[,] A12 = RotateMatrix(Axis.Z, angleZ);

            return MatrixDot(A12, A23, ARotate, A32, A21);
        }

        public static double[] RotateX(double theta, double[] tr1)
        {
            double[,] m = RotateMatrix(Axis.X, theta);
            return Transport3(m, tr1);
        }
        public static double[] RotateY(double theta, double[] tr1)
        {
            double[,] m = RotateMatrix(Axis.Y, theta);
            return Transport3(m, tr1);
        }
        public static double[] RotateZ(double theta, double[] tr1)
        {
            double[,] m = RotateMatrix(Axis.Z, theta);
            return Transport3(m, tr1);
        }
        public static double[] Transport3(double[,] tMatrix, double[] tr1)
        {
            if (tMatrix != null)
            {
                double[] tr2 = new double[3];

                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tr2[i] = tr2[i] + tMatrix[i, k] * tr1[k];
                    }
                }
                return tr2;
            }
            return (double[])tr1.Clone();
        }
        public static double[] Transport4(double[,] tMatrix, double[] tr1)
        {
            if (tMatrix != null)
            {
                double[] tr2 = Transport3(tMatrix, tr1);
                tr2[0] += tMatrix[0, 3];
                tr2[1] += tMatrix[1, 3];
                tr2[2] += tMatrix[2, 3];
                return tr2;
            }
            return (double[])tr1.Clone();
        }
        public static T Transport<T>(double[,] tMatrix, T p) where T : IXYZ
        {
            double[] result;
            if (p.IsHomogeneous)
                result = Transport4(tMatrix, p.Values);
            else
                result = Transport3(tMatrix, p.Values);
            T newP = (T)p.New(result);
            return newP;
        }

        public static IEnumerable<double[]> RotateX(double theta, params double[][] tr1)
        {
            double[,] m = RotateMatrix(Axis.X, theta);
            return Transport3(m, tr1);
        }
        public static IEnumerable<double[]> RotateY(double theta, params double[][] tr1)
        {
            double[,] m = RotateMatrix(Axis.Y, theta);
            return Transport3(m, tr1);
        }
        public static IEnumerable<double[]> RotateZ(double theta, params double[][] tr1)
        {
            double[,] m = RotateMatrix(Axis.Z, theta);
            return Transport3(m, tr1);
        }
        public static IEnumerable<double[]> Transport3(double[,] tMatrix, params double[][] tr1)
        {
            foreach (var r in tr1)
            {
                yield return Transport3(tMatrix, r);
            }
        }
        public static IEnumerable<double[]> Transport4(double[,] tMatrix, params double[][] tr1)
        {
            foreach (var r in tr1)
            {
                yield return Transport4(tMatrix, r);
            }
        }
        public static IEnumerable<T> Transport<T>(double[,] tMatrix, params T[] p) where T : IXYZ
        {
            foreach (var r in p)
            {
                yield return Transport(tMatrix, r);
            }
        }

        public static double[,] MatrixInverse(double[,] dMatrix)
        {
            int Level = dMatrix.GetLength(0);
            double dMatrixValue = MatrixDeterminant(dMatrix);
            // 判斷行列式是否為0 ( < allowError )
            double allowError = 0.000000000001;
            if (dMatrixValue > -allowError && dMatrixValue < allowError)
                return null;

            double[,] dReverseMatrix = new double[Level, 2 * Level];
            double x, c;
            // Init Reverse matrix
            for (int i = 0; i < Level; i++)
            {
                for (int j = 0; j < 2 * Level; j++)
                {
                    if (j < Level)
                        dReverseMatrix[i, j] = dMatrix[i, j];
                    else
                        dReverseMatrix[i, j] = 0;
                }

                dReverseMatrix[i, Level + i] = 1;
            }

            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {
                if (dReverseMatrix[i, j] == 0)
                {
                    int m = i + 1;
                    for (; m <= Level - 1 && dReverseMatrix[m, j] == 0; m++) ;/////////////////////////////////////////錯誤修正20150205
                    if (m == Level)
                        throw new ArithmeticException();
                    else
                        // Add i-row with m-row
                        for (int n = j; n < 2 * Level; n++)
                            dReverseMatrix[i, n] += dReverseMatrix[m, n];
                }

                // Format the i-row with "1" start
                x = dReverseMatrix[i, j];
                if (x != 1)
                {
                    for (int n = j; n < 2 * Level; n++)
                        if (dReverseMatrix[i, n] != 0)
                            dReverseMatrix[i, n] /= x;
                }

                // Set 0 to the current column in the rows after current row
                for (int s = Level - 1; s > i; s--)
                {
                    x = dReverseMatrix[s, j];
                    for (int t = j; t < 2 * Level; t++)
                        dReverseMatrix[s, t] -= (dReverseMatrix[i, t] * x);
                }
            }

            // Format the first matrix into unit-matrix
            for (int i = Level - 2; i >= 0; i--)
            {
                for (int j = i + 1; j < Level; j++)
                    if (dReverseMatrix[i, j] != 0)
                    {
                        c = dReverseMatrix[i, j];
                        for (int n = j; n < 2 * Level; n++)
                            dReverseMatrix[i, n] -= (c * dReverseMatrix[j, n]);
                    }
            }

            double[,] dReturn = new double[Level, Level];
            for (int i = 0; i < Level; i++)
                for (int j = 0; j < Level; j++)
                    dReturn[i, j] = dReverseMatrix[i, j + Level];
            return dReturn;
        }
        public static double MatrixDeterminant(double[,] MatrixList)
        {
            int Level = MatrixList.GetLength(0);
            double[,] dMatrix = new double[Level, Level];
            for (int i = 0; i < Level; i++)
                for (int j = 0; j < Level; j++)
                    dMatrix[i, j] = MatrixList[i, j];
            double c, x;
            int k = 1;
            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {
                if (dMatrix[i, j] == 0)
                {
                    int m = i;
                    for (; m < Level && dMatrix[m, j] == 0; m++) ;//////////////////////////////錯誤修正20150122
                    if (m == Level)
                        return 0;
                    else
                    {
                        // Row change between i-row and m-row
                        for (int n = j; n < Level; n++)
                        {
                            c = dMatrix[i, n];
                            dMatrix[i, n] = dMatrix[m, n];
                            dMatrix[m, n] = c;
                        }

                        // Change value pre-value
                        k *= (-1);
                    }
                }

                // Set 0 to the current column in the rows after current row
                for (int s = Level - 1; s > i; s--)
                {
                    x = dMatrix[s, j];
                    for (int t = j; t < Level; t++)
                        dMatrix[s, t] -= dMatrix[i, t] * (x / dMatrix[i, j]);
                }
            }

            double sn = 1;
            for (int i = 0; i < Level; i++)
            {
                if (dMatrix[i, i] != 0)
                    sn *= dMatrix[i, i];
                else
                    return 0;
            }
            return k * sn;
        }
        public static T[,] MatrixTranspose<T>(T[,] Matrix)
        {
            int r1, c1;
            r1 = Matrix.GetUpperBound(0) + 1;  //UBound
            c1 = Matrix.GetUpperBound(1) + 1;  // UBound
            T[,] Trans = new T[c1, r1];
            for (int ii = 0; ii < r1; ii++)
                for (int jj = 0; jj < c1; jj++)
                    Trans[jj, ii] = Matrix[ii, jj];
            return Trans;
        }
        #endregion

        #region 其他
        public static double Sphive(double tphi, double tgammab)
        {
            double tsphive;
            tsphive = System.Math.Atan(System.Math.Tan(tphi) * System.Math.Sin(tgammab)) / System.Math.Sin(tgammab) - tphi;
            return tsphive;
        }
        public static double Involute(double alpha)
        {

            return (System.Math.Tan(alpha) - alpha);

        }
        public static double Pow(double tx, double td)
        {
            return System.Math.Pow(tx, td);
        }
        public static double Floor(double x, double a)
        {
            double xx = x / a;
            int xx1 = (int)xx;
            return xx1 * a;
        }
        public static double Sqrt(double ta)
        {
            return System.Math.Sqrt(ta);
        }
        public static double PI
        {
            get { return System.Math.PI; }
        }
        public static double Abs(double ta)
        {
            return System.Math.Abs(ta);
        }
        public static double Round(double ta, double tb)
        {
            return System.Math.Round(ta / tb) * tb;
        }
        public static double Mod(double ta, double tb)
        {
            double tc = ta - Floor(ta / tb, 1.0) * tb;
            return tc;
        }
        public static bool Compare(double a, double b, double precision)
        {
            return Abs(a - b) <= precision;
        }
        public static void Compare_Boundary(XYZ4[] Boundary, XYZ4 point2Check)
        {
            if (point2Check.X > Boundary[1].X) { Boundary[1].X = point2Check.X; }
            else if (point2Check.X < Boundary[0].X) { Boundary[0].X = point2Check.X; }
            if (point2Check.Y > Boundary[1].Y) { Boundary[1].Y = point2Check.Y; }
            else if (point2Check.Y < Boundary[0].Y) { Boundary[0].Y = point2Check.Y; }
            if (point2Check.Z > Boundary[1].Z) { Boundary[1].Z = point2Check.Z; }
            else if (point2Check.Z < Boundary[0].Z) { Boundary[0].Z = point2Check.Z; }
        }
        public static int Sign(double values)
        {
            return System.Math.Sign(values);
        }
        public static IEnumerable<int> EachSign(IEnumerable<double> values)
        {
            foreach (var item in values)
            {
                yield return Sign(item);
            }
        }
        #endregion
    }
}
