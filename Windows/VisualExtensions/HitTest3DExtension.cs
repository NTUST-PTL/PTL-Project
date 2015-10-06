using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PTL.Windows.VisualExtensions
{
    public static class HitTest3DExtension
    {
        public static RayMeshGeometry3DHitTestResult[] MultiRayMeshHitTest(this Visual3D visual3D, RayHitTestParameters[] HitTestParameters)
        {
            int HitNum = HitTestParameters.GetLength(0);
            RayMeshGeometry3DHitTestResult[] results = new RayMeshGeometry3DHitTestResult[HitNum];

            int i = 0;
            Func<HitTestResult, HitTestResultBehavior> ResultCallback = (result) =>
            #region
            {
                RayHitTestResult rayResult = result as RayHitTestResult;
                //Did we hit 3D?
                if (rayResult != null)
                {
                    RayMeshGeometry3DHitTestResult rayMeshResult = result as RayMeshGeometry3DHitTestResult;
                    //Did we hit MeshGeometry3D?
                    if (rayMeshResult != null)
                    {
                        results[i] = rayMeshResult;
                        return HitTestResultBehavior.Stop;
                    }
                }

                return HitTestResultBehavior.Continue;
            };
            #endregion
            
            for (i = 0; i < HitNum; i++)
            {
                VisualTreeHelper.HitTest(visual3D, null,new HitTestResultCallback(ResultCallback), HitTestParameters[i]);
            }

            return results;
        }
    }
}
