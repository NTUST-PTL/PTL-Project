using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Data;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry
{
    public class CompoundSurface : EntityCollection, ISurfaceEntity
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

        private double _EdgeLineWidth = 1;
        public double EdgeLineWidth
        {
            get { return _EdgeLineWidth; }
            set
            {
                if (value != _EdgeLineWidth)
                {
                    _EdgeLineWidth = value;
                    NotifyChanged(nameof(EdgeLineWidth));
                }
            }
        }
        private double _MeshLineWidth = 1;
        public double MeshLineWidth
        {
            get { return _MeshLineWidth; }
            set
            {
                if (value != _MeshLineWidth)
                {
                    _MeshLineWidth = value;
                    NotifyChanged(nameof(MeshLineWidth));
                }
            }
        }

        public override object Clone()
        {
            var newCS = new CompoundSurface();
            DataCopy.CopyByReflection(newCS, this);
            return newCS;
        }

        public override void PlotInOpenGL()
        {
            switch (SurfaceDisplayOption)
            {
                case SurfaceDisplayOptions.Null:
                    break;
                case SurfaceDisplayOptions.Surface:
                    PlotFace();
                    break;
                case SurfaceDisplayOptions.Mesh:
                    PlotMesh();
                    break;
                case SurfaceDisplayOptions.Edge:
                    PlotEdge();
                    break;
                case SurfaceDisplayOptions.SurfaceAndEdge:
                    PlotFace();
                    PlotEdge();
                    break;
                case SurfaceDisplayOptions.SurfaceAndMesh:
                    PlotFace();
                    PlotMesh();
                    break;
                default:
                    break;
            }
        }

        public void PlotEdge()
        {
            
        }

        public void PlotFace()
        {
            foreach (var item in this.Entities.Values.Cast<ISurfaceEntity>())
            {
                item.PlotFace();
            }
        }

        public void PlotMesh()
        {
            foreach (var item in this.Entities.Values.Cast<ISurfaceEntity>())
            {
                item.PlotMesh();
            }
        }

        public void PlotNormal()
        {
            foreach (var item in this.Entities.Values.Cast<ISurfaceEntity>())
            {
                item.PlotNormal();
            }
        }
    }
}
