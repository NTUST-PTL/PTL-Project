using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry
{
    public interface ISurfaceEntity
    {
        SurfaceDisplayOptions SurfaceDisplayOption { get; set; }
        double EdgeLineWidth { get; set; }
        double MeshLineWidth { get; set; }

        void PlotEdge();
        void PlotMesh();
        void PlotNormal();
        void PlotFace();
    }
}
