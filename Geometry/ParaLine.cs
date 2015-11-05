using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using CsGL.OpenGL;
using PTL.Definitions;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class ParaLine : PolyLine
    {
        protected Func<double, PointD> function;
        protected Func<double, PointD> transformedFunction;
        public Func<double, PointD> Function
        {
            get { return this.function; }
            set
            {
                this.function = null;
                this.function = value;
            }
        }
        protected List<double> tt;
        public List<double> TT
        {
            get { return tt; }
            set
            {
                if (this.tt != value)
                {
                    this.tt = value;
                }
            }
        }

        public virtual void RenderGeomatry()
        {
            if (this.CoordinateSystem != null)
                this.transformedFunction = (double t) =>
                {
                    PointD p = this.function(t);
                    p.Transform(this.CoordinateSystem);
                    return p;
                };
            else
                this.transformedFunction = this.function;
            this.Points = new List<XYZ4>();
            foreach (var t in this.TT)
                this.Points.Add(transformedFunction(t));
        }

        public override object Clone()
        {
            ParaLine aParaLine = new ParaLine()
            {
                Color = this._color,
                LineType = this.LineType,
                LineWidth = this.LineWidth,
                LineTypefactor = this.LineTypefactor,
                Function = this.Function
            };
            aParaLine.TT = new List<double>();
            foreach (var t in this.TT)
                aParaLine.TT.Add(t);
            aParaLine.Points = new List<XYZ4>();
            for (int i = 0; i < Points.Count; i++)
                aParaLine.Points.Add(this.Points[i].Clone() as XYZ4);
            return aParaLine;
        }

        public override void Transform(double[,] TransformMatrix)
        {
            Func<double, PointD> function2;
            if (TransformMatrix != null)
                function2 = (double t) =>
                {
                    PointD p = this.function(t);
                    p.Transform(this.CoordinateSystem);
                    return p;
                };
            else
                function2 = this.function;
            this.Function = function2;
        }
    }
}
