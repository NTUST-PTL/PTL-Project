using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry.MathModel
{
    public interface IParametricSurface
    {
        XYZ4 P(double u, double v);
        XYZ3 dU(double u, double v);
        XYZ3 dV(double u, double v);
        XYZ3 dU2(double u, double v);
        XYZ3 dV2(double u, double v);
        XYZ3 dUdV(double u, double v);
        XYZ3 N(double u, double v);
        XYZ3 n(double u, double v);
    }
}
