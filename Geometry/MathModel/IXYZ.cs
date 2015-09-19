using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry.MathModel
{
    public interface IXYZ : ICloneable
    {
        double[] Values { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        double this[int index] { get; set; }
        bool IsHomogeneous { get; }

        object New();
    }
}
