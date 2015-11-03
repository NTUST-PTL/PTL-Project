using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using PTL.Definitions;
using PTL.Geometry.MathModel;
using PTL.OpenGL.Plot;
using CsGL.OpenGL;

namespace PTL.Geometry
{
    public abstract class EntityCollection : Entity, ICloneable, ICanPlotInOpenGL, IHaveCoordinateSystem, IHaveBoundary, IHaveColor, IToDXFEntities
    {
        public Dictionary<String, Entity> Entities = new Dictionary<String, Entity>();

        public EntityCollection()
        {
            this.Name = "EntityCollection";
        }

        public virtual void AddEntity(params Entity[] Entitys)
        {
            foreach (var aEntity in Entitys)
            {
                if (aEntity != null)
                {
                    aEntity.Parent = this;
                    if (boundary != null)
                        UpdateBoundary(Entitys);
                    else
                        CheckBoundary();
                }
            }
        }

        public virtual void RemoveEntity(params Entity[] Entitys)
        {
            foreach (var aEntity in Entitys)
            {
                if (aEntity != null)
                    if (this.Entities.ContainsValue(aEntity))
                    {
                        aEntity.Parent = null;
                        CheckBoundary();
                    }
            }
        }

        public override void PlotInOpenGL()
        {
            if (this.Visible == true)
            {
                if (this.CoordinateSystem != null)
                {
                    GL.glPushMatrix();
                    GL.glMatrixMode(GL.GL_MODELVIEW);
                    MultMatrixd(this.CoordinateSystem);
                }

                foreach (Entity aEntity in this.Entities.Values)
                {
                    if (aEntity is ICanPlotInOpenGL)
                        (aEntity as ICanPlotInOpenGL).PlotInOpenGL();
                }
                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }

        public XYZ4[] boundary;
        public override XYZ4[] Boundary
        {
            get
            {
                if (this.CoordinateSystem == null)
                    return boundary;
                else
                {
                    return Transport(this.CoordinateSystem, boundary).ToArray();
                }
            }
        }

        public void UpdateBoundary(params Entity[] entities)
        {
            XYZ4[] newboundary = this.boundary;
            if (Entities.Count != 0)
            {
                XYZ4[] eboundary;
                foreach (Entity aEntity in entities)
                {
                    eboundary = aEntity.Boundary;
                    Compare_Boundary(newboundary, eboundary[0]);
                    Compare_Boundary(newboundary, eboundary[1]);
                }
            }
            this.boundary = newboundary;
        }

        public void CheckBoundary()
        {
            XYZ4[] boundary = new XYZ4[2] { new XYZ4(), new XYZ4() };
            if (Entities.Count != 0)
            {
                Entity[] entities = Entities.Values.ToArray();
                if (this.CoordinateSystem != null)
                {
                    boundary = Transport(this.CoordinateSystem, entities[0].Boundary).ToArray();
                    XYZ4[] eboundary;
                    foreach (Entity aEntity in entities)
                    {
                        eboundary = Transport(this.CoordinateSystem, aEntity.Boundary).ToArray();
                        Compare_Boundary(boundary, eboundary[0]);
                        Compare_Boundary(boundary, eboundary[1]);
                    }
                }
                else
                {
                    boundary = entities[0].Boundary;
                    XYZ4[] eboundary;
                    foreach (Entity aEntity in entities)
                    {
                        eboundary = aEntity.Boundary;
                        Compare_Boundary(boundary, eboundary[0]);
                        Compare_Boundary(boundary, eboundary[1]);
                    }
                }
            }
            this.boundary = boundary;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            foreach (var item in this.Entities.Values)
            {
                if (item is Triangle)
                {
                    Triangle tri = (Triangle)item;
                    if (tri.P1 == null || tri.P2 == null || tri.P3 == null)
                    {
                        Console.WriteLine("Point is null : " + Entities.Values.ToList().IndexOf(tri));
                    }
                }
                else
                {

                }
                item.Transform(TransformMatrix);
            }
        }

        public List<netDxf.Entities.EntityObject> ToDXFEntities()
        {
            List<netDxf.Entities.EntityObject> dxfEntities = (from entity in this.Entities.Values
                                                             where entity is IToDXFEntity
                                                             select ((IToDXFEntity)entity).ToDXFEntity()).ToList();
            List<IToDXFEntities> entityCollections = (from entity in this.Entities.Values
                                                      where entity is IToDXFEntities
                                                      select (IToDXFEntities)entity).ToList();
            foreach (var item in entityCollections)
            {
                dxfEntities.AddRange(item.ToDXFEntities());
            }
            return dxfEntities;
        }
    }
}
