using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using PTL.Mathematics;
using PTL.Geometry;
using PTL.Geometry.MathModel;

namespace PTL.Geometry.WPFExtensions
{
    public static class WPFExtensions
    {
        public static XYZ3 GetAnyNormal(XYZ3 direction)
        {
            XYZ3 n1 = PTLM.Cross(new XYZ3(0, 0, 1), direction);
            if (PTLM.Norm(n1) < 1e-5)
                n1 = PTLM.Cross(new XYZ3(0, 1, 0), direction);
            n1 = PTLM.Normalize(n1);
            return n1;
        }

        public static TopoFace GenerateHemisphere(XYZ4 center, XYZ3 direction, double radius, int segment1, int segment2, XYZ3 segment2StartDirection, bool counterClock)
        {
            XYZ3 v = PTLM.Normalize(direction);
            XYZ3 n1 = PTLM.Normalize(segment2StartDirection);
            XYZ3 n2 = counterClock? PTLM.Cross(n1, direction): PTLM.Cross(direction, n1);
            n2 = PTLM.Normalize(n2);

            TopoFace toprface = new TopoFace();
            toprface.Points = new XYZ4[segment1, segment2];
            toprface.Normals = new XYZ3[segment1, segment2];
            for (int i = 0; i < segment1; i++)
            {
                double theta1 = i * 0.5 * PTLM.PI / (segment1 - 1);
                double vr = radius * PTLM.Cos(theta1);
                double h = radius * PTLM.Sin(theta1);
                for (int j = 0; j < segment2; j++)
                {
                    double theta2 = j * 2.0 * PTLM.PI / (segment2 - 1);
                    XYZ3 rr = vr * n1 * PTLM.Cos(theta2) + vr * n2 * PTLM.Sin(theta2) + h * v;
                    XYZ4 p = center + rr;
                    toprface.Points[i, j] = p;
                    toprface.Normals[i, j] = rr;
                }
            }

            return toprface;
        }

        public static GeometryModel3D ToWPFGeometryModel3D(this STL stl)
        {
            int total = stl.Entities.Count;
            int added = 0;

            
            MeshGeometry3D mesh = new MeshGeometry3D();
            foreach (Triangle triangle in stl.Entities.Values)
            {
                mesh.Positions.Add(new Point3D(triangle.P1.X, triangle.P1.Y, triangle.P1.Z));
                mesh.Positions.Add(new Point3D(triangle.P2.X, triangle.P2.Y, triangle.P2.Z));
                mesh.Positions.Add(new Point3D(triangle.P3.X, triangle.P3.Y, triangle.P3.Z));
                if (triangle.N1 != null)
                {
                    XYZ3 n1 = triangle.N1;
                    mesh.Normals.Add(new Vector3D(n1.X, n1.Y, n1.Z));
                    XYZ3 n2 = triangle.N2 ?? n1;
                    mesh.Normals.Add(new Vector3D(n2.X, n2.Y, n2.Z));
                    XYZ3 n3 = triangle.N2 ?? n1;
                    mesh.Normals.Add(new Vector3D(n3.X, n3.Y, n3.Z));
                }
                mesh.TriangleIndices.Add(added * 3);
                mesh.TriangleIndices.Add(added * 3 + 1);
                mesh.TriangleIndices.Add(added * 3 + 2);
                added++;
            }

            Material material = new DiffuseMaterial(
                new SolidColorBrush(Color.FromArgb(
                    stl.Color.A,
                    stl.Color.R,
                    stl.Color.G,
                    stl.Color.B)));
            GeometryModel3D geomatry = new GeometryModel3D(mesh, material);
            geomatry.BackMaterial = material;

            if (stl.CoordinateSystem != null)
            {
                double[,] M = stl.CoordinateSystem;
                geomatry.Transform = new Transform3DGroup();
                MatrixTransform3D transform =
                new MatrixTransform3D(
                    new Matrix3D(
                    M[0, 0], M[1, 0], M[2, 0], M[2, 0],
                    M[0, 1], M[1, 1], M[2, 1], M[2, 1],
                    M[0, 2], M[1, 2], M[2, 2], M[2, 2],
                    M[0, 3], M[1, 3], M[2, 3], M[2, 3])
                    );
            }
            

            return geomatry;
        }

