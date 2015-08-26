using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PTL.Geometry
{
    public interface ICanBeWritedToScriptFile
    {
        void WriteToFileInScriptFormat(StreamWriter sw);
    }
}
