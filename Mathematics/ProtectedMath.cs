using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PTL.Geometry;
using PTL.Definitions;
using PTL.Geometry.MathModel;
using PTL.Exceptions;

namespace PTL.Mathematics
{
    public class ProtectedPTLM
    {
        protected static double delChk = 0.002;
        #region 三角函數
        protected static double Cos(double trad)
        {
            double tvalue;
            tvalue = System.Math.Cos(trad);
            return tvalue;
        }
        protected static double Sin(double trad)
        {
            double tvalue;
            tvalue = System.Math.Sin(trad);
            return tvalue;
        }
        protected static double Tan(double trad)
        {
            double tvalue;
            tvalue = System.Math.Tan(trad);
            return tvalue;
        }
        protected static double Sec(double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Cos(trad);
            return tvalue;
        }
        protected static double Acos(double tv)
        {
            double tvalue;
            tvalue = System.Math.Acos(tv);
            return tvalue;
        }
        protected static double Atan2(double y, double x)
        {
            double tvalue;
            tvalue = System.Math.Atan2(y, x);
            return tvalue;
        }
        protected static double Atan(double tv)
        {
            double tvalue;
            tvalue = System.Math.Atan(tv);
            return tvalue;
        }
        protected static double Asin(double tv)
        {
            double tvalue;
            tvalue = System.Math.Asin(tv);
            return tvalue;
        }
        protected static double Asec(double tv)
        {
            double tvalue;
            tvalue = System.Math.Acos(1.0 / tv);
            return tvalue;
        }
        protected static double Csc(double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Sin(trad);
            return tvalue;
        }
        protected static double Cot(double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Tan(trad);
            return tvalue;
        }
        #endregion

        #region 單位轉換
        protected static double DegToRad(double tdeg)
        {
            double trad;
            trad = tdeg / 180.0 * System.Math.PI;
            return trad;
        }
        protected static double RadToDeg(double trad)
        {
            double tdeg;
            tdeg = trad / System.Math.PI * 180.0;
            return tdeg;
        }
        protected static double mmToInch(double tmm)
        {
            double tInch;
            tInch = tmm / 25.4;
            return tInch;
        }
        protected static double InchTomm(double tInch)
        {
            double tmm;
            tmm = tInch * 25.4;
            return tmm;
        }
        protected static string DegToDMS(double x)
        {
            int x1 = (int)x;
            int x2 = (int)((x - x1) * 60);
            int x3 = (int)((((x - x1) * 60) - x2) * 60);
            string ss = "";
            ss = String.Format("{0}", x1) + "d" + String.Format("{0,3}", x2) + "m" + String.Format("{0,3}", x3) + "s";
            return ss;
        }
        protected static string DegToDMS(double x,short n)
        {
            double absx = Abs(x);
            int x1 = (int)absx;
            int x2 = (int)((absx - x1) * 60);
            int x3 = (int)((((absx - x1) * 60) - x2) * 60);
            string ss= "";

            if (n == 3)
            {
                ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
            }
            if (n == 2)
            {
                if (x1!=0)
                    ss = String.Format("{0,3}", x1) + "D" + String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
                else
                    ss = String.Format("{0,3}", x2) + "M" + String.Format("{0,3}", x3) + "S";
            }
            if (n ==1)
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
                ss ="-  " + ss;

            return ss;
        }
        protected static string RadToDMS(double y)
        {
            string ss = DegToDMS(RadToDeg(y));
            return ss;
        }
        protected static string RadToDMS(double y,short n)
        {
            string ss = DegToDMS(RadToDeg(y),n);
            return ss;
        }
        protected static double DMSToDeg(double dd, double mm, double ss)
        {
            double deg = dd + mm / 60.0 + ss / 3600.0;
            return deg;
        }
        protected static double DMSToRad(double dd, double mm, double ss)
        {
            double rad = DegToRad(DMSToDeg(dd, mm, ss));
            return rad;
        }
        #endregion

        #region Matrix & Vector
        protected static double[] Normalize(double[] r)
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
        protected static double GetLength(double[] input)
        {
            double sum = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                sum = sum + System.Math.Pow(input[i], 2);
            }
            double len = Sqrt(sum);

