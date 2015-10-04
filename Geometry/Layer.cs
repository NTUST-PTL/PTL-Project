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
    public class Layer : EntityCollection, ICanBeWritedToDXFFile
    {
        public Layer(String Name)
            : base()
        {
            this.Name = Name;
            this.Visible = true;
        }
        public Layer()
            : base()
        {
            this.Name = "EntityCollection";
            this.Visible = true;
        }

        #region IDXFMember 成員

        public void WriteToFileInDxfFormat(StreamWriter sw)
        {
            foreach (ICanBeWritedToDXFFile item in Entities.Values)
                item.WriteToFileInDxfFormat(sw);
        }

        #endregion

        public override object Clone()
        {
            Layer aDXFLayer = new Layer();
            if (this.Name != null)
                aDXFLayer.Name = this.Name;
            aDXFLayer.Visible = this.Visible;
            aDXFLayer.Color = this._color;
            Entity[] entities = this.Entities.Values.ToArray();
            for (int i = 0; i < entities.Length; i++)
                aDXFLayer.AddEntity(entities[i].Clone() as Entity);
            return aDXFLayer;
        }

        public Tuple<netDxf.Tables.Layer, List<netDxf.Entities.EntityObject>> ToDxfEntitiesAndLayer()
        {
            List<netDxf.Entities.EntityObject> entities = this.ToDXFEntities();
            netDxf.Tables.Layer dxfLayer = new netDxf.Tables.Layer(this.Name) { Color = new netDxf.AciColor(this.Color) };
            if (this._color.A == 255
                && this._color.R == 255
                && this._color.G == 255
                && this._color.B == 255)
            {
                dxfLayer.Color = new netDxf.AciColor(7);
            }
            foreach (var item in entities)
            {
                item.Layer = dxfLayer;
            }
            return new Tuple<netDxf.Tables.Layer, List<netDxf.Entities.EntityObject>>(dxfLayer, entities);
        }
    }
}
