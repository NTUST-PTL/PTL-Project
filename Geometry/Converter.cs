using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class Converter
    {
        public static NUB_Surface ToNUB_Surface(TopoFace tf)
        {
            return ToNUB_Surface(tf.Points);
        }

        public static NUB_Surface ToNUB_Surface(XYZ4[,] tf)
        {
            int NRow = tf.GetLength(0);
            int NCol = tf.GetLength(1);
            XYZ4[][] Points = new XYZ4[NRow][];
            for (int i = 0; i < NRow; i++)
            {
                Points[i] = new XYZ4[NCol];
                for (int j = 0; j < NCol; j++)
                    Points[i][j] = tf[i, j];
            }
            return new NUB_Surface(Points);
        }

        public static TopoFace ToTopoFace(
            Func<double, double, XYZ4> Function,
            Func<double, double, XYZ3> normalFunction,
            double xstart, double xend, int xAliqouts,
            double ystart, double yend, int yAliqouts,
            Action<TopoFace> TopoFaceSetter = null)
        {
            TopoFace topoFace = null;
            if (Function != null && xAliqouts > 0 && yAliqouts > 0)
            {
                int NRow = xAliqouts + 1;
                int NCol = yAliqouts + 1;
                topoFace = new TopoFace();
                topoFace.Points = new XYZ4[NRow, NCol];
                topoFace.Normals = normalFunction != null ? new XYZ3[NRow, NCol] : null;

                double dx = (xend - xstart) / xAliqouts;
                double dy = (yend - ystart) / yAliqouts;
                double x = xstart;
                for (int i = 0; i < NRow; i++)
                {
                    double y = ystart;
                    for (int j = 0; j < NCol; j++)
                    {
                        topoFace.Points[i, j] = Function(x, y);
                        if (normalFunction != null)
                            topoFace.Normals[i, j] = normalFunction(x, y);
                        y += dy;
                    }
                    x += dx;
                }

                if (normalFunction == null)
                    topoFace.SolveNormalVector();
                if (TopoFaceSetter != null)
                    TopoFaceSetter(topoFace);
                else
                    topoFace.Color = System.Drawing.Color.LawnGreen;
            }
            return topoFace;
        }

        public static TopoFace ToTopoFace(
            Func<double, double, XYZ4> Function, 
            double xstart, double xend, int xslices, 
            double ystart, double yend, int yslices, 
            Action<TopoFace> TopoFaceSetter = null)
        {
            return ToTopoFace(
                Function,
                null,
                xstart, xend, xslices,
                ystart, yend, yslices,
                TopoFaceSetter
                );
        }

        public static TopoFace ToTopoFace(
            NUB_Surface nub_Surface,
            double xstart = 0, double xend = 1, int xslices = 100,
            double ystart = 0, double yend = 1, int yslices = 100,
            Action<TopoFace> TopoFaceSetter = null)
        {
            return ToTopoFace(
                nub_Surface.P,
                nub_Surface.n,
                xstart, xend, xslices,
                ystart, yend, yslices,
                TopoFaceSetter
                );
        }

        public static PolyLine ToPolyLine(
            Func<double, XYZ4> Function,
            double start, double end, int slices,
            Action<PolyLine> PolyLineSetter = null)
        {
            PolyLine polyline = new PolyLine();

            if (start > end)
            {
                double newEnd = start;
                start = end;
                end = newEnd;
            }
            double dx = (end - start) / slices;
            if (Function != null)
            {
                double x = start;
                for (int i = 0; i <= slices; i++)
                {
                    polyline.AddPoint(Function(x));
                    x += dx;
                }
                if (PolyLineSetter != null)
                    PolyLineSetter(polyline);
                else
                    polyline.Color = System.Drawing.Color.LawnGreen;
            }

            return polyline;
        }

        public static PolyLine ToPolyLine(
            NUB_Curve nub_curve,
            double start, double end, int slices,
            Action<PolyLine> PolyLineSetter = null)
        {
            PolyLine polyline = ToPolyLine(
                nub_curve.P,
                start, end, slices,
                PolyLineSetter
                );

            return polyline;
        }
    }
}
