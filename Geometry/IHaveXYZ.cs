using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry
{
    public interface IHaveXYZ
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }
}
