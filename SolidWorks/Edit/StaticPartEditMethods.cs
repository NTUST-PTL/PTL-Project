using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using PTL.SolidWorks;
using PTL.Geometry;
using PTL.Geometry.MathModel;

namespace PTL.SolidWorks.Edit
{
    public class PartEditMethods
    {
        /// <summary>
        /// Add PolyLine To SolidWorks Part
        /// </summary>
        /// <param name="aModDoc"></param>
        /// <param name="aPolyLine"></param>
        public static void AddPolyLineToSolidWorksPart(IModelDoc2 aModDoc, PolyLine aPolyLine, ref int alreadyExistedCurveNumber)
        {
            aModDoc.InsertCurveFileBegin();
            for (int j = 0; j < aPolyLine.Points.Count; j++)
            {
                aModDoc.InsertCurveFilePoint(aPolyLine.Points[j].X / 1000.0,
                                                aPolyLine.Points[j].Y / 1000.0,
                                                aPolyLine.Points[j].Z / 1000.0);
            }
            aModDoc.InsertCurveFileEnd();
            alreadyExistedCurveNumber++;
        }

        /// <summary>
        /// Add TopoFace To SolidWorks Part
        /// </summary>
        /// <param name="aModDoc"></param>
        /// <param name="aTopoFace"></param>
        /// <returns></returns>
        public static void AddTopoFaceToSolidWorksPart(IModelDoc2 aModDoc, TopoFace aTopoFace, ref int alreadyExistedCurveNumber, ref int alreadyExistedSurfaceNumber, bool reverseLatitudeAndLongitude = false)
        {
            int curveNum = 0;
            int pointNum = 0;
            if (!reverseLatitudeAndLongitude)
            {
                curveNum = aTopoFace.Points.GetLength(0);
                pointNum = aTopoFace.Points.GetLength(1);

                for (int i = 0; i < curveNum; i++)
                {
                    aModDoc.InsertCurveFileBegin();
                    for (int j = 0; j < pointNum; j++)
                    {
                        aModDoc.InsertCurveFilePoint(aTopoFace.Points[i, j].X / 1000.0,
                                                    aTopoFace.Points[i, j].Y / 1000.0,
                                                    aTopoFace.Points[i, j].Z / 1000.0);
                    }
                    aModDoc.InsertCurveFileEnd();
                    alreadyExistedCurveNumber++;
                }

                aModDoc.ClearSelection2(true);
                for (int i = 0; i < curveNum; i++)
                {
                    int curveIndex = alreadyExistedCurveNumber - curveNum + 1 + i;
                    aModDoc.Extension.SelectByID2("曲線" + curveIndex, "REFERENCECURVES",
                        aTopoFace.Points[i, 0].X / 1000.0,
                        aTopoFace.Points[i, 0].Y / 1000.0,
                        aTopoFace.Points[i, 0].Z / 1000.0,
                        true, 0, null, 0);
                }
                aModDoc.InsertLoftRefSurface2(false, true, false, 1, 6, 6);
                alreadyExistedSurfaceNumber++;
            }
            else
            {
                curveNum = aTopoFace.Points.GetLength(1);
                pointNum = aTopoFace.Points.GetLength(0);
                for (int j = 0; j < curveNum; j++)
                {
                    aModDoc.InsertCurveFileBegin();
                    for (int i = 0; i < pointNum; i++)
                    {
                        aModDoc.InsertCurveFilePoint(aTopoFace.Points[i, j].X / 1000.0,
                                                    aTopoFace.Points[i, j].Y / 1000.0,
                                                    aTopoFace.Points[i, j].Z / 1000.0);
                    }
                    aModDoc.InsertCurveFileEnd();
                    alreadyExistedCurveNumber++;
                }

                aModDoc.ClearSelection2(true);
                for (int j = 0; j < curveNum; j++)
                {
                    int curveIndex = alreadyExistedCurveNumber - curveNum + 1 + j;
                    aModDoc.Extension.SelectByID2("曲線" + curveIndex, "REFERENCECURVES",
                        aTopoFace.Points[0, j].X / 1000.0,
                        aTopoFace.Points[0, j].Y / 1000.0,
                        aTopoFace.Points[0, j].Z / 1000.0,
                        true, 0, null, 0);
                }
                aModDoc.InsertLoftRefSurface2(false, true, false, 1, 6, 6);
                alreadyExistedSurfaceNumber++;
            }
        }

        /// <summary>
        /// Add TopoFace As Mesh Curve To SolidWorks Part
        /// </summary>
        /// <param name="aModDoc"></param>
        /// <param name="aTopoFace"></param>
        public void AddTopoFaceToSolidWorksPartInPolyLine(IModelDoc2 aModDoc, TopoFace aTopoFace, ref int alreadyExistedCurveNumber)
        {
            for (int j = 0; j < aTopoFace.Points.GetLength(1); j++)
            {
                aModDoc.InsertCurveFileBegin();
                for (int i = 0; i < aTopoFace.Points.GetLength(0); i++)
                {
                    aModDoc.InsertCurveFilePoint(aTopoFace.Points[i, j].X / 1000.0,
                                                aTopoFace.Points[i, j].Y / 1000.0,
                                                aTopoFace.Points[i, j].Z / 1000.0);
                }
                aModDoc.InsertCurveFileEnd();
                alreadyExistedCurveNumber++;
            }
            for (int i = 0; i < aTopoFace.Points.GetLength(0); i++)
            {
                aModDoc.InsertCurveFileBegin();
                for (int j = 0; j < aTopoFace.Points.GetLength(1); j++)
                {
                    aModDoc.InsertCurveFilePoint(aTopoFace.Points[i, j].X / 1000.0,
                                                aTopoFace.Points[i, j].Y / 1000.0,
                                                aTopoFace.Points[i, j].Z / 1000.0);
                }
                aModDoc.InsertCurveFileEnd();
                alreadyExistedCurveNumber++;
            }
        }
    }
}
