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
using PTL.Windows.Media.Media3D;

namespace PTL.Geometry.WPFExtensions
{
    public static class WPFExtensions
    {
        public static XYZ3 GetAnyNormal(XYZ3 direction)
        {
            XYZ3 n1 = Mathematics.BasicFunctions.Cross(new XYZ3(0.0, 0.0, 1.0), direction);
            if (Mathematics.BasicFunctions.Norm(n1) < 1e-5)
                n1 = Mathematics.BasicFunctions.Cross(new XYZ3(0.0, 1.0, 0.0), direction);
            n1 = Mathematics.BasicFunctions.Normalize(n1);
            return n1;
        }

        public static TopoFace GenerateHemisphere(
            XYZ4 center,
            XYZ3 direction,
            double radius,
            int segment1,
            int segment2,
            XYZ3 segment2StartDirection,
            bool counterClock)
        {
            XYZ3 v = Mathematics.BasicFunctions.Normalize(direction);
            XYZ3 n1 = Mathematics.BasicFunctions.Normalize(segment2StartDirection);
            XYZ3 n2 = counterClock ? Mathematics.BasicFunctions.Cross(n1, direction) : Mathematics.BasicFunctions.Cross(direction, n1);
            n2 = Mathematics.BasicFunctions.Normalize(n2);

            TopoFace toprface = new TopoFace();
            toprface.Points = new XYZ4[segment1, segment2];
            toprface.Normals = new XYZ3[segment1, segment2];
            for (int i = 0; i < segment1; i++)
            {
                double theta1 = i * 0.5 * Mathematics.BasicFunctions.PI / (segment1 - 1);
                double vr = radius * Mathematics.BasicFunctions.Cos(theta1);
                double h = radius * Mathematics.BasicFunctions.Sin(theta1);
                for (int j = 0; j < segment2; j++)
                {
                    double theta2 = j * 2.0 * Mathematics.BasicFunctions.PI / (segment2 - 1);
                    XYZ3 rr = vr * n1 * Mathematics.BasicFunctions.Cos(theta2) + vr * n2 * Mathematics.BasicFunctions.Sin(theta2) + h * v;
                    XYZ4 p = center + rr;
                    toprface.Points[i, j] = p;
                    toprface.Normals[i, j] = rr;
                }
            }

            return toprface;
        }

        public static XYZ4[] GetLineRenderPoint(
            XYZ4 position,
            XYZ3 lineDirect,
            double linewidth,
            XYZ3 look,
            XYZ3 up,
            double cemeraRange,
            int totalWidthPixel
            )
        {
            XYZ3 pn = Mathematics.BasicFunctions.Cross(look, lineDirect);

            //If lineDirect parallel to camera.LookDirection
            if (Mathematics.BasicFunctions.Norm(pn) < 1e-5)
                return null;

            double trueWidth = linewidth * (cemeraRange / totalWidthPixel);

            XYZ4 V = trueWidth / 2.0 * (XYZ4)Mathematics.BasicFunctions.Normalize(pn);
            XYZ4 p1, p2;
            if (Mathematics.BasicFunctions.Dot(pn, up) > 0)
            {
                p1 = position + V;
                p2 = position - V;
            }
            else
            {
                p2 = position + V;
                p1 = position - V;
            }
            return new XYZ4[] { p1, p2 };
        }

        //public static XYZ4[] GetLineRenderCircle(
        //    XYZ4 position,
        //    XYZ3 lineDirect,
        //    double linewidth,
        //    double cemeraRange,
        //    int totalWidthPixel
        //    )
        //{
        //    XYZ3 pn = PTLM.Cross(look, lineDirect);

        //    //If lineDirect parallel to camera.LookDirection
        //    if (PTLM.Norm(pn) < 1e-5)
        //        return null;

        //    double trueWidth = linewidth * (cemeraRange / totalWidthPixel);

        //    XYZ4 V = trueWidth / 2.0 * (XYZ4)PTLM.Normalize(pn);
        //    XYZ4 p1, p2;
        //    if (PTLM.Dot(pn, up) > 0)
        //    {
        //        p1 = position + V;
        //        p2 = position - V;
        //    }
        //    else
        //    {
        //        p2 = position + V;
        //        p1 = position - V;
        //    }
        //    return new XYZ4[] { p1, p2 };
        //}

