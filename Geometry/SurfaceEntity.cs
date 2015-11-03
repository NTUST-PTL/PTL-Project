using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PTL.Geometry
{
    public abstract class SurfaceEntity : Entity, IHaveColor, IHaveSurfaceDisplayOptions
    {
        private SurfaceDisplayOptions surfaceDisplayOption = SurfaceDisplayOptions.SurfaceOnly;
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
    }
}
