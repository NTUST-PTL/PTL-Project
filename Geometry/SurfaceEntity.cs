using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PTL.Geometry
{
    public abstract class SurfaceEntity : Entity, ISurfaceEntity, IHaveColor, IHaveSurfaceDisplayOptions
    {
        private SurfaceDisplayOptions surfaceDisplayOption = SurfaceDisplayOptions.Surface;
        public virtual SurfaceDisplayOptions SurfaceDisplayOption
        {
            get
            {
                if (this.surfaceDisplayOption == SurfaceDisplayOptions.Null && this.Parent != null)
                    if (this.Parent is IHaveSurfaceDisplayOptions)
                        return (this.Parent as IHaveSurfaceDisplayOptions).SurfaceDisplayOption;
                return this.surfaceDisplayOption;
            }
            set { this.surfaceDisplayOption = value; }
        }

        private double _EdgeLinWidth = 1;
        public double EdgeLineWidth
        {
            get { return _EdgeLinWidth; }
            set
            {
                if (value != _EdgeLinWidth)
                {
                    _EdgeLinWidth = value;
                    NotifyChanged(nameof(EdgeLineWidth));
                }
            }
        }
        private double _MeshLinWidth = 1;
        public double MeshLineWidth
        {
            get { return _MeshLinWidth; }
            set
            {
                if (value != _MeshLinWidth)
                {
                    _MeshLinWidth = value;
                    NotifyChanged(nameof(MeshLineWidth));
                }
            }
        }

        public abstract void PlotEdge();

        public abstract void PlotMesh();

        public abstract void PlotNormal();

        public abstract void PlotFace();
    }
}
