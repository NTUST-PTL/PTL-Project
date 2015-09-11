using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PTL.Geometry;
using PTL.Definitions;
using PTL.Geometry.MathModel;

namespace PTL.MathExtension
{
    public static class MathExtension
    {
        #region 三角函數
        public static double Cos(this object obj, double trad)
        {
            double tvalue;
            tvalue = System.Math.Cos(trad);
            return tvalue;
        }
        public static double Sin(this object obj, double trad)
        {
            double tvalue;
            tvalue = System.Math.Sin(trad);
            return tvalue;
        }
        public static double Tan(this object obj, double trad)
        {
            double tvalue;
            tvalue = System.Math.Tan(trad);
            return tvalue;
        }
        public static double Sec(this object obj, double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Cos(trad);
            return tvalue;
        }
        public static double Acos(this object obj, double tv)
        {
            double tvalue;
            tvalue = System.Math.Acos(tv);
            return tvalue;
        }
        public static double Atan2(this object obj, double y, double x)
        {
            double tvalue;
            tvalue = System.Math.Atan2(y, x);
            return tvalue;
        }
        public static double Atan(this object obj, double tv)
        {
            double tvalue;
            tvalue = System.Math.Atan(tv);
            return tvalue;
        }
        public static double Asin(this object obj, double tv)
        {
            double tvalue;
            tvalue = System.Math.Asin(tv);
            return tvalue;
        }
        public static double Asec(this object obj, double tv)
        {
            double tvalue;
            tvalue = System.Math.Acos(1.0 / tv);
            return tvalue;
        }
        public static double Csc(this object obj, double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Sin(trad);
            return tvalue;
        }
        public static double Cot(this object obj, double trad)
        {
            double tvalue;
            tvalue = 1 / System.Math.Tan(trad);
            return tvalue;
        }
        #endregion

        #region 單位轉換
        public static double DegToRad(this object obj, double tdeg)
        {
            double trad;
            trad = tdeg / 180.0 * System.Math.PI;
            return trad;
        }
        public static double RadToDeg(this object obj, double trad)
        {
            double tdeg;
            tdeg = trad / System.Math.PI * 180.0;
            return tdeg;
        }
        public static double mmToInch(this object obj, double tmm)
        {
            double tInch;
            tInch = tmm / 25.4;
            return tInch;
        }
        public static double InchTomm(this object obj, double tInch)
        {
            double tmm;
            tmm = tInch * 25.4;
            return tmm;
        }
        public static string DegToDMS(this object obj, double x)
        {
            int x1 = (int)x;
            int x2 = (int)((x - x1) * 60);
            int x3 = (int)((((x - x1) * 60) - x2) * 60);
            string ss = "";
            ss = String.Format("{0}", x1) + "d" + String.Format("{0,3}", x2) + "m" + String.Format("{0,3}", x3) + "s";
            return ss;
        }
        public static string DegToDMS(this object obj, double x,short n)
        {
            double absx = Abs(obj, x);
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
        public static string RadToDMS(this object obj, double y)
        {
            string ss = DegToDMS(obj, RadToDeg(obj, y));
            return ss;
        }
        public static string RadToDMS(this object obj, double y,short n)
        {
            string ss = DegToDMS(obj, RadToDeg(obj, y),n);
            return ss;
        }
        public static double DMSToDeg(this object obj, double dd, double mm, double ss)
        {
            double deg = dd + mm / 60.0 + ss / 3600.0;
            return deg;
        }
        public static double DMSToRad(this object obj, double dd, double mm, double ss)
        {
            double rad = DegToRad(obj, DMSToDeg(obj, dd, mm, ss));
            return rad;
        }
        #endregion

        #region Matrix & Vector
        public static T Normalize<T>(this object obj, T r1) where T : Vector, new()
        {
            double len = Sqrt(obj, r1.X * r1.X + r1.Y * r1.Y + r1.Z * r1.Z);
            T r2 = new T() { X = r1.X / len, Y = r1.Y / len, Z = r1.Z / len };
            return r2;
        }
        public static double GetLength(this object obj, double[] input)
        {
            double sum = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                sum = sum + System.Math.Pow(input[i], 2);
            }
            double len = Sqrt(obj, sum);

            return len;
        }
        public static double Norm(this object obj, double[] input)
        {
            double sum = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                sum = sum + System.Math.Pow(input[i], 2);
            }
            double len = Sqrt(obj, sum);

