using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Definitions
{
    public enum LineType : ushort
    {
        Solid = 0xFFFF,//實線
        Dashed = 0xF0F0,//虛線 1111000011110000
        DotDashed = 0xE4E4,//中心線 1110010011100100
        Null = 0x0000
    }
}
