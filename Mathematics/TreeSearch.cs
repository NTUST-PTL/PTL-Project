using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTL.Mathematics.BaseFunctions;
using PTL.DebugTools;
using PTL.Geometry;
using PTL.Geometry.MathModel;

namespace PTL.Mathematics
{
    public class TreeSearch
    {
        static Plot plot;
        static int[] indexbuffer;
        static double[] parambuffer;
        static double[] samplesbuffer;
        static double[] distancebuffer;
        static double[] ansBuffer;

        public static double[] NearestSearch(Func<double[], double[]> func, double[] target, double[] startBoundary, double[] endBoundary, int S1 = 4, int S2 = 1, int Iteration = 25)
        {
            //plot = new Plot();
            //plot.Window.View.AutoScale = false;

            int rank = startBoundary.Length;
            int L1 = S1 + 1;
            int dataLength = target.Length;
            int sampleNum = 1;
            for (int i = 0; i < rank; i++)
            {
                sampleNum *= L1;
            }

            if (indexbuffer == null || indexbuffer.Length != rank)
                indexbuffer = new int[rank];
            if (parambuffer == null || parambuffer.Length != rank)
                parambuffer = new double[rank];
            if (samplesbuffer == null || samplesbuffer.Length != sampleNum * dataLength)
                samplesbuffer = new double[sampleNum * dataLength];
            if (distancebuffer == null || distancebuffer.Length != sampleNum)
                distancebuffer = new double[sampleNum];
            if (ansBuffer == null || ansBuffer.Length != rank)
                ansBuffer = new double[rank];

            NearestSearch(
                  rank
                , dataLength
                , sampleNum

                , indexbuffer
                , parambuffer
                , samplesbuffer
                , distancebuffer
                , ansBuffer

                , func
                , target
                , startBoundary
                , endBoundary
                , S1
                , S2
                , L1
                , 0
                , Iteration);

            return ansBuffer;
        }

        private static void NearestSearch(
              int rank
            , int dataLength
            , int samplesNum

            , int[] index
            , double[] paras
            , double[] samplesData
            , double[] distances
            , double[] ansBuffer

            , Func<double[], double[]> func
            , double[] target
            , double[] startBoundary
            , double[] endBoundary
            , int S1
            , int S2
            , int L1
            , int currentIteration
            , int maxIteration)
        {
            #region 計算取樣點
            for (int i = 0; i < samplesNum; i++)
            {
                #region 計算多維編號index
                int residue = i;
                for (int j = 0; j < rank; j++)
                {
                    int num = 1;
                    for (int k = j + 1; k < rank; k++)
                        num *= L1;
                    index[j] = residue / num;
                    residue -= index[j] * num;
                }
                //Console.Write("Index : ");
                //Console.WriteLine(ArrayToString(index));
                #endregion

                #region 計算paras
                for (int j = 0; j < rank; j++)
                {
                    paras[j] = startBoundary[j] + index[j] * (endBoundary[j] - startBoundary[j]) / S1;
                }
                //Console.Write("paras : ");
                //Console.WriteLine(ArrayToString(paras));
                #endregion

                #region 計算取樣點
                double[] data = func(paras);
                int p = i * dataLength;
                for (int k = 0; k < dataLength; k++)
                {
                    samplesData[p + k] = data[k];
                }
                //Console.Write("data : ");
                //Console.WriteLine(ArrayToString(data));
                #endregion
            }
            #endregion
            //Console.Write("samplesData : ");
            //Console.WriteLine(ArrayToString(samplesData));

            #region 計算取樣點與目標距離
            for (int j = 0; j < samplesNum; j++)
            {
                int p = j * dataLength;
                distances[j] = 0;
                for (int k = 0; k < dataLength; k++)
                {
                    double d = samplesData[p + k] - target[k];
                    distances[j] += d * d;
                }
            }
            #endregion
            //Console.Write("samplesData : ");
            //Console.WriteLine(ArrayToString(distances));

            #region 尋找最近取樣點一維編號
            double minDis = double.PositiveInfinity;
            int minDisSampleIndex = 0;
            for (int i = 0; i < samplesNum; i++)
            {
                if (minDis > distances[i])
                {
                    minDis = distances[i];
                    minDisSampleIndex = i;
                }
            }
            #endregion
            //Console.Write("minDisSampleIndex : ");
            //Console.WriteLine(minDisSampleIndex);
            
            #region 計算最近取樣點多維編號 儲存於index
            int residue2 = minDisSampleIndex;
            for (int j = 0; j < rank; j++)
            {
                int num = 1;
                for (int k = j + 1; k < rank; k++)
                    num *= L1;
                index[j] = residue2 / num;
                residue2 -= index[j] * num;
            }
            #endregion
            //Console.Write("index : ");
            //Console.WriteLine(ArrayToString(index));

            #region 計算最新變數範圍
            double[] newStart = new double[rank];
            double[] newEnd = new double[rank];
            for (int j = 0; j < rank; j++)
            {
                if (index[j] >= S2)
                    newStart[j] = startBoundary[j] + (index[j] - S2) * (endBoundary[j] - startBoundary[j]) / S1;
                else
                    newStart[j] = startBoundary[j];

                if ((L1 - S2) > index[j])
                    newEnd[j] = startBoundary[j] + (index[j] + S2) * (endBoundary[j] - startBoundary[j]) / S1;
                else
                    newEnd[j] = endBoundary[j];
            }
            #endregion
            //Console.Write("newStart : ");
            //Console.WriteLine(ArrayToString(newStart));
            //Console.Write("newEnd : ");
            //Console.WriteLine(ArrayToString(newEnd));

            //plot.ParameterPlot((u, v) => new XYZ4(func(new double[] { u, v }))
            //, startBoundary[0], endBoundary[0], 20
            //, startBoundary[1], endBoundary[1], 20
            //, (tf) => tf.Color = System.Drawing.Color.FromArgb(80, 200, 200, 200));
            //List<PointD> points = new List<PointD>();
            //for (int i = 0; i < samplesNum; i++)
            //{
            //    double[] data = new double[3];
            //    int p = i * dataLength;
            //    for (int j = 0; j < 3; j++)
            //    {
            //        data[j] = samplesData[p + j];
            //    }
            //    points.Add(new PointD(data) { OpenGLDisplaySize = 0.01});
            //}
            //plot.AddSomethings(points.ToArray());
            //plot.AddSomethings(new PointD(target));

            //return;

            #region 疊代
            currentIteration++;
            if (currentIteration < maxIteration)
            {
                NearestSearch(
                      rank
                    , dataLength
                    , samplesNum

                    , index
                    , paras
                    , samplesData
                    , distances
                    , ansBuffer

                    , func
                    , target
                    , newStart
                    , newEnd
                    , S1
                    , S2
                    , L1
                    , currentIteration
                    , maxIteration);
            }
            else
            {
                for (int i = 0; i < rank; i++)
                {
                    ansBuffer[i] = (newStart[i] + newEnd[i]) / 2;
                }
            }
            #endregion
            
        }

        public static string ArrayToString<T>(T[] arr)
        {
            string str = "";
            for (int i = 0; i < arr.Length; i++)
            {
                str += arr[i] + ", ";
            }
            return str;
        }

    }
}
