using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Definitions
{
    public class DXFLineTypeConverter
    {
        public static netDxf.Tables.LineType Convert(LineType aLineType)
        {
            netDxf.Tables.LineType DxfLineType;
            switch (aLineType)
            {
                case LineType.Solid:
                    DxfLineType = netDxf.Tables.LineType.Continuous;
                    break;
                case LineType.Dashed:
                    DxfLineType = netDxf.Tables.LineType.Dashed;
                    break;
                case LineType.DotDashed:
                    DxfLineType = netDxf.Tables.LineType.Center;
                    break;
                case LineType.Null:
                    DxfLineType = netDxf.Tables.LineType.ByLayer;
                    break;
                default:
                    DxfLineType = netDxf.Tables.LineType.ByLayer;
                    break;
            }
            return DxfLineType;
        }
    }
}
