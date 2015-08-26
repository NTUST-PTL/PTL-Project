using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using netDxf.Entities;

namespace PTL.Geometry
{
    public interface ICanBeWritedToDXFFile
    {
        void WriteToFileInDxfFormat(StreamWriter sw);
    }
    public interface IToDXFEntity
    {
        netDxf.Entities.EntityObject ToDXFEntity();
    }
    public interface IToDXFEntities
    {
        List<netDxf.Entities.EntityObject> ToDXFEntities();
    }
    public interface IToDXFDocument
    {
        netDxf.DxfDocument ToDXFDocument();
    }
    public interface IDXF
    {
        void Save2DxfFile(String filename);
    }
    
}