            return len;
        }
        protected static double Norm(double[] input)
        {
            double sum = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                sum = sum + System.Math.Pow(input[i], 2);
            }
            double len = Sqrt(sum);

            return len;
        }
        protected static XYZ3 GetAnyNormal(XYZ3 direction)
        {
            return Mathematics.PTLM.GetAnyNormal(direction);
        }

        protected static double[] MatrixDot(double[] array1, double[] array2)
        {
            if (array1 != null && array2 != null)
            {
                int r1 = array1.Length;  // UBound

                double[] outptr = new double[r1];
                for (int i = 0; i < r1; i++)
                {
                    outptr[i] = array1[i] * array2[i];
                }
                return outptr;
            }
            else if (array1 != null)
                return array1;
            else if (array2 != null)
                return array2;
            return null;
        }
        protected static double[] MatrixDot(double[] array1, double[,] array2)
        {
            if (array1 != null && array2 != null)
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
            else if (array1 != null)
                return array1;
            return null;
        }
        protected static double[] MatrixDot(double[,] array1, double[] array2)
        {
            if (array1 != null && array2 != null)
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
            else if (array2 != null)
                return array2;
            return null;
        }
        protected static double[,] MatrixDot(double[,] array1, double[,] array2)
        {
            if (array1 != null && array2 != null)
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
            else if (array1 != null)
                return array1;
            else if (array2 != null)
                return array2;
            return null;
        }
        protected static double[,] MatrixDot(params double[][,] arrarys)
        {
            foreach (var item in arrarys)
                if (item == null)
                    return null;
            for (int i = 0; i < arrarys.Length - 1; i++)
                if (arrarys[i].GetLength(1) != arrarys[i + 1].GetLength(0))
                    throw new ArraySizeMismatchException();

            double[,] outputArray = arrarys[0];
            for (int i = 1; i < arrarys.Length; i++)
            {
                outputArray = MatrixDot(outputArray, arrarys[i]);
            }
            return outputArray;
        }
        protected static double[,] MatrixAdd(params double[][,] arrarys)
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
        protected static double[] Cross(double[] ptr1, double[] ptr2)
        {
            double[] outptr = new double[3];
            outptr[0] = ptr1[1] * ptr2[2] - ptr1[2] * ptr2[1];
            outptr[1] = ptr1[2] * ptr2[0] - ptr1[0] * ptr2[2];
            outptr[2] = ptr1[0] * ptr2[1] - ptr1[1] * ptr2[0];
            return outptr;
        }
        protected static double Dot(double[] ptr1, double[] ptr2)
        {
            double value = 0.0;

            for (int i = 0; i <= ptr1.GetUpperBound(0); i++)
            {
                value = value + ptr1[i] * ptr2[i];
            }
            return value;
        }
        protected static double[,] MatrixScale(double[,] tMatrix, double tscale)
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

        protected static double[,] IdentityMatrix(int tn)
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
        protected static double[,] ZeroMatrix(int dimension1, int dimention2)
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
        protected static System.Array InitializedMatrix<T>(T value, params int[] dimensions)
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
        protected static int[] ArrayGetDimension(System.Array Array)
        {
            int[] dimensions = new int[Array.Rank];
            for (int i = 0; i < Array.Rank; i++)
                dimensions[i] = Array.GetLength(i);
            return dimensions;
        }
        protected static System.Array ArrayReshape(System.Array Matrix, params int[] dimensions)
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
        protected static System.Array ArrayTake(System.Array Matrix, int[] startIndex, int[] endIndex)
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
        protected static void ArrayPut(System.Array Matrix, System.Array TargetMatrix, int[] startIndex)
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
        protected static Array ArratJoin(Array arr1, Array arr2, int dim = 0)
        {
            return PTLM.ArratJoin(arr1, arr2, dim);
        }

        protected static double[,] RotateMatrix(Axis axis, double theta)
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
        protected static double[,] RotateMatrix(double thetax, double thetay, double thetaz)
        {
            double[,] MRx = RotateMatrix(Axis.X, thetax);
            double[,] MRy = RotateMatrix(Axis.Y, thetay);
            double[,] MRz = RotateMatrix(Axis.Z, thetaz);

            double[,] Mr = MatrixDot(MRx, MRy, MRz);

            return Mr;
        }
        protected static double[,] RotateMatrix(double[] tRotateAxis, double theta)
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

        protected static double[] RotateX(double theta, double[] tr1)
        {
            double[,] m = RotateMatrix(Axis.X, theta);
            return Transport3(m, tr1);
        }
        protected static double[] RotateY(double theta, double[] tr1)
        {
            double[,] m = RotateMatrix(Axis.Y, theta);
            return Transport3(m, tr1);
        }
        protected static double[] RotateZ(double theta, double[] tr1)
        {
            double[,] m = RotateMatrix(Axis.Z, theta);
            return Transport3(m, tr1);
        }
        protected static double[] Transport3(double[,] tMatrix, double[] tr1)
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
        protected static double[] Transport4(double[,] tMatrix, double[] tr1)
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
        protected static T Transport<T>(double[,] tMatrix, T p) where T : IXYZ
        {
            double[] result;
            if (p.IsHomogeneous == true)
                result = Transport4(tMatrix, p.Values);
            else
                result = Transport3(tMatrix, p.Values);
            T newP = (T)p.New(result);
            return newP;
        }

        protected static IEnumerable<double[]> RotateX(double theta, params double[][] tr1)
        {
            double[,] m = RotateMatrix(Axis.X, theta);
            return Transport3(m, tr1);
        }
        protected static IEnumerable<double[]> RotateY(double theta, params double[][] tr1)
        {
            double[,] m = RotateMatrix(Axis.Y, theta);
            return Transport3(m, tr1);
        }
        protected static IEnumerable<double[]> RotateZ(double theta, params double[][] tr1)
        {
            double[,] m = RotateMatrix(Axis.Z, theta);
            return Transport3(m, tr1);
        }
        protected static IEnumerable<double[]> Transport3(double[,] tMatrix, params double[][] tr1)
        {
            foreach (var r in tr1)
            {
                yield return Transport3(tMatrix, r);
            }
        }
        protected static IEnumerable<double[]> Transport4(double[,] tMatrix, params double[][] tr1)
        {
            foreach (var r in tr1)
            {
                yield return Transport4(tMatrix, r);
            }
        }
        protected static IEnumerable<T> Transport<T>(double[,] tMatrix, params T[] p) where T : IXYZ
        {
            foreach (var r in p)
            {
                yield return Transport(tMatrix, r);
            }
        }

        protected static double[,] MatrixInverse(double[,] dMatrix)
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
        protected static double MatrixDeterminant(double[,] MatrixList)
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
        protected static T[,] MatrixTranspose<T>(T[,] Matrix)
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

        #region 基本工具
        protected static double Sphive(double tphi, double tgammab)
        {
            double tsphive;
            tsphive = System.Math.Atan(System.Math.Tan(tphi) * System.Math.Sin(tgammab)) / System.Math.Sin(tgammab) - tphi;
            return tsphive;
        }
        protected static double Involute(double alpha)
        {

            return (System.Math.Tan(alpha) - alpha);

        }
        protected static double Pow(double tx, double td)
        {
            return System.Math.Pow(tx, td);
        }
        protected static double Floor(double x, double a = 1)
        {
            return PTLM.Floor(x, a);
        }
        protected static double Ceiling(double x, double a = 1)
        {
            return PTLM.Ceiling(x, a);
        }
        protected static double Sqrt(double ta)
        {
            return System.Math.Sqrt(ta);
        }
        protected static double PI
        {
            get { return System.Math.PI; }
        }
        protected static double Abs(double ta)
        {
            return System.Math.Abs(ta);
        }
        protected static double Round(double ta, double tb)
        {
            return System.Math.Round(ta / tb) * tb;
        }
        protected static double Mod(double ta, double tb)
        {
            double tc = ta - Floor(ta / tb, 1.0) * tb;
            return tc;
        }
        protected static PointD CalcuCenterXY(PointD p1,PointD p2, double radius, short ctype)
        {
            double ra = Abs(radius);
            short Flag = (short)(radius / ra);
            PointD za = new PointD(0.0,0.0,1.0);
            PointD v1 = Normalize(p2-p1);
            PointD v2 = Cross(v1,za);
            double dist = Norm(p2 - p1);
            double xx = Sqrt(ra*ra - (dist / 2.0) * (dist / 2.0));

             PointD pc;
            if(ctype==2)
                pc= p1 + dist / 2.0 * v1 + Flag * xx * v2;
            else
                pc = p1 + dist / 2.0 * v1 - Flag * xx * v2;

            return pc;
        }
        protected static bool Compare(double a, double b, double precision)
        {
            return Abs(a - b) <= precision;
        }
        protected static void Compare_Boundary(XYZ4[] Boundary, XYZ4 point2Check)
        {
            if (point2Check.X > Boundary[1].X) { Boundary[1].X = point2Check.X; }
            else if (point2Check.X < Boundary[0].X) { Boundary[0].X = point2Check.X; }
            if (point2Check.Y > Boundary[1].Y) { Boundary[1].Y = point2Check.Y; }
            else if (point2Check.Y < Boundary[0].Y) { Boundary[0].Y = point2Check.Y; }
            if (point2Check.Z > Boundary[1].Z) { Boundary[1].Z = point2Check.Z; }
            else if (point2Check.Z < Boundary[0].Z) { Boundary[0].Z = point2Check.Z; }
        }
        protected static int Sign(double values)
        {
            return System.Math.Sign(values);
        }
        protected static IEnumerable<int> EachSign(IEnumerable<double> values)
        {
            foreach (var item in values)
            {
                yield return ProtectedPTLM.Sign(item);
            }
        }
        #endregion

        protected static class NonlinearFindRoot
        {
            public static string nError = "";
            public static TimeSpan TimeSpanLimit = new TimeSpan(0, 0, 5);
            public delegate void EquationSet(double[] input, double[] output);

            /// <summary>
            /// return item1:root item2:{times up, check}
            /// </summary>
            /// <param name="Equation"></param>
            /// <param name="InitialGuest"></param>
            /// <returns></returns>
            public static Tuple<double, bool[]> FindRoot(Func<double, double> Equation, double InitialGuest)
            {
                double[] x = new double[] { InitialGuest };
                EquationSet aEquationSet = (double[] xx, double[] yy) => { yy[0] = Equation(xx[0]); };
                Boolean[] Result = FindRoot(x, aEquationSet);
                return new Tuple<double, bool[]>(x[0], Result);
            }

            /// <summary>
            /// return item1:root item2:{times up, check}
            /// </summary>
            /// <param name="Equation"></param>
            /// <param name="InitialGuest"></param>
            /// <returns></returns>
            public static Tuple<double[], bool[]> FindRoot(Func<double[], double[]> Equation, double[] InitialGuest)
            {
                double[] x = new double[InitialGuest.Length];
                InitialGuest.CopyTo(x, 0);
                EquationSet aEquationSet = (double[] xx, double[] yy) => 
                    {
                        double[] current = Equation(xx);
                        yy[0] = current[0];
                        yy[1] = current[1];
                    };
                Boolean[] Result = FindRoot(x, aEquationSet);
                return new Tuple<double[],bool[]> (x, Result);
            }
            public static bool[] FindRoot(double[] x, EquationSet objEquationSet)
            {
                bool Check = false;
                bool timesUP = newt(x, objEquationSet, Check);
                bool[] states = new bool[]{timesUP, Check};

                return states;
            }
            private static bool newt(double[] x, EquationSet objEquationSet, bool Check)
            {
                const int MAXITS = 200;
                const double TOLF = 1E-08;
                const double TOLMIN = 1E-10;
                const int STPMX = 100;
                const double TOLX = 1E-11;

                bool timsUP = false;

                double den, f, fold, stpmax, sum, temp, test;
                double d = 0;
                int n = x.Length;
                int[] indx = new int[n];
                double[] g = new double[n];
                double[] p = new double[n];
                double[] xold = new double[n];
                double[,] fjac = new double[n, n];
                double[] fvec = new double[n];

                f = fmin(x, fvec, objEquationSet);
                test = 0.0;
                for (int i = 0; i < n; i++)
                    if ((Abs(fvec[i]) > test)) test = Abs(fvec[i]);
                if (test < 0.01 * TOLF)
                {
                    Check = false;
                    return timsUP;
                }
                sum = 0;
                for (int i = 0; i < n; i++) sum = sum + x[i] * x[i];
                stpmax = STPMX * Max(Sqrt(sum), Convert.ToDouble(n));
                for (int its = 0; its < MAXITS; its++)
                {
                    fdjac(x, fvec, fjac, objEquationSet);
                    for (int i = 0; i < n; i++)
                    {
                        sum = 0;
                        for (int j = 0; j < n; j++) sum = sum + fjac[j, i] * fvec[j];
                        g[i] = sum;
                    }
                    for (int i = 0; i < n; i++) xold[i] = x[i];
                    fold = f;
                    for (int i = 0; i < n; i++) p[i] = -fvec[i];
                    ludcmp(fjac, indx, d);
                    lubksb(fjac, indx, p);
                    timsUP = lnsrch(xold, fold, g, p, x, f, stpmax, Check, fvec, objEquationSet);
                    if (timsUP)
                    {
                        x[0] = 0;
                        return timsUP;
                    } 
                    test = 0.0;
                    for (int i = 0; i < n; i++)
                        if (Abs(fvec[i]) > test) test = Abs(fvec[i]);
                    if (test < TOLF)
                    {
                        Check = false;
                        return timsUP;
                    }
                    if (Check)
                    {
                        test = 0.0;
                        den = Max(f, 0.5 * n);
                        for (int i = 0; i < n; i++)
                        {
                            temp = Abs(g[i]) * Max(Abs(x[i]), 1.0) / den;
                            if (temp > test) test = temp;
                        }
                        Check = (test < TOLMIN);
                        return timsUP;
                    }
                    test = 0.0;
                    for (int i = 0; i < n; i++)
                    {
                        temp = (Abs(x[i] - xold[i])) / Max(Abs(x[i]), 1.0);
                        if (temp > test) test = temp;
                    }
                    if (test < TOLX) return timsUP;
                }
                return timsUP;
            }
            private static void ludcmp(double[,] a, int[] indx, double d)
            {
                const double TINY = 1E-20;
                int imax = 0;
                double big, dum, sum, temp;

                int n = a.GetLength(0);
                double[] vv = new double[n];

                d = 1;

                for (int i = 0; i < n; i++)
                {
                    big = 0;
                    for (int j = 0; j < n; j++)
                        if ((temp = Abs(a[i, j])) > big) big = temp;
                    if (big == 0) nError = "Singular matrix in routine ludcmp";
                    vv[i] = 1 / big;
                }
                for (int j = 0; j < n; j++)
                {
                    for (int i = 0; i < j; i++)
                    {
                        sum = a[i, j];
                        for (int k = 0; k < i; k++) sum = sum - a[i, k] * a[k, j];
                        a[i, j] = sum;
                    }
                    big = 0.0;
                    for (int i = j; i < n; i++)
                    {
                        sum = a[i, j];
                        for (int k = 0; k < j; k++) sum = sum - a[i, k] * a[k, j];
                        a[i, j] = sum;
                        dum = vv[i] * Abs(sum);
                        if ((dum = vv[i] * Abs(sum)) >= big)
                        {
                            big = dum;
                            imax = i;
                        }
                    }
                    if (j != imax)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            dum = a[imax, k];
                            a[imax, k] = a[j, k];
                            a[j, k] = dum;
                        }
                        d = -d;
                        vv[imax] = vv[j];
                    }
                    indx[j] = imax;
                    if (a[j, j] == 0) a[j, j] = TINY;

                    if (j != n - 1)
                    {
                        dum = 1.0 / a[j, j];
                        for (int i = j + 1; i < n; i++) a[i, j] = a[i, j] * dum;
                    }
                }
            }
            private static void lubksb(double[,] a, int[] indx, double[] b)
            {
                int ii = 0, ip;
                double sum;

                int n = a.GetLength(0);
                for (int i = 0; i < n; i++)
                {
                    ip = indx[i];
                    sum = b[ip];
                    b[ip] = b[i];
                    if (ii != 0)
                        for (int j = ii - 1; j < i; j++) sum = sum - a[i, j] * b[j];
                    else if (sum != 0) ii = i + 1;
                    b[i] = sum;
                }
                for (int i = n - 1; i >= 0; i--)
                {
                    sum = b[i];
                    for (int j = i + 1; j < n; j++) sum = sum - a[i, j] * b[j];
                    b[i] = sum / a[i, i];
                }
            }
            private static bool lnsrch(double[] xold, double fold, double[] g, double[] p, double[] x, double f, double stpmax, bool Check, double[] fvec, EquationSet objEquationSet)
            {
                const double ALF = 1E-08;
                const double TOLX = 1E-11;

                double a, alam, alam2 = 0.0, alamin, b, disc, f2 = 0.0;
                double rhs1, rhs2, slope, sum, temp, test, tmplam;

                int n = xold.Length;
                Check = false;
                sum = 0.0;
                for (int i = 0; i < n; i++) sum = sum + p[i] * p[i];
                sum = Sqrt(sum);
                if (sum > stpmax)
                    for (int i = 0; i < n; i++) p[i] = p[i] * stpmax / sum;
                slope = 0.0;
                for (int i = 0; i < n; i++) slope = slope + g[i] * p[i];
                if (slope >= 0.0) nError = "Roundoff problem in lnsrch.";
                test = 0.0;
                for (int i = 0; i < n; i++)
                {
                    temp = Abs(p[i]) / Max(Abs(xold[i]), 1);
                    if (temp > test) test = temp;
                }
                alamin = TOLX / test;
                alam = 1.0;
                DateTime startT = DateTime.Now;
                int i_times = 0;
                while(true)
                {
                    if (i_times == 100)
                    {
                        i_times = 0;
                        if (DateTime.Now - startT > TimeSpanLimit)
                        {
                            Debug.WriteLine("NonLinear Find Root Overtime({0}s)", TimeSpanLimit.Seconds);
                            return true;
                        }
                    }

                    for (int i = 0; i < n; i++) x[i] = xold[i] + alam * p[i];
                    f = fmin(x, fvec, objEquationSet);
                    if (alam < alamin)
                    {
                        for (int i = 0; i < n; i++) x[i] = xold[i];
                        Check = true;
                        return false;
                    }
                    else if (f <= fold + ALF * alam * slope) return false;
                    else
                    {
                        if (alam == 1.0) tmplam = -slope / (2.0 * (f - fold - slope));
                        else
                        {
                            rhs1 = f - fold - alam * slope;
                            rhs2 = f2 - fold - alam2 * slope;
                            a = (rhs1 / (alam * alam) - rhs2 / (alam2 * alam2)) / (alam - alam2);
                            b = (-alam2 * rhs1 / (alam * alam) + alam * rhs2 / (alam2 * alam2)) / (alam - alam2);

                            if (a == 0.0) tmplam = -slope / (2.0 * b);
                            else
                            {
                                disc = b * b - 3.0 * a * slope;
                                if (disc < 0.0) tmplam = 0.5 * alam;
                                else if (b <= 0.0) tmplam = (-b + Sqrt(disc)) / (3.0 * a);
                                else tmplam = -slope / (b + Sqrt(disc));
                            }
                            if (tmplam > 0.5 * alam) tmplam = 0.5 * alam;
                        }
                    }

                    alam2 = alam;
                    f2 = f;
                    alam = Max(tmplam, 0.1 * alam);

                    i_times++;
                }
            }
            private static void fdjac(double[] x, double[] fvec, double[,] df, EquationSet objEquationSet)
            {
                const double EPS = 1E-08;

                double h, temp;

                int n = x.Length;
                double[] f = new double[n];

                for (int j = 0; j < n; j++)
                {
                    temp = x[j];
                    h = EPS * Abs(temp);
                    if (h == 0) h = EPS;
                    x[j] = temp + h;
                    h = x[j] - temp;
                    objEquationSet(x, f);
                    x[j] = temp;
                    for (int i = 0; i < n; i++) df[i, j] = (f[i] - fvec[i]) / h;
                }
            }
            private static double fmin(double[] x, double[] fvec, EquationSet objEquationSet)
            {
                objEquationSet(x, fvec);

                int n = x.Length;
                double sum = 0.0;
                for (int i = 0; i < n; i++) sum = sum + fvec[i] * fvec[i];
                return 0.5 * sum;
            }
            private static double Max(double a, double b)
            {
                double tMax = a;
                if (tMax < b) tMax = b;
                return tMax;
            }
        }

        protected static class Differential
        {
            public static double Mid2PDiff(Func<double, double> f, double xi, double h)
            {
                double dfdx = (f(xi - h) - f(xi + h)) / 2 / h;
                return dfdx;
            }
        }

        protected static class Integration
        {
            const int JMAX = 20, JMAXP = JMAX + 1, K = 5;
            const double EPS = 1.0e-10;
            public delegate double Equation(double input);


            static double s = 0;

            public static double trapzd(Equation func, double a, double b, int n)
            {
                double x, tnm, sum, del;
                //double s=0;

                int it, j;

                if (n == 1)
                {
                    return (s = 0.5 * (b - a) * (func(a) + func(b)));
                }
                else
                {
                    for (it = 1, j = 1; j < n - 1; j++) it <<= 1;
                    tnm = it;
                    del = (b - a) / tnm;
                    x = a + 0.5 * del;
                    for (sum = 0.0, j = 0; j < it; j++, x += del) sum += func(x);
                    s = 0.5 * (s + (b - a) * sum / tnm);
                    return s;
                }
            }

            public static void polint(double[] xa, double[] ya, double x, ref double y, ref double dy)
            {
                int i, m, ns = 0;
                double den, dif, dift, ho, hp, w;

                int n = xa.Length;
                double[] c = new double[n];
                double[] d = new double[n];

                dif = Abs(x - xa[0]);
                for (i = 0; i < n; i++)
                {
                    if ((dift = Abs(x - xa[i])) < dif)
                    {
                        ns = i;
                        dif = dift;
                    }
                    c[i] = ya[i];
                    d[i] = ya[i];
                }
                y = ya[ns--];
                for (m = 1; m < n; m++)
                {
                    for (i = 0; i < n - m; i++)
                    {
                        ho = xa[i] - x;
                        hp = xa[i + m] - x;
                        w = c[i + 1] - d[i];
                        den = ho - hp;
                        //if ((den=ho-hp) == 0.0) nrerror("Error in routine polint");
                        den = w / den;
                        d[i] = hp * den;
                        c[i] = ho * den;
                    }
                    y += (dy = (2 * (ns + 1) < (n - m) ? c[ns + 1] : d[ns--]));
                }
            }

            public static double qromb(Equation func, double a, double b)
            {

                double ss = 0, dss = 0;
                double[] s = new double[JMAX];
                double[] h = new double[JMAXP];
                double[] s_t = new double[K];
                double[] h_t = new double[K];
                int i, j;

                h[0] = 1.0;
                for (j = 1; j <= JMAX; j++)
                {
                    s[j - 1] = trapzd(func, a, b, j);
                    if (j >= K)
                    {
                        for (i = 0; i < K; i++)
                        {
                            h_t[i] = h[j - K + i];
                            s_t[i] = s[j - K + i];
                        }
                        polint(h_t, s_t, 0.0, ref ss, ref dss);
                        if (Abs(dss) <= EPS * Abs(ss)) return ss;
                    }
                    h[j] = 0.25 * h[j - 1];
                }
                //nrerror("Too many steps in routine qromb");
                return 0.0;
            }
        }
    }

    
}