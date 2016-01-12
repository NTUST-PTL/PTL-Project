using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using PTL.SolidWorks;
using PTL.Geometry.MathModel;
using PTL.Geometry;
using PTL.SolidWorks.GearConstruction;

namespace WindowsFormsApplication1
{
    public class APIFileReader
    {
        public static GearData OpenRead()
        {
            GearData newGearData = null;

            #region 讀檔
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String[] splited = openFileDialog1.FileName.Split('.');
                if (splited.Last().ToLower() == "json")
                {
                    newGearData = OpenRead_json(openFileDialog1.FileName);
                }
                else
                {
                    newGearData = OpenRead_txt(openFileDialog1.FileName);
                }
            }
            #endregion

            return newGearData;
        }

        public static GearData OpenRead_json(String filename)
        {
            GearData newGearData = PTL.FileOperation.Json.ReadJsonFile<GearData>(filename);
            return newGearData;
        }

        [System.Obsolete("舊格式，建議改用Json較為方便。")]
        public static GearData OpenRead_txt(String filename)
        {
            GearData newGearData = null;

            newGearData = new GearData();
            System.IO.StreamReader myFile = new System.IO.StreamReader(filename);

            string myString1;
            string[] myString2;


            myFile.BaseStream.Seek(0, 0);//回第一行
            String axisStr = myFile.ReadLine();
            newGearData.Axis = (PTL.Definitions.Axis)Enum.Parse(typeof(PTL.Definitions.Axis), axisStr);

            myString1 = myFile.ReadLine();
            myString2 = myString1.Split(new Char[] { ',' });
            int toothNumber = Convert.ToInt16(myString2[0]);
            int blankPointNumber = Convert.ToInt16(myString2[1]);
            int surfaceNumber = Convert.ToInt16(myString2[2]);

            //齒數
            newGearData.TeethNumber = Convert.ToInt16(myString2[0]);
            //齒胚點
            newGearData.BlankLines = new List<List<double[]>>();
            List<XYZ4> blankPoints = new List<XYZ4>();
            for (int ii = 0; ii < blankPointNumber; ii++)
            {
                myString1 = myFile.ReadLine();
                myString2 = myString1.Split(new Char[] { ',' });

                XYZ4 p = new XYZ4();
                p.X = Convert.ToDouble(myString2[1]);
                p.Y = Convert.ToDouble(myString2[2]);
                p.Z = Convert.ToDouble(myString2[3]);
                blankPoints.Add(p);
            }
            for (int i = 0; i < blankPointNumber - 1; i++)
            {
                newGearData.BlankLines.Add(new List<double[]> { blankPoints[i], blankPoints[i + 1] });
            }
            newGearData.BlankLines.Add(new List<double[]> { blankPoints.Last(), blankPoints.First() });



            newGearData.ToothFaces = new List<double[,][]>();
            for (int ii = 0; ii < surfaceNumber; ii++)//存拓普點
            {
                myString1 = myFile.ReadLine();
                myString2 = myString1.Split(new Char[] { ',' });

                int CrvNum = Convert.ToInt16(myString2[0]);
                int PntNum = Convert.ToInt16(myString2[1]);

                double[,][] tps = new double[CrvNum, PntNum][];

                for (int jj = 0; jj < CrvNum; jj++)
                {
                    for (int kk = 0; kk < PntNum; kk++)
                    {
                        myString1 = myFile.ReadLine();
                        myString2 = myString1.Split(new Char[] { ',' });
                        tps[jj, kk] = new double[3];
                        tps[jj, kk][0] = Convert.ToDouble(myString2[2]);
                        tps[jj, kk][1] = Convert.ToDouble(myString2[3]);
                        tps[jj, kk][2] = Convert.ToDouble(myString2[4]);
                    }
                }

                newGearData.ToothFaces.Add(tps);
            }
            myFile.Close();

            return newGearData;
        }
    }
}
