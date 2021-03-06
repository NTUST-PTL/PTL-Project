﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;
using static System.Math;
using static PTL.Mathematics.BasicFunctions;
using static PTL.Geometry.Operations.PrivateMethods;

namespace PTL.Geometry
{
    public class Operations
    {
        public static TopoFace SealToRadius(
            XYZ4[] points, 
            double r, 
            PTL.Definitions.Axis axis = Definitions.Axis.Z,
            int interpolate = 0,
            bool specify_Z_Value = false,
            double axisPosition = 0,
            Action<TopoFace> TopoFaceSetter = null)
        {
            int n = points.Length + (points.Length - 1) * interpolate;
            TopoFace sealSurface = new TopoFace(n, 2);
            XYZ4[] interpolatedPoints;
            if (interpolate > 0)
            {
                interpolatedPoints = new XYZ4[n];
                for (int i = 0; i < points.Length; i++)
                {
                    if (i < points.Length - 1)
                    {
                        XYZ4 p1 = points[i];
                        XYZ4 p2 = points[i + 1];
                        for (int j = 0; j <= interpolate + 1; j++)
                        {
                            XYZ4 p = p1 + (p2 - p1) / (interpolate + 1) * j;
                            int index = i * (1 + interpolate) + j;
                            interpolatedPoints[index] = p;
                        }
                    }
                    else
                    {
                        interpolatedPoints[n - 1] = points.Last();
                    }
                }
            }
            else
            {
                interpolatedPoints = points;
            }


            for (int i = 0; i < n; i++)
            {
                XYZ4 p = interpolatedPoints[i];
                sealSurface.Points[i, 0] = p;
                if (specify_Z_Value)
                    sealSurface.Points[i, 1] = SetPosition(p, r, axisPosition, axis);
                else
                    sealSurface.Points[i, 1] = SetPosition(p, r, GetAxisPosition(p, axis), axis);
            }

            sealSurface.SolveNormalVector();
            TopoFaceSetter?.Invoke(sealSurface);
            return sealSurface;
        }

        public static TopoFace SealToRadius(
            TopoFace face, double r,
            TopoFaceEdges edge,
            PTL.Definitions.Axis axis = Definitions.Axis.Z,
            int interpolate = 0,
            bool specify_Z_Value = false,
            double axisPosition = 0,
            Action<TopoFace> TopoFaceSetter = null)
        {
            return SealToRadius(GetEdgePoints(face.Points, edge), r, axis, interpolate, specify_Z_Value, axisPosition, TopoFaceSetter);
        }

        public static TopoFace SealTwoFace(TopoFace face1, TopoFaceEdges face1Edge, TopoFace face2, TopoFaceEdges face2Edge, bool IsTagent2Face1 = false, bool IsTagent2Face2 = false)
        {
            int n1 = GetEdgePointsLength(face1.Points, face1Edge);
            int n2 = GetEdgePointsLength(face2.Points, face2Edge);

            if (n1 != n2)
            {
                throw new ArgumentException("矩陣長度不一致");
            }

            XYZ4[] edge1 = GetEdgePoints(face1.Points, face1Edge);
            XYZ4[] edge2 = GetEdgePoints(face2.Points, face2Edge);

            TopoFace tf = new TopoFace(n1, 2);
            for (int i = 0; i < n1; i++)
            {
                tf.Points[i, 0] = edge1[i];
                tf.Points[i, 1] = edge2[i];
            }

            //設定法向量
            if (!IsTagent2Face1 || !IsTagent2Face2)
            {
                tf.SolveNormalVector();
            }
            if (face1.Normals != null && IsTagent2Face1)
            {
                XYZ3[] edge_Normal = GetEdgePoints(face1.Normals, face1Edge);
                for (int i = 0; i < n1; i++)
                {
                    tf.Normals[i, 0] = edge_Normal[i];
                }
            }
            if (face1.Normals != null && IsTagent2Face1)
            {
                XYZ3[] edge_Normal = GetEdgePoints(face2.Normals, face2Edge);
                for (int i = 0; i < n1; i++)
                {
                    tf.Normals[i, 1] = edge_Normal[i];
                }
            }
            return tf;
        }

        public static List<XYZ4> MergeAndSeqauence(List<XYZ4[]> Edges, double tolerance = 1e-5)
        {
            List<List<XYZ4>> edges = Edges.Cast((XYZ4[] arr) => arr.ToList());
            List<XYZ4> merged = new List<XYZ4>();
            merged.AddRange(edges[0]);
            edges.Remove(edges[0]);
            merged = MergeAndSeqauence(merged, edges, tolerance);
            return merged;
        }

