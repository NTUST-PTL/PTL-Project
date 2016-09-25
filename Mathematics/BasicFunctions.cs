using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PTL.Geometry;
using PTL.Definitions;
using PTL.Geometry.MathModel;
using PTL.Exceptions;
using static System.Math;

namespace PTL.Mathematics
{
    public static class BasicFunctions
    {
        #region 三角函數
        public static double Sec(double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Cos(trad);
            return tvalue;
        }
        public static double Asec(double tv)
        {
            double tvalue;
            tvalue = System.Math.Acos(1 / tv);
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
        public static double GetFirstNNorm(double[] input, double n)
        {
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum = sum + input[i] * input[i];
            }
            double len = Sqrt(sum);

            return len;
        }
        public static XYZ3 GetAnyNormal(XYZ3 direction)
        {
            XYZ3 n1 = Cross(new XYZ3(0.0, 0.0, 1.0), direction);
            if (Norm(n1) < 1e-5)
                n1 = Cross(new XYZ3(0.0, 1.0, 0.0), direction);
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
        public static double[] Mult(double[] vector, double tscale)
        {
            double[] newVector = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
            {
                newVector[i] = tscale * vector[i];
            }
            return newVector;
        }
        public static double[,] Mult(double[,] tMatrix, double tscale)
        {
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
        public static double[] Add(params double[][] arrarys)
        {
            for (int i = 0; i < arrarys.Length - 1; i++)
                if (arrarys[i].Length != arrarys[i + 1].Length)
                    return null;

            int n = arrarys[0].Length;

            double[] outputArray = new double[n];
            for (int i = 0; i < n; i++)
                outputArray[i] = 0;

            for (int k = 0; k < arrarys.Length; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    outputArray[i] += arrarys[k][i];
                }
            }
            return outputArray;
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
            for (int k = 0; k < arrarys.Length; k++)
            {
                for (int i = 0; r1 > i; i++)
                {
                    for (int j = 0; c1 > j; j++)
                    {
                        outputArray[i, j] += arrarys[k][i, j];
                    }
                }
            }
            return outputArray;
        }
        public static double[] Substract(double[]  arr1, double[] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new ArrayDimensionMismatchException();

            double[] re = new double[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                re[i] = arr1[i] - arr2[i];
            }

            return re;
        }
        public static double[,] Substract(double[,] arr1, double[,] arr2)
        {
            if (arr1.Length != arr2.Length || arr1.GetLength(0) != arr2.GetLength(0))
                throw new ArrayDimensionMismatchException();

            int nRow = arr1.GetLength(0);
            int nCol = arr1.GetLength(1);

            double[,] re = new double[nRow, nCol];
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nCol; j++)
                {
                    re[i, j] = arr1[i, j] - arr2[i, j];
                }
            }

            return re;
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
                for (int i = 0; i < c2; i++)
                    outptr[i] = 0.0;

