using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Definitions;

namespace PTL.Geometry
{
    public interface ILineArchitecture
    {
        LineType LineType { get; set; }
        float LineWidth { get; set; }
        int LineTypefactor { get; set; }
    }
}