        public static GeometryModel3D ToWPFGeometryModel3D(this Line line)
        {
            XYZ4 p1 = line.p1;
            XYZ4 p2 = line.p2;
            XYZ3 V = p2 - p1;
            XYZ3 n = GetAnyNormal(V);
            double r = line.LineWidth / 2.0;
            int N1 = 5;
            int N2 = 9;
            TopoFace sFace = GenerateHemisphere(p1, -1 * V, r, N1, N2, n, true);
            TopoFace eFace = GenerateHemisphere(p2, V, r, N1, N2, n, false);
            TopoFace allFace = new TopoFace() {
                Points = new XYZ4[N1 * 2, N2],
                Normals = new XYZ3[N1 * 2, N2],
                Color = line.Color,
                CoordinateSystem = line.CoordinateSystem };

            for (int i = 0; i < N1; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    allFace.Points[i, j] = sFace.Points[N1 - 1 - i, j];
                    allFace.Normals[i, j] = sFace.Normals[N1 - 1 - i, j];

                    allFace.Points[i + N1, j] = eFace.Points[i, j];
                    allFace.Normals[i + N1, j] = eFace.Normals[i, j];
                }
            }

            return allFace.ToWPFGeometryModel3D();

            //XYZ4 p1 = line.p1;
            //XYZ4 p2 = line.p2;
            //XYZ3 V = p2 - p1;
            //XYZ3 t = PTLM.Normalize(V);
            //XYZ3 n1 = PTLM.Cross(new XYZ3(0, 0, 1), t);
            //if (PTLM.Norm(n1) < 1e-5)
            //    n1 = PTLM.Cross(new XYZ3(0, 1, 0), t);
            //n1 = PTLM.Normalize(n1);
            //XYZ3 n2 = PTLM.Cross(t, n1);
            //n2 = PTLM.Normalize(n2);
            //double r = line.LineWidth / 2.0;

            //int N = 9;
            //MeshGeometry3D mesh = new MeshGeometry3D();
            //for (int i = 0; i < N; i++)
            //{
            //    double theta1 = i * 0.5 * PTLM.PI / (N - 1);
            //    double vr = r * PTLM.Cos(theta1);
            //    double h = r * PTLM.Sin(theta1);
            //    for (int j = 0; j < N; j++)
            //    {
            //        double theta2 = j * 2.0 * PTLM.PI / (N - 1);
            //        XYZ3 rr = vr * n1 * PTLM.Cos(theta2) + vr * n2 * PTLM.Sin(theta2);
            //        XYZ3 nn1 = rr + h * (-1) * t;
            //        XYZ3 nn2 = rr + h * t;
            //        XYZ4 pp1 = p1 + nn1;
            //        XYZ4 pp2 = p2 + nn2;
            //        mesh.Positions.Add(new Point3D(pp1[0], pp1[1], pp1[2]));
            //        mesh.Positions.Add(new Point3D(pp2[0], pp2[1], pp2[2]));
            //        mesh.Normals.Add(new Vector3D(nn1[0], nn1[1], nn1[2]));
            //        mesh.Normals.Add(new Vector3D(nn2[0], nn2[1], nn2[2]));
            //    }
            //}
            ////側面
            //for (int i = 0; i < N - 1; i++)
            //{
            //    int s1 = i * 2;
            //    int s2 = s1 + 1;
            //    int s3 = s1 + 2;
            //    int s4 = s1 + 3;
            //    mesh.TriangleIndices.Add(s1);
            //    mesh.TriangleIndices.Add(s3);
            //    mesh.TriangleIndices.Add(s2);

            //    mesh.TriangleIndices.Add(s3);
            //    mesh.TriangleIndices.Add(s4);
            //    mesh.TriangleIndices.Add(s2);
            //}
            ////頭尾
            //for (int i = 0; i < N - 1; i++)
            //{
            //    for (int j = 0; j < N - 1; j++)
            //    {
            //        int s1 = i * (2 * N) + j * 2;
            //        int s2 = s1 + 2;
            //        int s3 = s1 + 2 * N;
            //        int s4 = s3 + 2;
            //        mesh.TriangleIndices.Add(s1);
            //        mesh.TriangleIndices.Add(s3);
            //        mesh.TriangleIndices.Add(s2);

            //        mesh.TriangleIndices.Add(s4);
            //        mesh.TriangleIndices.Add(s2);
            //        mesh.TriangleIndices.Add(s3);

            //        int e1 = s1 + 1;
            //        int e2 = s2 + 1;
            //        int e3 = s3 + 1;
            //        int e4 = s4 + 1;
            //        mesh.TriangleIndices.Add(e3);
            //        mesh.TriangleIndices.Add(e1);
            //        mesh.TriangleIndices.Add(e2);

            //        mesh.TriangleIndices.Add(e4);
            //        mesh.TriangleIndices.Add(e3);
            //        mesh.TriangleIndices.Add(e2);
            //    }
            //}