        public static FakeLineGeometryModel3D ToLineGeometryModel3D(this Line line)
        {
            PolyLine pline = new PolyLine()
            {
                Points = new List<XYZ4> { line.p1, line.p2 },
                Color = line.Color,
                CoordinateSystem = line.CoordinateSystem
            };
            return pline.ToLineGeometryModel3D();
        }

        public static FakeLineGeometryModel3D ToLineGeometryModel3D(this PolyLine pline)
        {
            FakeLineGeometryModel3D lineGeometryModel3D = new FakeLineGeometryModel3D();

            Material material = new DiffuseMaterial(
                new SolidColorBrush(Color.FromArgb(
                    pline.Color.A,
                    pline.Color.R,
                    pline.Color.G,
                    pline.Color.B)));
            lineGeometryModel3D.Bone = pline;
            lineGeometryModel3D.Bone.AbsorbCoordinateSystem();
            lineGeometryModel3D.Model = new GeometryModel3D();
            lineGeometryModel3D.Model.Material = material;

            Action<XYZ3, XYZ3, double, int> RefreshMesh = (look, up, range, pixelNum) =>
            #region
            {
                int pointNum = pline.Points.Count;
                List<XYZ4[]> points = new List<XYZ4[]>();

                Vector3D normal = new Vector3D(-look.X, -look.Y, -look.Z);
                
                //Start
                #region Start
                {
                    points.Add(GetLineRenderPoint(pline.Points[0], Mathematics.BasicFunctions.Normalize(pline.Points[1] - pline.Points[0]), pline.LineWidth, look, up, range, pixelNum));
                }
                #endregion Start
                //Mid
                #region Mid
                {
                    for (int i = 1; i < pointNum - 1; i++)
                    {
                        points.Add(GetLineRenderPoint(pline.Points[i], Mathematics.BasicFunctions.Normalize(pline.Points[i] - pline.Points[i - 1]), pline.LineWidth, look, up, range, pixelNum));
                        points.Add(GetLineRenderPoint(pline.Points[i], Mathematics.BasicFunctions.Normalize(pline.Points[i + 1] - pline.Points[i]), pline.LineWidth, look, up, range, pixelNum));
                    }
                }
                #endregion Mid
                //End
                #region End
                {
                    points.Add(GetLineRenderPoint(pline.Points[pointNum - 1], Mathematics.BasicFunctions.Normalize(pline.Points[pointNum - 1] - pline.Points[pointNum - 2]), pline.LineWidth, look, up, range, pixelNum));
                }
                #endregion End

                MeshGeometry3D mesh;
                int N1 = points.Count;
                int N1_1 = N1 - 1;
                if (lineGeometryModel3D.Model.Geometry != null)
                {
                    mesh = (MeshGeometry3D)lineGeometryModel3D.Model.Geometry;
                    lineGeometryModel3D.Model.Geometry = null;

                    mesh.Positions.Clear();
                    mesh.Normals.Clear();

                    for (int i = 0; i < N1; i++)
                    {
                        mesh.Positions.Add(new Point3D(points[i][0].X, points[i][0].Y, points[i][0].Z));
                        mesh.Positions.Add(new Point3D(points[i][1].X, points[i][1].Y, points[i][1].Z));
                        mesh.Normals.Add(normal);
                        mesh.Normals.Add(normal);
                    }
                    lineGeometryModel3D.Model.Geometry = mesh;
                }
                else
                {
                    mesh = new MeshGeometry3D();
                    for (int i = 0; i < N1; i++)
                    {
                        mesh.Positions.Add(new Point3D(points[i][0].X, points[i][0].Y, points[i][0].Z));
                        mesh.Positions.Add(new Point3D(points[i][1].X, points[i][1].Y, points[i][1].Z));
                        mesh.Normals.Add(normal);
                        mesh.Normals.Add(normal);
                    }
                    for (int i = 0; i < N1_1; i = i + 2)
                    {
                        int s1 = i * 2;
                        int s2 = s1 + 1;
                        int s3 = s1 + 2;
                        int s4 = s1 + 3;
                        mesh.TriangleIndices.Add(s1);
                        mesh.TriangleIndices.Add(s2);
                        mesh.TriangleIndices.Add(s3);

                        mesh.TriangleIndices.Add(s1);
                        mesh.TriangleIndices.Add(s3);
                        mesh.TriangleIndices.Add(s2);

                        mesh.TriangleIndices.Add(s3);
                        mesh.TriangleIndices.Add(s2);
                        mesh.TriangleIndices.Add(s4);

                        mesh.TriangleIndices.Add(s3);
                        mesh.TriangleIndices.Add(s4);
                        mesh.TriangleIndices.Add(s2);
                    }
                    lineGeometryModel3D.Model.Geometry = mesh;
                }
            };
            #endregion

            lineGeometryModel3D.ReshreshModelMesh += new FakeLineGeometryModel3D.ReshreshModelMeshFunction(RefreshMesh);

            return lineGeometryModel3D;
        }

