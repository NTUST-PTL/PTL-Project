using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.Geometry
{
    public interface IToDXFEntities
    {
        List<netDxf.Entities.EntityObject> ToDXFEntities();
    }
}