            //Material material = new DiffuseMaterial(
            //    new SolidColorBrush(Color.FromArgb(
            //        line.Color.A,
            //        line.Color.R,
            //        line.Color.G,
            //        line.Color.B)));
            //GeometryModel3D geomatry = new GeometryModel3D(mesh, material);
            ////geomatry.BackMaterial = material;

            //if (line.CoordinateSystem != null)
            //{
            //    double[,] M = line.CoordinateSystem;
            //    geomatry.Transform = new Transform3DGroup();
            //    MatrixTransform3D transform =
            //    new MatrixTransform3D(
            //        new Matrix3D(
            //        M[0, 0], M[1, 0], M[2, 0], M[2, 0],
            //        M[0, 1], M[1, 1], M[2, 1], M[2, 1],
            //        M[0, 2], M[1, 2], M[2, 2], M[2, 2],
            //        M[0, 3], M[1, 3], M[2, 3], M[2, 3])
            //        );
            //}


            //return geomatry;
        }

        public static GeometryModel3D ToWPFGeometryModel3D(this TopoFace face)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            //判斷方向
            bool direct = false;
            XYZ3 v1112 = face.Points[0, 1] - face.Points[0, 0];
            XYZ3 v1221 = face.Points[0, 1] - face.Points[0, 1];
            XYZ3 n1 = face.Normals[0, 0];
            if (PTLM.Dot(PTLM.Cross(v1112, v1221), n1) > 0)
                direct = true;

            //加入點資料
            int N1 = face.Dim1Length;
            int N2 = face.Dim2Length;
            for (int i = 0; i < N1; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    mesh.Positions.Add(new Point3D(
                        face.Points[i, j][0],
                        face.Points[i, j][1],
                        face.Points[i, j][2]
                        ));
                    mesh.Normals.Add(new Vector3D(
                        face.Normals[i, j][0],
                        face.Normals[i, j][1],
                        face.Normals[i, j][2]
                        ));
                }
            }

            //建立網格
            for (int i = 0; i < N1 - 1; i++)
            {
                for (int j = 0; j < N2 - 1; j++)
                {
                    int s11 = i * N2 + j;
                    int s12 = s11 + 1;
                    int s21 = s11 + N2;
                    int s22 = s21 + 1;
                    if (direct)
                    {
                        mesh.TriangleIndices.Add(s11);
                        mesh.TriangleIndices.Add(s21);
                        mesh.TriangleIndices.Add(s12);

                        mesh.TriangleIndices.Add(s21);
                        mesh.TriangleIndices.Add(s22);
                        mesh.TriangleIndices.Add(s12);
                    }
                    else
                    {
                        mesh.TriangleIndices.Add(s11);
                        mesh.TriangleIndices.Add(s12);
                        mesh.TriangleIndices.Add(s21);

                        mesh.TriangleIndices.Add(s21);
                        mesh.TriangleIndices.Add(s12);
                        mesh.TriangleIndices.Add(s22);
                    }
                }
            }

            Material material = new DiffuseMaterial(
                new SolidColorBrush(Color.FromArgb(
                    face.Color.A,
                    face.Color.R,
                    face.Color.G,
                    face.Color.B)));
            GeometryModel3D geomatry = new GeometryModel3D(mesh, material);
            //geomatry.BackMaterial = material;

            if (face.CoordinateSystem != null)
            {
                double[,] M = face.CoordinateSystem;
                geomatry.Transform = new Transform3DGroup();
                MatrixTransform3D transform =
                new MatrixTransform3D(
                    new Matrix3D(
                    M[0, 0], M[1, 0], M[2, 0], M[2, 0],
                    M[0, 1], M[1, 1], M[2, 1], M[2, 1],
                    M[0, 2], M[1, 2], M[2, 2], M[2, 2],
                    M[0, 3], M[1, 3], M[2, 3], M[2, 3])
                    );
            }


            return geomatry;
        }

        public static Model3DGroup ToWPFGeometryModel3D(this PolyLine pline)
        {
            Model3DGroup geomatry = new Model3DGroup();
            for (int i = 0; i < pline.Points.Count - 1; i++)
            {
                Line line = new Line() {
                    p1 = pline.Points[i],
                    p2 = pline.Points[i + 1],
                    Color = pline.Color,
                    LineWidth = pline.LineWidth,
                    CoordinateSystem = pline.CoordinateSystem
                };
                geomatry.Children.Add(line.ToWPFGeometryModel3D());
            }
            return geomatry;
        }
    }
}
