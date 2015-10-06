using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using PTL.Geometry;
using PTL.Definitions;
using PTL.Geometry.MathModel;

namespace PTL.Measurement
{
    public interface ISTLMeasurement_NeedNormals 
    {
        /// <summary>
        /// 量測點的法向量
        /// </summary>
        List<Vector> MeasurePointNormals { get; set; }
    }
}
