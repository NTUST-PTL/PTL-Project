using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Geometry.MathModel;

namespace PTL.Geometry
{
    public interface IHaveBoundary
    {
        XYZ4[] GetBoundary(double[,] externalCoordinateMatrix);
    }
}
