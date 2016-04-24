using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using PTL.Exceptions;
using PTL.Definitions;
using PTL.Geometry.MathModel;

namespace PTL.Mathematics
{
    public class MatrixFunctions
    {
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
        public static double[] GetAnyNormal(double[] direction)
        {
            if (direction.Length != 3)
                throw new NotImplementedException("No implement for which vector length is not 3.");

            double[] n1 = Cross(new double[] { 0.0, 0.0, 1.0 }, direction);
            if (Norm(n1) < 1e-5)
                n1 = Cross(new double[] { 0.0, 1.0, 0.0 }, direction);
            n1 = Normalize(n1);
            return n1;
        }

        public static double[] Mult(double[] array1, double[] array2)
        {
            if (array1 != null && array2 != null)
            {
                int r1 = array1.Length;
                int r2 = array2.Length;

                if (r1 != r2)
                    throw new ArrayDimensionMismatchException(
                        String.Format("Array1 Length is : {0} ; Array2 Length is : {1}", r1, r2));

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
        public static double[,] Mult(double[,] tMatrix, double tscale)
        {
            int r1 = tMatrix.GetLength(0);
            int c1 = tMatrix.GetLength(1);
            double[,] MatrixNew = new double[r1, c1];

            unsafe
            {
                fixed(double* arr = tMatrix, opt = MatrixNew)
                {
                    for (int i = 0; i < r1; i++)
                    {
                        for (int j = 0; j < c1; j++)
                        {
                            opt[i * c1 + j] = tscale * arr[i * c1 + j];
                        }
                    }
                }
            }
            
            return MatrixNew;
        }
        public static double[] Cross(double[] ptr1, double[] ptr2)
        {
            double[] outptr = new double[3];
            outptr[0] = ptr1[1] * ptr2[2] - ptr1[2] * ptr2[1];
            outptr[1] = ptr1[2] * ptr2[0] - ptr1[0] * ptr2[2];
            outptr[2] = ptr1[0] * ptr2[1] - ptr1[1] * ptr2[0];
            return outptr;
        }
        public static double[,] Add(params double[][,] arrarys)
        {
            for (int i = 0; i < arrarys.Length - 1; i++)
                if (arrarys[i].GetLength(0) != arrarys[i + 1].GetLength(0) && arrarys[i].GetLength(1) != arrarys[i + 1].GetLength(1))
                    return null;

            int r1, c1;
            r1 = arrarys[0].GetLength(0);  // UBound
            c1 = arrarys[0].GetLength(1);  // UBound
            
            double[,] outputArray = NewZeroMatrix(r1, c1);

            unsafe
            {
                fixed (double* opt = outputArray)
                {
                    for (int k = 0; k < arrarys.Length; k++)
                    {
                        fixed (double* arr = arrarys[k])
                        {
                            for (int i = 0; r1 > i; i++)
                            {
                                for (int j = 0; c1 > j; j++)
                                {
                                    opt[i * c1 + j] += arr[i * c1 + j];
                                }
                            }
                        }
                    }
                }
            }
            
            return outputArray;
        }
        public static double[] Dot(double[,] array1, double[] array2)
        {
            if (array1 != null && array2 != null)
            {
                int r1, c1, r2, i, k;
                r1 = array1.GetLength(0);
                c1 = array1.GetLength(1);
                r2 = array2.GetLength(0);

                if (c1 != r2)
                    throw new ArrayDimensionMismatchException(
                        String.Format("Array1 Dimension is : {{0},{1}} ; Array2 Length is : {2}", c1, r2, r2));

                double[] outptr = new double[r1];

                unsafe
                {
                    fixed(double* arr1 = array1, arr2 = array2, opt= outptr)
                    {
                        for (i = 0; i < r1; i++)
                        {
                            double res = 0;
                            for (k = 0; k < c1; k++)
                            {
                                res += arr1[i * c1 + k] * arr2[k];
                            }
                            opt[i] = res;
                        }
                    }
                }
                
                return outptr;
            }
            else if (array2 != null)
                return array2;
            return null;
        }
        public static double[] Dot(double[] array1, double[,] array2)
        {
            if (array1 != null && array2 != null)
            {
                int c1 = array1.Length;
                int r2 = array2.GetLength(0);
                int c2 = array2.GetLength(1);

                if (c1 != r2)
                    throw new ArrayDimensionMismatchException(
                        String.Format("Array1 Length is : {0} ; Array2 Dimension is : {{1},{2}}", c1, r2, c2));

                double[] outptr = new double[c2];

                unsafe
                {
                    fixed(double* arr1= array1, arr2 = array2, opt = outptr)
                    {
                        for (int i = 0; i < c2; i++)
                        {
                            double res = 0;
                            for (int j = 0; j < r2; j++)
                            {
                                res += arr1[j] * arr2[j * c2 + i];
                            }
                            opt[i] = res;
                        }
                    }       
                }
                return outptr;
            }
            else if (array1 != null)
                return array1;
            return null;
        }
        public static double[,] Dot(double[,] array1, double[,] array2)
        {
            if (array1 != null && array2 != null)
            {
                int r1 = array1.GetLength(0);
                int c1 = array1.GetLength(1);
                int r2 = array2.GetLength(0);
                int c2 = array2.GetLength(1);

                if (c1 != r2)
                    throw new ArraySizeMismatchException(
                        String.Format("Array1 Dimension is : {{0},{1}} ; Array2 Dimension is : {{2},{3}}", r1, c1, r2, c2));

                double[,] outputArray = new double[r1, c2];

                unsafe
                {
                    fixed (double* arr1 = array1, arr2 = array2, outputArr = outputArray)
                    {
                        int i1, i2;
                        for (int i = 0; i < r1; i++)
                        {
                            i1 = i * c1;
                            for (int j = 0; j < c2; j++)
                            {
                                i2 = j;
                                double res = 0;
                                for (int k = 0; k < c1; k++, i2 += c2)
                                {
                                    res += arr1[i1 + k] * arr2[i2];
                                }
                                outputArr[i * c2 + j] = res;
                            }
                        }
                    }
                }

                return outputArray;
            }
            else if (array1 != null)
                return array1;
            else if (array2 != null)
                return array2;
            return null;
        }
        public static double[,] Dot(params double[][,] arrarys)
        {
            double[,] outputArray = arrarys[0];
            for (int i = 1; i < arrarys.Length; i++)
            {
                outputArray = Dot(outputArray, arrarys[i]);
            }
            return outputArray;
        }

        public static double[,] NewIdentityMatrix(int tn)
        {
            double[,] matrix = new double[tn, tn];
            unsafe
            {
                fixed (double* arr = matrix)
                {
                    for (int i = 0; i < tn; i++)
                    {
                        for (int j = 0; j < tn; j++)
                        {
                            if (i == j)
                                arr[i * tn + j] = 1;
                            else
                                arr[i * tn + j] = 0;
                        }
                    }
                }
            }
            return matrix;
        }
        public static double[,] NewZeroMatrix(int dimension1, int dimention2)
        {
            double[,] matrix = new double[dimension1, dimention2];
            unsafe
            {
                fixed(double* arr = matrix)
                {
                    for (int i = 0; i < dimension1; i++)
                    {
                        for (int j = 0; j < dimention2; j++)
                        {
                            arr[i * dimention2 + j] = 0;
                        }
                    }
                }
            }
            return matrix;
        }
    }
}
