using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;
using static System.Math;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry
{
    public class Operations
    {
        public static TopoFace SealToRadius(XYZ4[] points, double r, int interpolate = 0, Action<TopoFace> TopoFaceSetter = null)
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
                double ri = GetFirstNNorm(interpolatedPoints[i], 2);
                double ratio = r / ri;
                XYZ4 p = interpolatedPoints[i];
                sealSurface.Points[i, 0] = p;
                sealSurface.Points[i, 1] = new XYZ4(p.X * ratio, p.Y * ratio, p.Z);
            }

            sealSurface.SolveNormalVector();
            TopoFaceSetter?.Invoke(sealSurface);
            return sealSurface;
        }

        public static TopoFace SealToRadius(TopoFace face, double r, TopoFaceEdges edge, int interpolate = 0, Action<TopoFace> TopoFaceSetter = null)
        {
            return SealToRadius(GetEdgePoints(face.Points, edge), r, interpolate, TopoFaceSetter);
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

        static T[] GetEdgePoints<T>(T[,] array, TopoFaceEdges edge)
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

        static int GetEdgePointsLength<T>(T[,] array, TopoFaceEdges edge)
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
    }
}
