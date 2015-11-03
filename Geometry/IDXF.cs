using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using netDxf.Entities;

namespace PTL.Geometry
{
    public interface IDXF
    {
        void Save2DxfFile(String filename);
    }
    
}
