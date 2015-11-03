using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PTL.Geometry
{
    public interface ICanBeWritedToDXFFile
    {
        void WriteToFileInDxfFormat(StreamWriter sw);
    }
}