        //public static Model3D ToWPFGeometryModel3D(this Line line)
        //{
        //    XYZ4 p1 = line.p1;
        //    XYZ4 p2 = line.p2;
        //    XYZ3 V = p2 - p1;
        //    XYZ3 n = GetAnyNormal(V);
        //    double r = line.LineWidth / 2.0;
        //    int N1 = 5;
        //    int N2 = 9;
        //    TopoFace sFace = GenerateHemisphere(p1, -1 * V, r, N1, N2, n, true);
        //    TopoFace eFace = GenerateHemisphere(p2, V, r, N1, N2, n, false);
        //    TopoFace allFace = new TopoFace()
        //    {
        //        Points = new XYZ4[N1 * 2, N2],
        //        Normals = new XYZ3[N1 * 2, N2],
        //        Color = line.Color,
        //        CoordinateSystem = line.CoordinateSystem
        //    };

        //    for (int i = 0; i < N1; i++)
        //    {
        //        for (int j = 0; j < N2; j++)
        //        {
        //            allFace.Points[i, j] = sFace.Points[N1 - 1 - i, j];
        //            allFace.Normals[i, j] = sFace.Normals[N1 - 1 - i, j];

        //            allFace.Points[i + N1, j] = eFace.Points[i, j];
        //            allFace.Normals[i + N1, j] = eFace.Normals[i, j];
        //        }
        //    }

        //    return allFace.ToWPFGeometryModel3D();
        //}

        //public static Model3D ToWPFGeometryModel3D(this PolyLine pline)
        //{
        //    int pointNum = pline.Points.Count;
        //    int segmentNum = pointNum - 1;
        //    int N1 = 5;
        //    int N2 = 9;
        //    TopoFace allFace = new TopoFace()
        //    {
        //        Points = new XYZ4[N1 * 2 * segmentNum, N2],
        //        Normals = new XYZ3[N1 * 2 * segmentNum, N2],
        //        Color = pline.Color,
        //        CoordinateSystem = pline.CoordinateSystem
        //    };

        //    double r = pline.LineWidth / 2.0;
        //    for (int k = 0; k < segmentNum - 1; k++)
        //    {
        //        XYZ4 p1 = pline.Points[k];
        //        XYZ4 p2 = pline.Points[k + 1];
        //        XYZ3 V = p2 - p1;
        //        XYZ3 n = GetAnyNormal(V);

        //        TopoFace sFace = GenerateHemisphere(p1, -1 * V, r, N1, N2, n, true);
        //        TopoFace eFace = GenerateHemisphere(p2, V, r, N1, N2, n, false);

        //        int start = k * 2 * N1;
        //        for (int i = 0; i < N1; i++)
        //        {
        //            for (int j = 0; j < N2; j++)
        //            {
        //                int i_index1 = start + i;
        //                int i_index2 = start + N1 + i;
        //                allFace.Points[i_index1, j] = sFace.Points[N1 - 1 - i, j];
        //                allFace.Normals[i_index1, j] = sFace.Normals[N1 - 1 - i, j];

        //                allFace.Points[i_index2 + N1, j] = eFace.Points[i, j];
        //                allFace.Normals[i_index2 + N1, j] = eFace.Normals[i, j];
        //            }
        //        }
        //    }

        //    return allFace.ToWPFGeometryModel3D();
        //}

        public static Model3D ToModel3D(this STL stl)
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

        public static Model3D ToModel3D(this TopoFace face)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            //判斷方向
            bool direct = false;
            XYZ3 v1112 = face.Points[0, 1] - face.Points[0, 0];
            XYZ3 v1221 = face.Points[0, 1] - face.Points[0, 1];
            XYZ3 n1 = face.Normals[0, 0];
            if (Mathematics.BasicFunctions.Dot(Mathematics.BasicFunctions.Cross(v1112, v1221), n1) > 0)
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

        
    }
}
