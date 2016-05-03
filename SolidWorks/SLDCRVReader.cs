using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using PTL.Geometry;
using PTL.FileOperation;
using PTL.Geometry.MathModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PTL.SolidWorks
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
                for (int zz = 0; zz < openFileDialog1.FileNames.Length; zz++)
                {
                    if (openFileDialog1.FileNames[zz]?.Split('.').Last().Trim().ToLower() == "json")
                    {
                        curves.AddRange(
                            ParseCurveDataFromJsonReadIn(
                                Json.ReadJsonFile(openFileDialog1.FileNames[zz]))
                            );
                    }
                    else
                    {
                        System.IO.StreamReader myFile = new System.IO.StreamReader(openFileDialog1.FileNames[zz]);
                        curves.Add(ParseCurveDataFromSLDCRV(myFile));
                        myFile.Close();
                    }
                }
            }

            #endregion
            return curves;
        }

        public static List<PolyLine> ParseCurveDataFromJsonReadIn(object data)
        {
            List<PolyLine> curveList = new List<PolyLine>();

            if (data is JArray)
            {
                JArray dataArr = (JArray)data;
                if (dataArr.Type == JTokenType.Array &&
                    dataArr[1].Type == JTokenType.Array &&
                    (
                        ((JArray)dataArr[1])[1].Type == JTokenType.Float ||
                        ((JArray)dataArr[1])[1].Type == JTokenType.Integer
                    )
                    )
                {
                    var curveData = (from p in dataArr
                                     select
                                     (
                                        new XYZ4((from value in (JArray)p select value.Value<double>()).ToArray())
                                     )
                                     ).ToArray();
                    curveList.Add(new PolyLine(curveData));
                }
                else
                {
                    foreach (var itemData in dataArr)
                    {
                        var result = ParseCurveDataFromJsonReadIn(itemData);
                        curveList.AddRange(result);
                    }
                }
            }

            return curveList;
        }

        public static PolyLine ParseCurveDataFromSLDCRV(System.IO.StreamReader data)
        {
            string myString1;
            string[] myString2;
            int kk = 0;
            while ((myString1 = data.ReadLine()) != null) kk++;//算行數
            PolyLine pl = new PolyLine();

            data.BaseStream.Seek(0, 0);//回第一行

            for (int ii = 0; ii < kk; ii++)//存點
            {
                myString1 = data.ReadLine();
                myString2 = myString1.Split(new Char[] { ',' });
                XYZ4 p = new XYZ4();
                p.X = Convert.ToDouble(myString2[0]);
                p.Y = Convert.ToDouble(myString2[1]);
                p.Z = Convert.ToDouble(myString2[2]);
                pl.AddPoint(p);
            }
            return pl;
        }
    }
}
