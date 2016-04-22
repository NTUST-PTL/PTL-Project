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
    }
}
