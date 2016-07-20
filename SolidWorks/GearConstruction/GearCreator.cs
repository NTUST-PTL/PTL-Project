using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Definitions;
using PTL.Geometry;
using PTL.Geometry.MathModel;
using PTL.SolidWorks;
using PTL.SolidWorks.Edit;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using static System.Math;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.SolidWorks.GearConstruction
{
    public class GearCreator
    {
        public static void PublishToSolidWorks(GearData gearData)
        {
            int TotalCurveNumber = 0;
            int TotalFaceNumber = 0;

            PTL.SolidWorks.SolidWorksAppAdapter swAPP = new PTL.SolidWorks.SolidWorksAppAdapter();
            IModelDoc2 modDoc = swAPP.CreatePart();

            #region 限制條件關閉
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPoints, true);//設定限制條件
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPerpendicular, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsParallel, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVLines, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVPoints, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsLength, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsAngle, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsCenterPoints, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsMidPoints, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsQuadrantPoints, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsIntersections, false);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsNearest, true);//
            #endregion

            //建齒胚線
            List<PolyLine> BlankLines = new List<PolyLine>();
            foreach (List<double[]> item in gearData.BlankLines)
                BlankLines.Add(new PolyLine() { Points = ChangeArrayType<double[], XYZ4>(item.ToArray(), (a) => new XYZ4(a)).ToList() });
            for (int i = 0; i < BlankLines.Count; i++)
                PartEditMethods.AddPolyLineToSolidWorksPart(modDoc, BlankLines[i], ref TotalCurveNumber);

            //建立基準軸
            switch (gearData.Axis)
            {
                case Axis.X:
                    modDoc.Extension.SelectByID2("前基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
                    modDoc.Extension.SelectByID2("上基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
                    modDoc.InsertAxis2(true);
                    break;
                case Axis.Y:
                    modDoc.Extension.SelectByID2("前基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
                    modDoc.Extension.SelectByID2("右基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
                    modDoc.InsertAxis2(true);
                    break;
                case Axis.Z:
                    modDoc.Extension.SelectByID2("上基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
                    modDoc.Extension.SelectByID2("右基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
                    modDoc.InsertAxis2(true);
                    break;
                default:
                    break;
            }

            //建立齒胚
            #region 建立齒胚
            //判斷草圖平面
            XYZ3 v1 = (XYZ3)gearData.BlankLines[0][1] - (XYZ3)gearData.BlankLines[0][0];
            XYZ3 v2 = (XYZ3)gearData.BlankLines[1][1] - (XYZ3)gearData.BlankLines[1][0];
            XYZ3 normal = Normalize(Cross(v1, v2));
            //選擇草圖平面
            
            String sketchPlan;
            if (Abs(Abs(normal.X) - 1) < 1e-10)
                sketchPlan = "右基準面";
            else if (Abs(Abs(normal.Y) - 1) < 1e-10)
                sketchPlan = "上基準面";
            else
                sketchPlan = "前基準面";
            modDoc.ClearSelection2(true);
            modDoc.Extension.SelectByID2(sketchPlan, "PLANE", 0, 0, 0, true, 0, null, 0);
            modDoc.InsertSketch2(true);//執行草圖繪製-開始
            modDoc.ClearSelection2(true);

            modDoc.ClearSelection2(true);
            for (int i = (TotalCurveNumber - BlankLines.Count + 1); i <= TotalCurveNumber; i++)
                modDoc.Extension.SelectByID2("曲線" + i, "REFERENCECURVES", 0, 0, 0, true, 0, null, 0);
            modDoc.InsertCompositeCurve();
            modDoc.SketchManager.SketchUseEdge2(false);

            modDoc.SetPickMode();
            modDoc.ClearSelection2(true);
            modDoc.InsertSketch2(true);//執行草圖繪製-結束

            XYZ4 selectPoint = (BlankLines[0].Points[0] + BlankLines[1].Points[1]) / 2.0;

            modDoc.Extension.SelectByID2("基準軸1", "AXIS", 0, 0, 0, true, 16, null, 0);
            modDoc.Extension.SelectByID2("草圖1", "SKETCH", selectPoint.X, selectPoint.Y, selectPoint.Z, false, 2, null, 0);
            modDoc.FeatureManager.FeatureRevolve2(true, true, false, false, false, false, 0, 0, 6.2831853071796, 0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
            #endregion

            #region point to curve 繪製齒形曲面
            foreach (var topoface in gearData.ToothFaces)
            {
                TopoFace tf = new TopoFace() { Points = ChangeArrayType<double[], XYZ4>(topoface, (a) => new XYZ4(a)) };
                tf.SolveNormalVector();
                tf.SurfaceDisplayOption = SurfaceDisplayOptions.Mesh;
                PartEditMethods.AddTopoFaceToSolidWorksPart(modDoc, tf, ref TotalCurveNumber, ref TotalFaceNumber);
                //PTL.DebugTools.Plot plot0 = new DebugTools.Plot();
                //plot0.Window.View.AutoScale = false;
                //plot0.AddSomethings(tf);
            }

            if (gearData.ToothFaces.Count > 1)
            {
                modDoc.ClearSelection2(true);
                for (int i = 1; i <= TotalFaceNumber; i++)
                {
                    bool ok = modDoc.Extension.SelectByID2("曲面-疊層拉伸" + i, "SURFACEBODY", 0, 0, 0, true, 1, null, 0);
                    ok = true;
                }
                modDoc.FeatureManager.InsertSewRefSurface(true, false, false, 0.0001, 0.0001);
            }
            #endregion point to curve 繪製齒形曲面

            #region 除料、環狀排列
            //除料
            if (gearData.ToothFaces.Count > 1)
            {
                modDoc.ClearSelection2(true);
                modDoc.Extension.SelectByID2("曲面-縫織1", "SURFACEBODY", 0, 0, 0, true, 0, null, 0);
                modDoc.InsertCutSurface(gearData.InvertCuttingSide, 0);

                //環狀排列
                modDoc.ClearSelection2(true);
                modDoc.Extension.SelectByID2("使用曲面除料1", "BODYFEATURE", 0, 0, 0, true, 4, null, 0);
                modDoc.Extension.SelectByID2("基準軸1", "AXIS", 0, 0, 0, true, 1, null, 0);
                if (modDoc.FeatureManager.FeatureCircularPattern3(gearData.TeethNumber, 6.2831853071796, false, "NULL", false, true) == null)
                {
                    modDoc.EditUndo2(1);
                    //除料
                    modDoc.ClearSelection2(true);
                    modDoc.Extension.SelectByID2("曲面-縫織1", "SURFACEBODY", 0, 0, 0, true, 0, null, 0);
                    modDoc.InsertCutSurface(true, 0);

                    //環狀排列
                    modDoc.ClearSelection2(true);
                    modDoc.Extension.SelectByID2("使用曲面除料2", "BODYFEATURE", 0, 0, 0, true, 4, null, 0);
                    modDoc.Extension.SelectByID2("基準軸1", "AXIS", 0, 0, 0, true, 1, null, 0);
                    modDoc.FeatureManager.FeatureCircularPattern3(gearData.TeethNumber, 6.2831853071796, false, "NULL", false, true);
                }
            }
            else
            {
                modDoc.ClearSelection2(true);
                modDoc.Extension.SelectByID2("曲面-疊層拉伸1", "SURFACEBODY", 0, 0, 0, true, 0, null, 0);
                modDoc.InsertCutSurface(gearData.InvertCuttingSide, 0);
            }
            #endregion 除料、環狀排列

            #region 隱藏曲線、曲面
            if (gearData.ToothFaces.Count > 1)
            {
                modDoc.ClearSelection2(true);
                modDoc.Extension.SelectByID2("曲面-縫織1", "SURFACEBODY", 0, 0, 0, false, 0, null, 0);
                modDoc.FeatureManager.HideBodies();
            }
            modDoc.ClearSelection2(true);
            for (int i = 1; i <= TotalCurveNumber; i++)
            {
                modDoc.Extension.SelectByID2("曲線" + i, "REFERENCECURVES", 0, 0, 0, false, 0, null, 0);
                modDoc.BlankRefGeom();
            }
            modDoc.ClearSelection2(true);
            for (int i = 0; i < TotalFaceNumber; i++)
            {
                modDoc.Extension.SelectByID2("曲面-疊層拉伸" + i, "REFERENCECURVES", 0, 0, 0, false, 0, null, 0);
                modDoc.BlankRefGeom();
            }
            modDoc.ClearSelection2(true);
            modDoc.Extension.SelectByID2("基準軸1", "AXIS", 0, 0, 0, false, 0, null, 0);
            modDoc.BlankRefGeom();
            #endregion

            #region 限制條件開啟
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPoints, true);//設定限制條件
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPerpendicular, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsParallel, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVLines, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVPoints, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsLength, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsAngle, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsCenterPoints, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsMidPoints, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsQuadrantPoints, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsIntersections, true);
            swAPP.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsNearest, true);//
            #endregion
        }
    }
}
