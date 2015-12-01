using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PTL.FileOperation
{
    public class TXTFile
    {
        public static void SaveToTXTFile(String filname, String content, FileMode filmode)
        {
            FileStream fs = new FileStream(filname, filmode);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(content);
            sw.Flush();
            sw.Close();
        }
    }
}
