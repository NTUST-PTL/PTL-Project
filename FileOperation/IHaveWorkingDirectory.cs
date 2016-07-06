using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.FileOperation
{
    public interface IHaveWorkingDirectory
    {
        WorkingDirectory Directory { get; set; }
    }
}
