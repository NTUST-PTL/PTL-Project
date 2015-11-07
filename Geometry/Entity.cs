using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PTL.OpenGL.Plot;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public abstract class Entity : PlotSub, IHaveBoundary, IHaveName, ICloneable, IHaveCoordinateSystem, ICanPlotInOpenGL, IHaveColor, IHaveParent, ITransformable
    {
        public virtual String Name { get; set; }
        private EntityCollection fParent;
        public virtual EntityCollection Parent
        {
            get
            {
                return fParent;
            }
            set
            {
                if (value != null)
                {
                    if (this.fParent != value)
                    {
                        if (this.fParent != null)
                            this.fParent.Entities.Remove(this.Name);
                        if (this.Name != null)
                        {
                            if (!value.Entities.ContainsKey(this.Name))
                                value.Entities.Add(this.Name, this);
                            else
                            {
                                int i = 1;
                                while (value.Entities.ContainsKey(this.Name + "-" + i))
                                    i++;
                                this.Name = this.Name + "-" + i;
                                value.Entities.Add(this.Name, this);
                            }
                        }
                        else
                        {
                            this.Name = this.GetType().Name.ToString() + value.Entities.Count;
                            value.Entities.Add(this.Name, this);
                        }
                        this.fParent = value;
                    }
                }
                else
                {
                    this.fParent.Entities.Remove(this.Name);
                    this.fParent = value;
                }
            }
        }
        bool? visible = true;
        public virtual bool? Visible
        {
            get
            {
                if (this.visible == null && this.Parent != null)
                    return Parent.Visible;
                return this.visible;
            }
            set
            {
                if (this.visible != value)
                {
                    this.visible = value;
                }
            }
        }
        public Color _color = Color.Black;
        public virtual Color Color
        {
            get
            {
                if (this._color.A == 0 && this.Parent != null)
                    if (this.Parent is IHaveColor)
                        return (this.Parent as IHaveColor).Color;
                return this._color;
            }
            set
            {
                if (this._color != value)
                {
                    this._color = value;
                }
            }
        }
        double[,] coordinateSystem;
        public virtual double[,] CoordinateSystem
        {
            get { return this.coordinateSystem; }
            set
            {
                if (this.coordinateSystem != value &&
                    (value == null || value.GetLength(0) == 4 && value.GetLength(1) == 4))
                {
                    this.coordinateSystem = value;
                }
            }
        }
        public abstract XYZ4[] GetBoundary(double[,] externalCoordinateMatrix);

        public abstract object Clone();

        public abstract void PlotInOpenGL();

        public abstract void Transform(double[,] TransformMatrix);

        public virtual void AbsorbCoordinateSystem()
        {
            Transform(this.coordinateSystem);
            this.coordinateSystem = null;
        }
    }
}
