using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using PTL.Geometry;
using PTL.Geometry.MathModel;

namespace PTL.Windows.Media.Media3D
{
    public class FakeLineGeometryModel3D
    {
        public GeometryModel3D Model;
        public PolyLine Bone;
        public ReshreshModelMeshFunction ReshreshModelMesh;
        public delegate void ReshreshModelMeshFunction(XYZ3 look, XYZ3 up, double range, int horizaontalPixel);
    }
}
