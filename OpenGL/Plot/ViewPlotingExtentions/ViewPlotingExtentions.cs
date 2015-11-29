using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.OpenGL;
using PTL.Geometry;
using PTL.Geometry.MathModel;

namespace PTL.OpenGL.Plot.ViewPlotingExtentions
{
    public static class ViewPlotingExtentions
    {
        public static void ParameterPlot(this OpenGLView view, Func<double, XYZ4> Function, double start, double end, uint slices, Action<PolyLine> PolyLineSetter = null)
        {
            PolyLine polyline = PTL.Geometry.PTLConvert.ToPolyLine(
                (u) => Function(u), start, end, slices, PolyLineSetter
                );

            view.AddSomeThing2Show(polyline);
        }

        public static void ParameterPlot(this OpenGLView view, Func<double, double, XYZ4> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<TopoFace> TopoFaceSetter = null)
        {
            TopoFace topoFace = PTL.Geometry.PTLConvert.ToTopoFace(
                (u, v) => Function(u, v),
                xstart, xend, xslices,
                ystart, yend, yslices,
                TopoFaceSetter);

            view.AddSomeThing2Show(topoFace);
        }

        public static void ParameterPlotBoundary(this OpenGLView view, Func<double, double, XYZ4> Function, double xstart, double xend, uint xslices, double ystart, double yend, uint yslices, Action<PolyLine> PolyLineSetter = null)
        {
            ParameterPlot(view, (u) => Function(u, ystart), xstart, xend, xslices, PolyLineSetter);
            ParameterPlot(view, (u) => Function(u, yend), xstart, xend, xslices, PolyLineSetter);
            ParameterPlot(view, (v) => Function(xstart, v), ystart, yend, yslices, PolyLineSetter);
            ParameterPlot(view, (v) => Function(xend, v), ystart, yend, yslices, PolyLineSetter);
        }
    }
}
