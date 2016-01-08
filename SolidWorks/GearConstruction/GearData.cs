using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.Definitions;
using PTL.Geometry;
using Newtonsoft.Json;

namespace PTL.SolidWorks.GearConstruction
{
    public class GearData
    {
        [JsonIgnore]
        protected int teethNumber;
        [JsonIgnore]
        protected Axis axis;
        [JsonIgnore]
        protected List<List<double[]>> blankLines;
        [JsonIgnore]
        protected List<double[,][]> toothFaces;

        public int TeethNumber
        {
            get { return teethNumber; }
            set
            {
                if (value > 0)
                    teethNumber = value;
            }
        }
        public Axis Axis
        {
            get { return axis; }
            set { axis = value; }
        }
        public List<List<double[]>> BlankLines
        {
            get { return blankLines; }
            set { blankLines = value; }
        }
        public List<double[,][]> ToothFaces
        {
            get { return toothFaces; }
            set { toothFaces = value; }
        }
    }
}