        public static List<XYZ4> MergeAndSeqauence(List<XYZ4> merged, List<List<XYZ4>> edges, double tolerance = 1e-5)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                var re = CheckMergeable(merged, edges[i]);
                if (re.IsMergeable)
                {
                    if (re.IsArr1NeedToBeReversed)
                        merged.Reverse();
                    if (re.IsArr2NeedToBeReversed)
                        edges[i].Reverse();
                    merged.AddRange(edges[i]);
                    edges.RemoveAt(i);
                    if (edges.Count != 0)
                    {
                        return MergeAndSeqauence(merged, edges, tolerance);
                    }
                }
                else
                {
                    return merged;
                }
            }
            return merged;
        }

        public static T[] GetEdgePoints<T>(T[,] array, TopoFaceEdges edge)
        {
            int n = GetEdgePointsLength(array, edge);
            int nRow = array.GetLength(0);
            int nCol = array.GetLength(1);
            T[] points = new T[n];
            switch (edge)
            {
                case TopoFaceEdges.U0:

                    points = new T[n];
                    for (int i = 0; i < n; i++)
                    {
                        points[i] = array[0, i];
                    }
                    break;
                case TopoFaceEdges.U1:
                    points = new T[n];
                    for (int i = 0; i < n; i++)
                    {
                        points[i] = array[nRow - 1, i];
                    }
                    break;
                case TopoFaceEdges.V0:
                    points = new T[n];
                    for (int i = 0; i < n; i++)
                    {
                        points[i] = array[i, 0];
                    }
                    break;
                case TopoFaceEdges.V1:
                    points = new T[n];
                    for (int i = 0; i < n; i++)
                    {
                        points[i] = array[i, nCol - 1];
                    }
                    break;
                default:
                    break;
            }
            return points;
        }

        public static int GetEdgePointsLength<T>(T[,] array, TopoFaceEdges edge)
        {
            int n;
            switch (edge)
            {
                case TopoFaceEdges.U0:
                    n = array.GetLength(1);
                    break;
                case TopoFaceEdges.U1:
                    n = array.GetLength(1);
                    break;
                case TopoFaceEdges.V0:
                    n = array.GetLength(0);
                    break;
                case TopoFaceEdges.V1:
                    n = array.GetLength(0);
                    break;
                default:
                    n = array.GetLength(1);
                    break;
            }

            return n;
        }

        public enum TopoFaceEdges
        {
            U0,
            U1,
            V0,
            V1
        }

        public class PrivateMethods
        {
            public static double GetRadius(double[] p, PTL.Definitions.Axis axis)
            {
                double r;
                switch (axis)
                {
                    case Definitions.Axis.X:
                        r = Sqrt(p[1] * p[1] + p[2] * p[2]);
                        break;
                    case Definitions.Axis.Y:
                        r = Sqrt(p[0] * p[0] + p[2] * p[2]);
                        break;
                    case Definitions.Axis.Z:
                        r = Sqrt(p[0] * p[0] + p[1] * p[1]);
                        break;
                    default:
                        r = Sqrt(p[0] * p[0] + p[1] * p[1]);
                        break;
                }
                return r;
            }

            public static double GetAxisPosition(double[] p, PTL.Definitions.Axis axis)
            {
                return p[(int)axis];
            }

            public static XYZ4 SetPosition(double[] p, double r, double axisPosition, PTL.Definitions.Axis axis)
            {
                double ri = GetRadius(p, axis);
                double ratio = r / ri;
                double[] newP;
                switch (axis)
                {
                    case Definitions.Axis.X:
                        newP = new double[] { axisPosition, p[1] * ratio, p[2] * ratio };
                        break;
                    case Definitions.Axis.Y:
                        newP = new double[] { p[0] * ratio, axisPosition, p[2] * ratio };
                        break;
                    case Definitions.Axis.Z:
                        newP = new double[] { p[0] * ratio, p[1] * ratio, axisPosition };
                        break;
                    default:
                        newP = new double[] { p[0] * ratio, p[1] * ratio, axisPosition };
                        break;
                }
                return newP;
            }

            public static CheckOrientationResult CheckMergeable(List<XYZ4> arr1, List<XYZ4> arr2, double Tolerance = 1e-5)
            {
                XYZ4 p11 = arr1.First();
                XYZ4 p12 = arr1.Last();
                XYZ4 p21 = arr2.First();
                XYZ4 p22 = arr2.Last();
                double[,] dist = new double[,]
                {
                    { Norm(p11 - p21), Norm(p11 - p22) },
                    { Norm(p12 - p21), Norm(p12 - p22) },
                };
                int index1 = 0;
                int index2 = 0;
                double min = dist[0, 0];
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (dist[i, j] < min)
                        {
                            min = dist[i, j];
                            index1 = i;
                            index2 = j;
                        }
                    }
                }

                return new CheckOrientationResult() {
                    IsMergeable = min < Tolerance,
                    IsArr1NeedToBeReversed = index1 == 0,
                    IsArr2NeedToBeReversed = index2 == 1 };
            }

            public class CheckOrientationResult
            {
                public bool IsMergeable { get; set; }
                public bool IsArr1NeedToBeReversed { get; set; }
                public bool IsArr2NeedToBeReversed { get; set; }
            }
        }
    }
}