                for (int i = 0; i < c2; i++)
                {
                    for (int k = 0; k < r2; k++)
                    {
                        outptr[i] += array1[k] * array2[k, i];
                    }
                }
                return outptr;
            }
            else if (array1 != null)
                return array1;
            return null;
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
                        String.Format("Array1 Dimension is : {{{0},{1}}} ; Array2 Length is : {2}", c1, c1, r2));

                double[] outptr = new double[r1];

                for (i = 0; i < r1; i++)
                {
                    outptr[i] = 0.0;
                }
                for (i = 0; i < r1; i++)
                {
                    for (k = 0; k < c1; k++)
                    {
                        outptr[i] += array1[i, k] * array2[k];
                    }
                }
                return outptr;
            }
            else if (array2 != null)
                return array2;
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

                double[,] outptr = new double[r1, c2];
                for (int i = 0; i < r1; i++)
                {
                    for (int j = 0; j < c2; j++)
                    {
                        outptr[i, j] = 0.0;
                    }
                }
                for (int i = 0; i < r1; i++)
                {
                    for (int j = 0; j < c2; j++)
                    {
                        for (int k = 0; k < c1; k++)
                        {
                            outptr[i, j] += array1[i, k] * array2[k, j];
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
        public static double[,] Dot(params double[][,] arrarys)
        {
            double[,] outputArray = arrarys[0];
            for (int i = 1; i < arrarys.Length; i++)
            {
                outputArray = Dot(outputArray, arrarys[i]);
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
        

        public static double[,] NewIdentityMatrix(int tn)
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
        public static double[,] NewZeroMatrix(int dimension1, int dimention2)
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
        public static System.Array NewFilledMatrix<T>(T value, params int[] dimensions)
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
                newArray.SetValue(value, indices);
            }
            return newArray;
        }
        public static int[] GetDimensions(System.Array Array)
        {
            int[] dimensions = new int[Array.Rank];
            for (int i = 0; i < Array.Rank; i++)
                dimensions[i] = Array.GetLength(i);
            return dimensions;
        }
        public static System.Array Reshape(System.Array Array, params int[] dimensions)
        {
            System.Type elementType = Array.GetType().GetElementType();
            //定義Array每個維度的起始索引值(起始值可以不為0)
            int[] lowerBounds = new int[dimensions.Length];
            for (int i = 0; i < lowerBounds.Length; i++)
                lowerBounds[i] = 0;
            //建立新多維Array
            System.Array newArray = System.Array.CreateInstance(elementType, dimensions, lowerBounds);
            //擺入元素
            int index = 0;
            foreach (var item in Array)
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
        public static System.Array Reverse(System.Array Array, int dimension)
        {
            System.Type elementType = Array.GetType().GetElementType();
            int[] dimensions = GetDimensions(Array);
            //定義Array每個維度的起始索引值(起始值可以不為0)
            int[] lowerBounds = new int[dimensions.Length];
            for (int i = 0; i < lowerBounds.Length; i++)
                lowerBounds[i] = 0;
            //建立新多維Array
            System.Array newArray = System.Array.CreateInstance(elementType, dimensions, lowerBounds);
            //擺入元素
            int index = 0;
            foreach (var item in Array)
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
                indices[dimension] = dimensions[dimension] - indices[dimension] - 1;
                newArray.SetValue(item, indices);
                index++;
            }
            return newArray;
        }
        public static System.Array Take(System.Array Array, int[] startIndex, int[] endIndex)
        {
            System.Type elementType = Array.GetType().GetElementType();
            //定義Array每個維度的起始索引值(起始值可以不為0)
            int[] lowerBounds = new int[Array.Rank];
            for (int i = 0; i < lowerBounds.Length; i++)
                lowerBounds[i] = 0;

            //建立新多維Array
            int[] dimensions = new int[Array.Rank];
            for (int i = 0; i < Array.Rank; i++)
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
                newArray.SetValue(Array.GetValue(orgIndices), indices);
                index++;
            }
            return newArray;
        }
        public static System.Array Take(System.Array Array, params int[] range)
        {
            int n = range.Length;
            int[] startIndex = new int[n];
            int[] endIndex = new int[n];

            for (int i = 0; i < n; i++)
            {
                startIndex[i] = 0;
                endIndex[i] = range[i] - 1;
            }

            return Take(Array, startIndex, endIndex);
        }
        public static System.Array Cast<TypeIn, TypeOut>(System.Array array, Func<TypeIn, TypeOut> converter)
        {
            //定義Array每個維度的起始索引值(起始值可以不為0)
            int[] lowerBounds = new int[array.Rank];
            for (int i = 0; i < lowerBounds.Length; i++)
                lowerBounds[i] = 0;

            //建立新多維Array
            int[] dimensions = GetDimensions(array);
            System.Array newArray = System.Array.CreateInstance(typeof(TypeOut), dimensions, lowerBounds);

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
                
                newArray.SetValue(converter((TypeIn)array.GetValue(indices)), indices);
                index++;
            }
            return newArray;
        }
        public static TypeOut[] Cast<TypeIn, TypeOut>(this TypeIn[] array, Func<TypeIn, TypeOut> converter)
        {
            TypeOut[] newArr = new TypeOut[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArr[i] = converter(array[i]);
            }
            return newArr;
        }
        public static TypeOut[,] Cast<TypeIn, TypeOut>(this TypeIn[,] array, Func<TypeIn, TypeOut> converter)
        {
            int nRow = array.GetLength(0);
            int nCol = array.GetLength(1);
            TypeOut[,] newArr = new TypeOut[nRow, nCol];
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nCol; j++)
                {
                    newArr[i, j] = converter(array[i, j]);
                }
            }
            return newArr;
        }
        public static TypeOut[][,] Cast<TypeIn, TypeOut>(this TypeIn[][,] array, Func<TypeIn, TypeOut> converter)
        {
            int nN = array.GetLength(0);
            TypeOut[][,] newArr = new TypeOut[nN][,];
            for (int n = 0; n < nN; n++)
            {
                int nRow = array[n].GetLength(0);
                int nCol = array[n].GetLength(1);
                newArr[n] = new TypeOut[nRow, nCol];
                for (int i = 0; i < nRow; i++)
                {
                    for (int j = 0; j < nCol; j++)
                    {
                        newArr[n][i, j] = converter(array[n][i, j]);
                    }
                }
            }
            
            return newArr;
        }
        public static List<TypeOut> Cast<TypeIn, TypeOut>(this List<TypeIn> list, Func<TypeIn, TypeOut> converter)
        {
            List<TypeOut> newList = new List<TypeOut>();
            foreach (var item in list)
            {
                newList.Add(converter(item));
            }
            return newList;
        }
        public static void Replace(System.Array Matrix, System.Array TargetMatrix, int[] startIndex)
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
        public static Array Join(Array arr1, Array arr2, int dim = 0)
        {
            if (arr1 == null && arr2 != null)
                return arr2;
            if (arr1 != null && arr2 == null)
                return arr1;
            if (arr1 == null && arr2 == null)
                return null;

            Type type1 = arr1.GetType().GetElementType();
            Type type2 = arr2.GetType().GetElementType();

             int[] dims1 = GetDimensions(arr1);
             int[] dims2 = GetDimensions(arr2);


             if (dims1.Length != dims2.Length)
                 throw new ArrayDimensionMismatchException();
             for (int i = 0; i<dims1.Length; i++)
             {
                 if (i != dim && dims1[i] != dims2[i])
                     throw new ArraySizeMismatchException();
             }

             int[] newDims = new int[dims1.Length];
             for (int i = 0; i<newDims.Length; i++)
             {
                 if (i != dim)
                     newDims[i] = dims1[i];
                 else
                     newDims[i] = dims1[i] + dims2[i];
             }

             Array joined = Array.CreateInstance(type1, newDims);
             
             for (int i = 0; i<joined.Length; i++)
             {
                 int[] outputIndex = new int[newDims.Length];
                 int remains = i;
                 for (int k = 0; k<newDims.Length; k++)
                 {
                     int unit = 1;
                     for (int m = k + 1; m<newDims.Length; m++)
                         unit *= newDims[m];
                     outputIndex[k] = remains / unit;
                     remains -= outputIndex[k] * unit;
                 }

                 if (outputIndex[dim] < dims1[dim])
                     joined.SetValue(arr1.GetValue(outputIndex), outputIndex);
                 else
                 {
                     int[] indices2 = new int[outputIndex.Length];
                     outputIndex.CopyTo(indices2, 0);
                     indices2[dim] -= dims1[dim];
                     joined.SetValue(arr2.GetValue(indices2), outputIndex);
                 }
             }

             return joined;
        }
        public static newElementType[] ChangeArrayType<orgElementType, newElementType>(
            orgElementType[] orgArray,
            Func<orgElementType, newElementType> converter)
        {
            int num = orgArray.Length;
            newElementType[] newArray = new newElementType[num];
            for (int i = 0; i < num; i++)
            {
                newArray[i] = converter(orgArray[i]);
            }
            return newArray;
        }
        public static newElementType[,] ChangeArrayType<orgElementType, newElementType>(
            orgElementType[,] orgArray,
            Func<orgElementType, newElementType> converter)
        {
            int rNum = orgArray.GetLength(0);
            int cVum = orgArray.GetLength(1);
            newElementType[,] newArray = new newElementType[rNum, cVum];
            for (int i = 0; i < rNum; i++)
            {
                for (int j = 0; j < cVum; j++)
                {
                    newArray[i, j] = converter(orgArray[i, j]);
                }
            }
            return newArray;
        }
        public static List<object> Table(Func<int[], object> func, int[] perantIndex, params int[][] ranges)
        {
            List<object> results = new List<object>();
            if (ranges.Length > 1)
            {
                for (int i = ranges[0][0]; i <= ranges[0][1]; i++)
                {
                    int newIndexLength = perantIndex != null ? perantIndex.Length + 1 : 1;
                    int[] newPerantIndex = new int[newIndexLength];
                    perantIndex?.CopyTo(newPerantIndex, 0);
                    newPerantIndex[newIndexLength - 1] = i;

                    int[][] remainingIndexs = (from range in ranges
                                               where range != ranges[0]
                                               select range).ToArray();
                    results.Add(Table(func, newPerantIndex, remainingIndexs));
                }
            }
            else
            {
                

                for (int i = ranges[0][0]; i <= ranges[0][1]; i++)
                {
                    int IndexLength = perantIndex != null ? perantIndex.Length + 1 : 1;
                    int[] Indexs = new int[IndexLength];
                    perantIndex?.CopyTo(Indexs, 0);
                    Indexs[IndexLength - 1] = i;

                    results.Add(func(Indexs));
                }
            }
            return results;
        }

        public static double[,] NewRotateMatrix4(Axis axis, double theta)
        {
            double[,] Mr = null;
            switch (axis)
            {
                case Axis.X:
                    Mr = new double[,] { { 1,          0,           0, 0 },
                                         { 0, Cos(theta), -Sin(theta), 0 },
                                         { 0, Sin(theta),  Cos(theta), 0 },
                                         { 0,          0,           0, 1 } };
                    break;
                case Axis.Y:
                    Mr = new double[,] { {  Cos(theta), 0, Sin(theta), 0 },
                                         {           0, 1,          0, 0 },
                                         { -Sin(theta), 0, Cos(theta), 0 },
                                         {           0, 0,          0, 1 } };
                    break;
                case Axis.Z:
                    Mr = new double[,] { { Cos(theta), -Sin(theta), 0, 0 },
                                         { Sin(theta),  Cos(theta), 0, 0 },
                                         {          0,           0, 1, 0 },
                                         {          0,           0, 0, 1 } };
                    break;
            }
            return Mr;
        }
        public static double[,] NewRotateMatrix4(double[] tRotateAxis, double theta)
        {
            double angleZ = Atan2(tRotateAxis[1], tRotateAxis[0]);
            double angleY = -Atan2(tRotateAxis[2], Sqrt(tRotateAxis[0] * tRotateAxis[0] + tRotateAxis[1] * tRotateAxis[1]));
            double[,] A21 = NewRotateMatrix4(Axis.Z, -angleZ);
            double[,] A32 = NewRotateMatrix4(Axis.Y, -angleY);
            double[,] ARotate = NewRotateMatrix4(Axis.X, theta);
            double[,] A23 = NewRotateMatrix4(Axis.Y, angleY);
            double[,] A12 = NewRotateMatrix4(Axis.Z, angleZ);

            return Dot(A12, A23, ARotate, A32, A21);
        }

        public static double[] RotateX(double theta, double[] tr1)
        {
            double[,] m = NewRotateMatrix4(Axis.X, theta);
            return Transport3(m, tr1);
        }
        public static double[] RotateY(double theta, double[] tr1)
        {
            double[,] m = NewRotateMatrix4(Axis.Y, theta);
            return Transport3(m, tr1);
        }
        public static double[] RotateZ(double theta, double[] tr1)
        {
            double[,] m = NewRotateMatrix4(Axis.Z, theta);
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
            if (p.IsHomogeneous == true)
                result = Transport4(tMatrix, p.Values);
            else
                result = Transport3(tMatrix, p.Values);
            T newP = (T)p.New(result);
            return newP;
        }

        public static IEnumerable<double[]> RotateX(double theta, params double[][] tr1)
        {
            double[,] m = NewRotateMatrix4(Axis.X, theta);
            return Transport3(m, tr1);
        }
        public static IEnumerable<double[]> RotateY(double theta, params double[][] tr1)
        {
            double[,] m = NewRotateMatrix4(Axis.Y, theta);
            return Transport3(m, tr1);
        }
        public static IEnumerable<double[]> RotateZ(double theta, params double[][] tr1)
        {
            double[,] m = NewRotateMatrix4(Axis.Z, theta);
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

        public static double[,] Inverse(double[,] dMatrix)
        {
            int Level = dMatrix.GetLength(0);
            double dMatrixValue = Determinant(dMatrix);
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
        public static double Determinant(double[,] MatrixList)
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
        public static T[,] Transpose<T>(T[,] Matrix)
        {
            int r1, c1;
            r1 = Matrix.GetLength(0);  //UBound
            c1 = Matrix.GetLength(1);  // UBound
            T[,] Trans = new T[c1, r1];
            for (int ii = 0; ii < r1; ii++)
                for (int jj = 0; jj < c1; jj++)
                    Trans[jj, ii] = Matrix[ii, jj];
            return Trans;
        }
        public static T[][] Transpose<T>(T[][] Matrix)
        {
            int r1, c1;
            r1 = Matrix.Length;  //UBound
            c1 = Matrix[0].Length;  // UBound
            T[][] Trans = new T[c1][];
            for (int i = 0; i < c1; i++)
                Trans[i] = new T[r1];

            for (int ii = 0; ii < r1; ii++)
                for (int jj = 0; jj < c1; jj++)
                    Trans[jj][ii] = Matrix[ii][jj];
                
            return Trans;
        }

        public static T[][] ToIrregularArray<T>(T[,] Matrix)
        {
            int r1, c1;
            r1 = Matrix.GetLength(0);
            c1 = Matrix.GetLength(1);
            T[][] irM = new T[r1][];
            for (int ii = 0; ii < r1; ii++)
            {
                irM[ii] = new T[c1];
                for (int jj = 0; jj < c1; jj++)
                    irM[ii][jj] = Matrix[ii, jj];
            }
            return irM;
        }
        public static T[,] ToRegularArray<T>(T[][] Matrix)
        {
            int r1, c1;
            r1 = Matrix.Length;
            c1 = Matrix[0].Length;
            T[,] rM = new T[r1, c1];
            for (int ii = 0; ii < r1; ii++)
            {
                for (int jj = 0; jj < c1; jj++)
                    rM[ii,jj] = Matrix[ii][jj];
            }
            return rM;
        }
        #endregion

        public static class NonlinearFindRoot
        {
            public static string nError = "";
            public static TimeSpan TimeSpanLimit = new TimeSpan(0, 0, 5);
            public class Result
            {
                public double[] Answer;
                public bool Succes { get; set; }
                public bool TimesUp { get; set; }
            }
            public delegate void EquationSet(double[] input, double[] output);

            public static Result FindRoot(Func<double, double> Equation, double InitialGuest
                , int MAXITS = 200
                , double TOLF = 1E-08
                , double TOLMIN = 1E-10
                , double TOLX = 1E-11
                , double STPMX = 100)
            {
                double[] x = new double[] { InitialGuest };
                EquationSet aEquationSet = (double[] xx, double[] yy) => { yy[0] = Equation(xx[0]); };
                Result Result = FindRoot(x, aEquationSet
                    , MAXITS
                    , TOLF
                    , TOLMIN
                    , TOLX
                    , STPMX);
                return Result;
            }
            public static Result FindRoot(Func<double[], double[]> Equation, double[] InitialGuest
                , int MAXITS = 200
                , double TOLF = 1E-08
                , double TOLMIN = 1E-10
                , double TOLX = 1E-11
                , double STPMX = 100)
            {
                double[] x = new double[InitialGuest.Length];
                InitialGuest.CopyTo(x, 0);
                EquationSet aEquationSet = (double[] xx, double[] yy) => {
                    var y = Equation(xx);
                    for (int i = 0; i < yy.Length; i++)
                        yy[i] = y[i];
                };
                Result Result = FindRoot(x, aEquationSet
                    , MAXITS
                    , TOLF
                    , TOLMIN
                    , TOLX
                    , STPMX);
                return Result;
            }
            public static Result FindRoot(double[] x, EquationSet objEquationSet
                , int MAXITS = 200
                , double TOLF = 1E-08
                , double TOLMIN = 1E-10
                , double TOLX = 1E-11
                , double STPMX = 100)
            {
                bool Check = false;
                bool timesUP = newt(x, objEquationSet, Check
                    , MAXITS
                    , TOLF
                    , TOLMIN
                    , TOLX
                    , STPMX);
                Result states = new Result() { TimesUp = timesUP, Succes= Check, Answer = x };

                return states;
            }
            private static bool newt(double[] x, EquationSet objEquationSet, bool Check
                , int MAXITS = 200
                , double TOLF = 1E-08
                , double TOLMIN = 1E-10
                , double TOLX = 1E-11
                , double STPMX = 100)
            {
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
                while (true)
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

        #region 其他
        public static double Min(params double[] paras)
        {
            if (paras == null || paras.Length == 0)
                throw new ArgumentNullException();

            double min = double.PositiveInfinity;
            foreach (var item in paras)
            {
                if (item < min)
                {
                    min = item;
                }
            }
            return min;
        }
        public static double Max(params double[] paras)
        {
            if (paras == null || paras.Length == 0)
                throw new ArgumentNullException();

            double max = double.NegativeInfinity;
            foreach (var item in paras)
            {
                if (max < item)
                {
                    max = item;
                }
            }
            return max;
        }
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
        public static double Floor(double x, double a = 1)
        {
            double xx = x / a;
            double xx1 = System.Math.Floor(xx);
            return xx1 * a;
        }
        public static double Ceiling(double x, double a = 1)
        {
            double xx = x / a;
            double xx1 = System.Math.Ceiling(xx);
            return xx1 * a;
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
            if (point2Check == null || point2Check.Values == null)
                return;
            if (point2Check.X > Boundary[1].X) { Boundary[1].X = point2Check.X; }
            else if (point2Check.X < Boundary[0].X) { Boundary[0].X = point2Check.X; }
            if (point2Check.Y > Boundary[1].Y) { Boundary[1].Y = point2Check.Y; }
            else if (point2Check.Y < Boundary[0].Y) { Boundary[0].Y = point2Check.Y; }
            if (point2Check.Z > Boundary[1].Z) { Boundary[1].Z = point2Check.Z; }
            else if (point2Check.Z < Boundary[0].Z) { Boundary[0].Z = point2Check.Z; }
        }
        public static IEnumerable<int> EachSign(IEnumerable<double> values)
        {
            foreach (var item in values)
            {
                yield return Sign(item);
            }
        }
        public static void Exchange(ref double a,ref double b)
        {
            double a2 = a;
            a = b;
            b = a2;
        }
        #endregion
    }
}
