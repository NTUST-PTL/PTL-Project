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
    public class Part : EntityCollection, IDXF, IToDXFDocument
    {
        #region SystemColor_DXFColor
        public static Dictionary<Color, int> SystemColor_DXFColor =
            new Dictionary<Color, int>()
            {{Color.White, 7},
             {Color.Red, 1},
             {Color.Orange, 30},
             {Color.DarkOrange, 32},
             {Color.LawnGreen, 3},
             {Color.SkyBlue, 130},
             {Color.DeepSkyBlue, 140},
             {Color.Blue, 5},
             {Color.AntiqueWhite, 7}};
        #endregion SystemColor_DXFColor

        public Part()
            :base()
        {
        }

        public override object Clone()
        {
            Part newPart = new Part();
            newPart.Color = this._color;
            if (this.Name != null)
                newPart.Name = this.Name;
            Entity[] entities = this.Entities.Values.ToArray();
            for (int i = 0; i < entities.Length; i++)
                newPart.AddEntity((Entity)entities[i].Clone());
            return newPart;
        }

        public static int SystemColor2DXFColorIndex(Color color)
        {
            int colorIndex = 256;
            if (SystemColor_DXFColor.ContainsKey(color))
                colorIndex = SystemColor_DXFColor[color];//顏色號碼
            return colorIndex;
        }

        #region IDXF 成員
        public void Save2DxfFile(String filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            StreamWriter sw = fileInfo.CreateText();
            sw.Close();
            sw = new StreamWriter(filename, true, Encoding.ASCII);

            sw.WriteLine("0");
            sw.WriteLine("SECTION");
            sw.WriteLine("2");
            sw.WriteLine("ENTITIES");
            List<Entity> allEntities = this.Entities.Values.ToList();
            for (int i = 0; i < allEntities.Count; i++)
            {
                if (allEntities[i] is Layer)
                    (allEntities[i] as Layer).WriteToFileInDxfFormat(sw);
            }
            sw.WriteLine("0");
            sw.WriteLine("ENDSEC");
            //檔案結尾標記
            sw.WriteLine("0");
            sw.WriteLine("EOF");

            sw.Flush();
            sw.Close();
        }
        #endregion

        public netDxf.DxfDocument ToDXFDocument()
        {
            XYZ4[] boundary = GetBoundary(null);
            PointD sizeP = boundary[1] - boundary[0];
            double longLength = sizeP.ToArray().Max();
            netDxf.DxfDocument doc = new netDxf.DxfDocument(netDxf.Header.DxfVersion.AutoCad2007);

            //標註設定
            netDxf.Tables.DimensionStyle StandardDimStyle = doc.DimensionStyles["Standard"];
            StandardDimStyle.DIMDEC = (short)3;
            StandardDimStyle.DIMASZ = longLength / 80;
            StandardDimStyle.DIMTXT = longLength / 120;

            //依圖層
            List<Layer> allLayers = (from entity in this.Entities.Values
                                     where entity is Layer
                                     select (Layer)entity).ToList();
            foreach (var layer in allLayers)
            {
                Tuple<netDxf.Tables.Layer, List<netDxf.Entities.EntityObject>> LayerAndEntities = layer.ToDxfEntitiesAndLayer();
                netDxf.Tables.Layer dxfLayer = LayerAndEntities.Item1;
                List<netDxf.Entities.EntityObject> entities = LayerAndEntities.Item2;
                //doc.Layers.Add(dxfLayer);
                foreach (var item in entities)
                {
                    if (item is netDxf.Entities.Dimension)
                        (item as netDxf.Entities.Dimension).Style = StandardDimStyle;
                    doc.AddEntity(item);
                }
            }
            

            //依元素
            netDxf.Tables.Layer defualtLayer = new netDxf.Tables.Layer("defualt");
            List<IToDXFEntity> Entities = (from entity in this.Entities.Values
                                                  where entity is IToDXFEntity
                                                  select (IToDXFEntity)entity).ToList();
            foreach (var entity in Entities)
            {
                netDxf.Entities.EntityObject dxfEntity = entity.ToDXFEntity();
                if (dxfEntity != null)
                {
                    dxfEntity.Layer = defualtLayer;
                    doc.AddEntity(dxfEntity); 
                }
            }
            List<IToDXFEntities> MultEntities = (from entity in this.Entities.Values
                                             where entity is IToDXFEntities
                                             select (IToDXFEntities)entity).ToList();
            foreach (var entity in MultEntities)
            {
                foreach (netDxf.Entities.EntityObject item in entity.ToDXFEntities())
                {
                    item.Layer = defualtLayer;
                    doc.AddEntity(item);
                }
            }

            return doc;
        }
    }
}
