using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PTL.Geometry;
using PTL.Tools.BinaryExtentionMethods;
using System.Threading.Tasks;

namespace PTL.Tools.FileOperation
{
    public class STLReader
    {
        /// <summary>
        /// 讀取STL檔案，可加入過濾方法篩除不必要的三角面
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="aTriangleFilter">三角面過濾方法</param>
        /// <returns>回傳STL</returns>
        public static async Task<STL> ReadSTLFile(String fileName, Predicate<Triangle> TriangleFilter = null)
        {
            STL stl = new STL();
            //Try ASCII
            stl = await Task.Run<STL>(()=>ReadSTL_ASCII(fileName, TriangleFilter));
            //Try Bynary
            if (stl == null)
                stl = await Task.Run<STL>(() => ReadSTL_Binary(fileName, TriangleFilter));
            
            
            return stl;
        }

        public static STL ReadSTL_ASCII(String fileName, Predicate<Triangle> TriangleFilter = null)
        {
            System.IO.StreamReader stream = new System.IO.StreamReader(fileName, Encoding.Default);

            string strLine;
            string[] strArray;
            List<String> strList;

            STL newSTL;

            //aSTL.Name = myFile.ReadLine();
            try
            {
                STL aSTL = new STL() { Color = System.Drawing.Color.Gray };
                while (stream.Peek() != -1)
                {
                    Triangle aTriangle = new Triangle() { Color = System.Drawing.Color.Transparent };
                    //1 法向量
                    strLine = stream.ReadLine();
                    strArray = strLine.Split(' ');
                    strList = new List<string>();
                    foreach (var item in strArray)
                    {
                        if (item != "")
                            strList.Add(item);
                    }
                    if (strList[0].ToLower() == "facet")
                    {
                        PointD N = new PointD()
                        {
                            X = Convert.ToDouble(strList[2]),
                            Y = Convert.ToDouble(strList[3]),
                            Z = Convert.ToDouble(strList[4])
                        };
                        aTriangle.N1 = N;
                        //捨棄
                        stream.ReadLine();
                        //點
                        strLine = stream.ReadLine();
                        strArray = strLine.Split(' ');
                        strList = new List<string>();
                        foreach (var item in strArray)
                        {
                            if (item != "")
                                strList.Add(item);
                        }
                        aTriangle.P1 = new PointD()
                        {
                            X = Convert.ToDouble(strList[1]),
                            Y = Convert.ToDouble(strList[2]),
                            Z = Convert.ToDouble(strList[3])
                        };
                        //點
                        strLine = stream.ReadLine();
                        strArray = strLine.Split(' ');
                        strList = new List<string>();
                        foreach (var item in strArray)
                        {
                            if (item != "")
                                strList.Add(item);
                        }
                        aTriangle.P2 = new PointD()
                        {
                            X = Convert.ToDouble(strList[1]),
                            Y = Convert.ToDouble(strList[2]),
                            Z = Convert.ToDouble(strList[3])
                        };
                        //點
                        strLine = stream.ReadLine();
                        strArray = strLine.Split(' ');
                        strList = new List<string>();
                        foreach (var item in strArray)
                        {
                            if (item != "")
                                strList.Add(item);
                        }
                        aTriangle.P3 = new PointD()
                        {
                            X = Convert.ToDouble(strList[1]),
                            Y = Convert.ToDouble(strList[2]),
                            Z = Convert.ToDouble(strList[3])
                        };
                        //捨棄
                        stream.ReadLine();
                        //捨棄
                        stream.ReadLine();


                        if (TriangleFilter != null)
                        {
                            if (TriangleFilter(aTriangle))
                                aSTL.AddEntity(aTriangle);
                        }
                        else
                        {
                            aSTL.AddEntity(aTriangle);
                        }

                    }
                }
                newSTL = aSTL;
            }
            catch 
            {
                newSTL = null;
            }

            stream.Dispose();
            stream.Close();

            if (newSTL.Entities.Count == 0)
                newSTL = null;

            return newSTL;

        }

        public static STL ReadSTL_Binary(String fileName, Predicate<Triangle> TriangleFilter = null)
        {
            BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open), Encoding.ASCII);
            STL aSTL = new STL() { Color = System.Drawing.Color.Gray };
            
            try
            {
                //Read Header
                String header = new String(reader.ReadChars(80));

                //Read Triangle Number
                uint TriangleNumber = reader.ReadUInt32();
                
                //Read Triangle
                while (true)
                {
                    Triangle aTriangle = new Triangle() { Color = System.Drawing.Color.Transparent };

                    double[] Normal = new double[3];
                    for (int i = 0; i < 3; i++)
                    {
                        Tuple<bool, float> tryReadResult = reader.TryReadSingle();
                        if (tryReadResult.Item1 == true)
                            Normal[i] = tryReadResult.Item2;
                        else
                        {
                            reader.Close();
                            return aSTL;
                        }
                    }
                    double[] vertex1 = new double[3];
                    for (int i = 0; i < 3; i++)
                    {
                        Tuple<bool, float> tryReadResult = reader.TryReadSingle();
                        if (tryReadResult.Item1 == true)
                            vertex1[i] = tryReadResult.Item2;
                        else
                        {
                            reader.Close();
                            return aSTL;
                        }
                    }
                    double[] vertex2 = new double[3];
                    for (int i = 0; i < 3; i++)
                    {
                        Tuple<bool, float> tryReadResult = reader.TryReadSingle();
                        if (tryReadResult.Item1 == true)
                            vertex2[i] = tryReadResult.Item2;
                        else
                        {
                            reader.Close();
                            return aSTL;
                        }
                    }
                    double[] vertex3 = new double[3];
                    for (int i = 0; i < 3; i++)
                    {
                        Tuple<bool, float> tryReadResult = reader.TryReadSingle();
                        if (tryReadResult.Item1 == true)
                            vertex3[i] = tryReadResult.Item2;
                        else
                        {
                            reader.Close();
                            return aSTL;
                        }
                    }

                    Tuple<bool, UInt16> attributeByteCount = reader.TryReadUInt16();
                    if (attributeByteCount.Item1 == false)
                    {
                        reader.Close();
                        return aSTL;
                    }

                    aTriangle.N1 = new Vector(Normal);
                    aTriangle.P1 = new PointD(vertex1);
                    aTriangle.P2 = new PointD(vertex2);
                    aTriangle.P3 = new PointD(vertex3);

                    if (TriangleFilter != null)
                    {
                        if (TriangleFilter(aTriangle))
                            aSTL.AddEntity(aTriangle);
                    }
                    else
                    {
                        aSTL.AddEntity(aTriangle);
                    }
                }
            }
            catch (Exception)
            {
                aSTL = null;
            }

            if (aSTL.Entities.Count == 0)
                aSTL = null;

            return aSTL;
        }
     }
}
