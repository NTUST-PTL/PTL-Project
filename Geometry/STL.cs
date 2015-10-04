using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using PTL.Base;
using PTL.Definitions;
using PTL.Geometry.MathModel;
using PTL.OpenGL.Plot;
using CsGL.OpenGL;

namespace PTL.Geometry
{
    public class STL : EntityCollection
    {
        public STL()
            : base()
        {
            
        }

        public override void AddEntity(params Entity[] Entities)
        {
            foreach (var aEntity in Entities)
            {
                if (aEntity is Triangle)
                {
                    aEntity.Parent = this;
                }
            }
        }

        public override void RemoveEntity(params Entity[] Entities)
        {
            foreach (var aEntity in Entities)
            {
                if (this.Entities.ContainsValue(aEntity))
                {
                    aEntity.Parent = null;
                }
            }
            
        }

        public override object Clone()
        {
            STL aSTL = new STL() { Name = this.Name, Visible = this.Visible, CoordinateSystem = this.CoordinateSystem, Color = this._color};
            Entity[] entities = this.Entities.Values.ToArray();
            for (int i = 0; i < entities.Length; i++)
                aSTL.AddEntity(entities[i].Clone() as Entity);

            return aSTL;
        }
    }
}
