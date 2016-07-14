using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;
using static PTL.Mathematics.BasicFunctions;

namespace PTL.Geometry.MathModel
{
    public class NUB_Surface : NUB_Surface_base
    {
        public enum GivenTangentsDirections
        {
            UDirection,
            VDirection,
            None
        }
        public GivenTangentsDirections GivenTangentsDirection = GivenTangentsDirections.None;
        public bool ReverseUV { get { return GivenTangentsDirection == GivenTangentsDirections.UDirection; } }

        public NUB_Surface(XYZ4[][] dataPoints)
        {
            Calulate(dataPoints);
        }

        public NUB_Surface(XYZ4[][] dataPoints, XYZ3[][] Tangents, GivenTangentsDirections givenTangentsDirection = GivenTangentsDirections.VDirection)
        {
            XYZ4[][] dataPoints2 = givenTangentsDirection == GivenTangentsDirections.UDirection ? Transpose(dataPoints) : dataPoints;
            this.GivenTangentsDirection = givenTangentsDirection;
            Calulate(dataPoints2, Tangents);
        }

        public NUB_Surface(NUB_Curve[] UCurves)
            :base(UCurves)
        {
        }

        public new XYZ4 P(double u, double v)
        {
            return ReverseUV ? base.P(v, u) : base.P(u, v);
        }

        public new XYZ3 dV(double u, double v)
        {
            return ReverseUV ? base.dU(v, u) : base.dV(u, v);
        }

        public new XYZ3 dV2(double u, double v)
        {
            return ReverseUV ? base.dU2(v, u) : base.dV2(u, v);
        }

        public new XYZ3 dU(double u, double v)
        {
            return ReverseUV ? base.dV(v, u) : base.dU(u, v);
        }

        public new XYZ3 dU2(double u, double v)
        {
            return ReverseUV ? base.dV2(v, u) : base.dU2(u, v);
        }

        public new XYZ3 dUdV(double u, double v)
        {
            return ReverseUV ? base.dUdV(v, u) : base.dUdV(u, v);
        }

        public new XYZ3 N(double u, double v)
        {
            return ReverseUV ? base.N(v, u) : base.N(u, v);
        }

        public new XYZ3 n(double u, double v)
        {
            return ReverseUV ? base.n(v, u) : base.n(u, v);
        }
    }
}