            return len;
        }
        public static double Norm(this object obj, XYZ input)
        {
            return Norm(obj, input.Values);
        }
        public static double Norm<T>(this object obj, T p1) where T : Vector, new()
        {
            double sum = p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z;
            double len = Sqrt(obj, sum);

            return len;
        }

        public static double[] MatrixDot(this object obj, double[] array1, double[] array2)
        {
            int r1 = array1.Length;  // UBound

            double[] outptr = new double[r1];
            for (int i = 0; i < r1; i++)
            {
                outptr[i] = array1[i] * array2[i];
            }
            return outptr;
        }
        public static double[] MatrixDot(this object obj, double[] array1, double[,] array2)
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
        public static double[] MatrixDot(this object obj, double[,] array1, double[] array2)
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
        public static double[,] MatrixDot(this object obj, double[,] array1, double[,] array2)
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
        public static double[,] MatrixDot(this object obj, params double[][,] arrarys)
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
        public static double[,] MatrixAdd(this object obj, params double[][,] arrarys)
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
        public static double[] Cross(this object obj, double[] ptr1, double[] ptr2)
        {
            double[] outptr = new double[3];
            outptr[0] = ptr1[1] * ptr2[2] - ptr1[2] * ptr2[1];
            outptr[1] = ptr1[2] * ptr2[0] - ptr1[0] * ptr2[2];
            outptr[2] = ptr1[0] * ptr2[1] - ptr1[1] * ptr2[0];
            return outptr;
        }
        public static XYZ Cross(this object obj, XYZ ptr1, XYZ ptr2)
        {
            XYZ outptr = new XYZ();
            outptr[0] = ptr1[1] * ptr2[2] - ptr1[2] * ptr2[1];
            outptr[1] = ptr1[2] * ptr2[0] - ptr1[0] * ptr2[2];
            outptr[2] = ptr1[0] * ptr2[1] - ptr1[1] * ptr2[0];
            return outptr;
        }
        public static T Cross<T>(this object obj, T p1, T p2) where T : Vector, new()
        {
            T p3 = new T();

            p3.X = p1.Y * p2.Z - p1.Z * p2.Y;
            p3.Y = p1.Z * p2.X - p1.X * p2.Z;
            p3.Z = p1.X * p2.Y - p1.Y * p2.X;

            return p3;
        }
        public static double Dot(this object obj, double[] ptr1, double[] ptr2)
        {
            double value = 0.0;

            for (int i = 0; i <= ptr1.GetUpperBound(0); i++)
            {
                value = value + ptr1[i] * ptr2[i];
            }
            return value;
        }
        public static double Dot(this object obj, Vector p1, Vector p2)
        {
            double value;

            value = p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;

            return value;
        }
        public static T MatrixDot3<T>(this object obj, double[,] tMatrix, T tp1) where T : Vector, new()
        {
            if (tMatrix == null)
                return tp1.Clone() as T;
            double[] tr1 = new double[3] { tp1.X, tp1.Y, tp1.Z, };
            double[] tr2 = new double[3];

            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    tr2[i] = tr2[i] + tMatrix[i, k] * tr1[k];
                }
            }

            T p2 = new T() { X = tr2[0], Y = tr2[1], Z = tr2[2] };
            return p2;
        }
        public static T MatrixDot4<T>(this object obj, double[,] tMatrix, T tp1) where T : Vector, new()
        {
            if (tMatrix == null)
                return tp1.Clone() as T;
            double[] tr1 = new double[4] { tp1.X, tp1.Y, tp1.Z, 1.0 };
            double[] tr2 = new double[4];

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    tr2[i] = tr2[i] + tMatrix[i, k] * tr1[k];
                }
            }

            T p2 = new T() { X = tr2[0], Y = tr2[1], Z = tr2[2] };
            return p2;
        }
        public static double[] MatrixDot3(this object obj, double[,] tMatrix, double[] tr1)
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
        public static double[] MatrixDot4(this object obj, double[,] tMatrix, double[] tr1)
        {
            double[] tr2 = new double[4];

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    tr2[i] = tr2[i] + tMatrix[i, k] * tr1[k];
                }
            }
            return tr2;
        }
        public static double[,] MatrixScale(this object obj, double[,] tMatrix, double tscale)
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

        public static double[,] IdentityMatrix(this object obj, int tn)
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
        public static double[,] ZeroMatrix(this object obj, int dimension1, int dimention2)
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
        public static System.Array InitializedMatrix<T>(this object obj, T value, params int[] dimensions)
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
        public static int[] ArrayGetDimension(this object obj, System.Array Array)
        {
            int[] dimensions = new int[Array.Rank];
            for (int i = 0; i < Array.Rank; i++)
                dimensions[i] = Array.GetLength(i);
            return dimensions;
        }
        public static System.Array ArrayReshape(this object obj, System.Array Matrix, params int[] dimensions)
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
        public static System.Array ArrayTake(this object obj, System.Array Matrix, int[] startIndex, int[] endIndex)
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
        public static void ArrayPut(this object obj, System.Array Matrix, System.Array TargetMatrix, int[] startIndex)
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

        public static double[,] RotateMatrix(this object obj, Axis axis, double theta)
        {
            double[,] Mr = IdentityMatrix(obj, 4);
            switch (axis)
            {
                case Axis.X:
                    Mr[1, 1] = Cos(obj, theta);
                    Mr[2, 2] = Cos(obj, theta);
                    Mr[1, 2] = -Sin(obj, theta);
                    Mr[2, 1] = Sin(obj, theta);
                    break;
                case Axis.Y:
                    Mr[0, 0] = Cos(obj, theta);
                    Mr[2, 2] = Cos(obj, theta);
                    Mr[0, 2] = -Sin(obj, theta);
                    Mr[2, 0] = Sin(obj, theta);
                    break;
                case Axis.Z:
                    Mr[0, 0] = Cos(obj, theta);
                    Mr[1, 1] = Cos(obj, theta);
                    Mr[0, 1] = -Sin(obj, theta);
                    Mr[1, 0] = Sin(obj, theta);
                    break;
            }
            return Mr;
        }
        public static double[,] RotateMatrix(this object obj, double thetax, double thetay, double thetaz)
        {
            double[,] MRx = RotateMatrix(obj, Axis.X, thetax);
            double[,] MRy = RotateMatrix(obj, Axis.Y, thetay);
            double[,] MRz = RotateMatrix(obj, Axis.Z, thetaz);

            double[,] Mr = MatrixDot(MRx, MRy, MRz);

            return Mr;
        }
        public static double[,] RotateMatrix(this object obj, Vector tRotateAxis, double theta)
        {
            double angleZ = Atan2(obj, tRotateAxis.Y, tRotateAxis.X);
            double angleY = Atan2(obj, tRotateAxis.Z, tRotateAxis.X * tRotateAxis.X + tRotateAxis.Y * tRotateAxis.Y);
            double[,] A21 = RotateMatrix(obj, Axis.Z, -angleZ);
            double[,] A32 = RotateMatrix(obj, Axis.Y, angleY);
            double[,] ARotate = RotateMatrix(obj, Axis.X, theta);
            double[,] A23 = MatrixTranspose(obj, A32);
            double[,] A12 = MatrixTranspose(obj, A21);

            return MatrixDot(A12, A23, ARotate, A32, A21);
        }
        public static T RotatePointZ<T>(this object obj, T pnt, double theta) where T : Vector, new()
        {
            T pntnew = MatrixDot4(obj, RotateMatrix(obj, Axis.Z, theta), pnt);
            return pntnew;
        }
        public static T[,] RotatePointZ<T>(this object obj, T[,] pnt, double theta) where T : Vector, new()
        {
            return RotatePoints(obj, RotateMatrix(obj, Axis.Z, theta), pnt);
        }
        public static double[,] TranslateMatrix(this object obj, PointD p1)
        {
            double[,] tMatrix = new double[4, 4];

            tMatrix = IdentityMatrix(obj, 4);
            tMatrix[0, 3] = p1.X;
            tMatrix[1, 3] = p1.Y;
            tMatrix[2, 3] = p1.Z;

            return tMatrix;
        }
        public static T RotatePoint<T>(this object obj, double[,] M21, T pnt) where T : Vector, new()
        {
            T pntnew = MatrixDot4(obj, M21, pnt);
            return pntnew;
        }
        public static T[] RotatePoints<T>(this object obj, double[,] M21, T[] pset) where T : Vector, new()
        {
            int nump = pset.Length;
            T[] pseta = new T[nump];

            for (short k = 0; k < nump; k++)
                pseta[k] = MatrixDot4(obj, M21, pset[k]);

            return pseta;
        }
        public static List<T> RotatePoints<T>(this object obj, double[,] M21, IEnumerable<T> pset) where T : Vector, new()
        {
            List<T> pseta = new List<T>();

            foreach (var p in pset)
                pseta.Add(MatrixDot4(obj, M21, p));

            return pseta;
        }
        public static T[,] RotatePoints<T>(this object obj, double[,] M21, T[,] pset) where T : Vector, new()
        {
            int nump0 = pset.GetLength(0);
            int nump1 = pset.GetLength(1);
            T[,] pseta = new T[nump0, nump1];

            for (short i = 0; i < nump0; i++)
                for (int j = 0; j < nump1; j++)
                    pseta[i, j] = MatrixDot4(obj, M21, pset[i, j]);

            return pseta;
        }

        //新寫法20150409
        //Homogeneous與非Homogeneous適用、傳遞參數數目傳不固定
        public static T TransformPoint<T>(this object obj, double[,] M21, T pset) where T : Vector, new()
        {
            if (M21 == null || pset == null)
                return pset;

            double[] tr1;
            double[] tr2;
            if (pset is IHomogeneous)
            {
                tr1 = new double[4] { pset.X, pset.Y, pset.Z, 1};
                tr2 = new double[4];

                for (int i = 0; i < 4; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        tr2[i] += M21[i, k] * tr1[k];
                    }
                }
            }
            else
            {
                tr1 = new double[3] { pset.X, pset.Y, pset.Z };
                tr2 = new double[3];

                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tr2[i] += M21[i, k] * tr1[k];
                    }
                }
            }

            T p2 = new T() { X = tr2[0], Y = tr2[1], Z = tr2[2] };
            return p2;
        }
        public static List<T> TransformPoints<T>(this object obj, double[,] M21, List<T> pset) where T : Vector, new()
        {
            List<T> pseta = new List<T>();

            int k = 0;
            foreach (var p in pset)
            {
                pseta.Add(TransformPoint(obj, M21, p));
                k++;
            }

            return pseta;
        }
        public static List<T> TransformPoints<T>(this object obj, double[,] M21, params T[] pset) where T : Vector, new()
        {
            List<T> pseta = new List<T>();

            int k = 0;
            foreach (var p in pset)
            {
                pseta.Add(TransformPoint(obj, M21, p));
                k++;
            }

            return pseta;
        }
        public static T[,] TransformPoints<T>(this object obj, double[,] M21, T[,] pset) where T : Vector, new()
        {
            int dim1 = pset.GetLength(0);
            int dim2 = pset.GetLength(1);

            T[,] newPset = new T[dim1, dim2];
            for (int i = 0; i < dim1; i++)
            {
                for (int j = 0; j < dim2; j++)
                {
                    newPset[i, j] = TransformPoint(obj, M21, pset[i, j]);
                }
            }
            return newPset;
        }
        public static double[] Transport3(this object obj, double[,] tMatrix, double[] tr1)
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
        public static double[] Transport4(this object obj, double[,] tMatrix, double[] tr1)
        {
            double[] tr2 = Transport3(obj, tMatrix, tr1);
            tr2[0] += tMatrix[0, 3];
            tr2[1] += tMatrix[1, 3];
            tr2[2] += tMatrix[2, 3];
            return tr2;
        }

        /// <summary>
        /// 反矩陣
        /// </summary>
        /// <param name="dMatrix">矩陣</param>
        /// <param name="Level">矩陣的維度</param>
        /// <returns></returns>
        public static double[,] MatrixInverse(this object obj, double[,] dMatrix)
        {
            int Level = dMatrix.GetLength(0);
            double dMatrixValue = MatrixDeterminant(obj, dMatrix);
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
                    for (; m <= Level - 1 && dReverseMatrix[m, j] == 0 ; m++) ;/////////////////////////////////////////錯誤修正20150205
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
        public static double MatrixDeterminant(this object obj, double[,] MatrixList)
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
        public static T[,] MatrixTranspose<T>(this object obj, T[,] Matrix)
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
        public static double Sphive(this object obj, double tphi, double tgammab)
        {
            double tsphive;
            tsphive = System.Math.Atan(System.Math.Tan(tphi) * System.Math.Sin(tgammab)) / System.Math.Sin(tgammab) - tphi;
            return tsphive;
        }
        public static double Involute(this object obj, double alpha)
        {

            return (System.Math.Tan(alpha) - alpha);

        }
        public static double Pow(this object obj, double tx, double td)
        {
            return System.Math.Pow(tx, td);
        }
        public static double Floor(this object obj, double x, double a)
        {
            double xx = x / a;
            int xx1 = (int)xx;
            return xx1 * a;
        }
        public static double Sqrt(this object obj, double ta)
        {
            return System.Math.Sqrt(ta);
        }
        public static double PI()
        {
            return System.Math.PI;
        }
        public static double Abs(this object obj, double ta)
        {
            return System.Math.Abs(ta);
        }
        public static double Round(this object obj, double ta, double tb)
        {
            return System.Math.Round(ta / tb) * tb;
        }
        public static double Mod(this object obj, double ta, double tb)
        {
            double tc = ta - Floor(obj, ta / tb, 1.0) * tb;
            return tc;
        }
        public static PointD CalcuCenterXY(this object obj, PointD p1,PointD p2, double radius, short ctype)
        {
            double ra = Abs(obj, radius);
            short Flag = (short)(radius / ra);
            PointD za = new PointD(0.0,0.0,1.0);
            PointD v1 = Normalize(obj, p2 -p1);
            PointD v2 = Cross(obj, v1,za);
            double dist = Norm(obj, p2 - p1);
            double xx = Sqrt(obj, ra * ra - (dist / 2.0) * (dist / 2.0));

             PointD pc;
            if(ctype==2)
                pc= p1 + dist / 2.0 * v1 + Flag * xx * v2;
            else
                pc = p1 + dist / 2.0 * v1 - Flag * xx * v2;

            return pc;
        }
        public static void Compare_Boundary(PointD[] Boundary,PointD point2Check)
        {
            if (point2Check.X > Boundary[1].X) { Boundary[1].X = point2Check.X; }
            else if (point2Check.X < Boundary[0].X) { Boundary[0].X = point2Check.X; }
            if (point2Check.Y > Boundary[1].Y) { Boundary[1].Y = point2Check.Y; }
            else if (point2Check.Y < Boundary[0].Y) { Boundary[0].Y = point2Check.Y; }
            if (point2Check.Z > Boundary[1].Z) { Boundary[1].Z = point2Check.Z; }
            else if (point2Check.Z < Boundary[0].Z) { Boundary[0].Z = point2Check.Z; }
        }
        public static int[] Sign(params double[] values)
        {
            int[] signs = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > 0)
                    signs[i] = 1;
                else if (values[i] < 0)
                    signs[i] = -1;
                else
                    signs[i] = 0;
            }
            return signs;
        }
        #endregion
    }

    
}