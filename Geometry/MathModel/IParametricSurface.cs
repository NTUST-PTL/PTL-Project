using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry.MathModel
{
    public interface IParametricSurface
    {
        XYZ4 Position(double u, double v);
        XYZ3 U_Tangent(double u, double v);
        XYZ3 V_Tangent(double u, double v);
        XYZ3 Normal(double u, double v);
        XYZ3 UnitNormal(double u, double v);
    }
}
