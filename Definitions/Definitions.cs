using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Definitions
{
    /// <summary>
    /// X Y Z 軸的枚舉
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum Dimension
    {
        OneDimention,
        TwoDimension,
        ThreeDimension
    }

    public enum LineType : ushort
    {
        Solid = 0xFFFF,//實線
        Dashed = 0xF0F0,//虛線 1111000011110000
        DotDashed = 0xE4E4,//中心線 1110010011100100
        Null = 0x0000
    }

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
