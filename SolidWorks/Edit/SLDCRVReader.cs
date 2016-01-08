using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using PTL.Geometry;
using PTL.Geometry.MathModel;

namespace WindowsFormsApplication1
{
    public class SLDCRVReader
    {
        public static List<PolyLine> OpenRead()
        {
            List<PolyLine> curves = null;

            #region 讀檔
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                curves = new List<PolyLine>();
                for (int zz = 0; zz < openFileDialog1.FileNames.GetLength(0); zz++)
                {
                    System.IO.StreamReader myFile = new System.IO.StreamReader(openFileDialog1.FileNames[zz]);

                    string myString1;
                    string[] myString2;
                    int kk = 0;
                    while ((myString1 = myFile.ReadLine()) != null) kk++;//算行數
                    PolyLine pl = new PolyLine();

                    myFile.BaseStream.Seek(0, 0);//回第一行

                    for (int ii = 0; ii < kk; ii++)//存點
                    {
                        myString1 = myFile.ReadLine();
                        myString2 = myString1.Split(new Char[] { ',' });
                        XYZ4 p = new XYZ4();
                        p.X = Convert.ToDouble(myString2[0]);
                        p.Y = Convert.ToDouble(myString2[1]);
                        p.Z = Convert.ToDouble(myString2[2]);
                        pl.AddPoint(p);
                    }
                    curves.Add(pl);
                    myFile.Close();
                }
            }

            #endregion
            return curves;
        }
    }
}
