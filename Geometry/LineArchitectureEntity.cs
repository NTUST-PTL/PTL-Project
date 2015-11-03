using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Definitions;

namespace PTL.Geometry
{
    public abstract class LineArchitectureEntity : Entity, IHaveColor, ILineArchitecture
    {
        private LineType fLineType = LineType.Solid;
        private float fLineWidth = 0.8f;
        private int fLineTypefactor = 3;

        public virtual LineType LineType
        {
            get
            {
                if (this.fLineType == LineType.Null && this.Parent != null)
                    if (this.Parent is ILineArchitecture)
                        return (this.Parent as ILineArchitecture).LineType;
                return this.fLineType;
            }
            set { this.fLineType = value; }
        }
        public virtual float LineWidth
        {
            get
            {
                if (this.fLineWidth <= 0 && this.Parent != null)
                    if (this.Parent is ILineArchitecture)
                        return (this.Parent as ILineArchitecture).LineWidth;
                return this.fLineWidth;
            }
            set { this.fLineWidth = value; }
        }
        public virtual int LineTypefactor
        {
            get
            {
                if (this.fLineTypefactor <= 0 && this.Parent != null)
                    if (this.Parent is ILineArchitecture)
                        return (this.Parent as ILineArchitecture).LineTypefactor;
                return this.fLineTypefactor;
            }
            set { this.fLineTypefactor = value; }
        }
    }
}
