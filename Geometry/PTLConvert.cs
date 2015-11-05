﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public class PTLConvert
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
            double xstart, double xend, uint xslices, 
            double ystart, double yend, uint yslices, 
            Action<TopoFace> TopoFaceSetter = null)
        {
            TopoFace topoFace = null;
            if (Function != null && xslices > 0 && yslices > 0)
            {
                uint NRow = xslices + 1;
                uint NCol = yslices + 1;
                topoFace = new TopoFace() { Points = new XYZ4[NRow, NCol] };

                //確認方向
                if (xstart > xend)
                {
                    double newEnd = xstart;
                    xstart = xend;
                    xend = newEnd;
                }
                if (ystart > yend)
                {
                    double newEnd = ystart;
                    ystart = yend;
                    yend = newEnd;
                }


                double dx = (xend - xstart) / xslices;
                double dy = (yend - ystart) / yslices;
                double x = xstart;
                for (int i = 0; i < NRow; i++)
                {
                    double y = ystart;
                    for (int j = 0; j < NCol; j++)
                    {
                        topoFace.Points[i, j] = Function(x, y);
                        y += dy;
                    }
                    x += dx;
                }
                topoFace.SovleNormalVector();
                if (TopoFaceSetter != null)
                    TopoFaceSetter(topoFace);
                else
                    topoFace.Color = System.Drawing.Color.LawnGreen;
            }
            return topoFace;
        }

        public static TopoFace ToTopoFace(
            NUB_Surface nub_Surface,
            double xstart = 0, double xend = 1, uint xslices = 100,
            double ystart = 0, double yend = 1, uint yslices = 100,
            Action<TopoFace> TopoFaceSetter = null)
        {
            return ToTopoFace(
                (u, v) => nub_Surface.P(u, v),
                xstart, xend, xslices,
                ystart, yend, yslices,
                TopoFaceSetter
                );
        }

        public static PolyLine ToPolyLine(
            Func<double, XYZ4> Function,
            double start, double end, uint slices,
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
            double start, double end, uint slices,
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