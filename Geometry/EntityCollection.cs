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
using static PTL.Mathematics.BasicFunctions;

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
            if (Entitys == null)
                return;

            foreach (var aEntity in Entitys)
            {
                if (aEntity != null)
                {
                    aEntity.Parent = this;
                }
            }
        }

        public virtual void RemoveEntity(params Entity[] Entitys)
        {
            if (Entitys == null)
                return;

            foreach (var aEntity in Entitys)
            {
                if (aEntity != null)
                    if (this.Entities.ContainsValue(aEntity))
                    {
                        aEntity.Parent = null;
                    }
            }
        }

        public virtual IEnumerable<Entity> EntitiesDeepSearch(Predicate<Entity> predicate)
        {
            foreach (var item in Entities.Values)
            {
                if (predicate(item))
                {
                    yield return item;
                }
                else if(item is EntityCollection)
                {
                    foreach (var item2 in ((EntityCollection)item).EntitiesDeepSearch(predicate))
                    {
                        yield return item2;
                    }
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
                    {
                        (aEntity as ICanPlotInOpenGL).PlotInOpenGL();
                    }
                }
                if (this.CoordinateSystem != null)
                {
                    GL.glPopMatrix();
                }
            }
        }
        
        public override XYZ4[] GetBoundary(double[,] externalCoordinateMatrix)
        {
            double[,] M = Dot(externalCoordinateMatrix, this.CoordinateSystem);
            return GetTotalBoundary(this.Entities.Values.ToArray(), M);
        }

        public static XYZ4[] GetTotalBoundary(Entity[] entities, double[,] externalCoordinateMatrix)
        {
            double[,] M = externalCoordinateMatrix;

            XYZ4[] boundary = new XYZ4[2] { new XYZ4(), new XYZ4() };
            if (entities.Length != 0)
            {
                boundary = entities[0].GetBoundary(M);
                foreach (Entity aEntity in entities)
                {
                    XYZ4[] eboundary = aEntity.GetBoundary(M);
                    Compare_Boundary(boundary, eboundary[0]);
                    Compare_Boundary(boundary, eboundary[1]);
                }
            }
            return boundary;
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
