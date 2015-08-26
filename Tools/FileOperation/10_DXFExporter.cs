using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using PTL.Geometry;

namespace PTL.Tools.FileOperation
{
    public interface IDXFExporter
    {
        void ExportDXF(Layer DXF2Export);
        void ExportDXF(Layer DXF2Export, string filename);
        void ExportScript(Part DXF2Export, bool openFolder);
        void ExportScript(Part DXF2Export, string filename, bool openFolder);
    }

    public class DXFExporter : PTL.Math
    {
        /// <summary>
        /// System.Drawing.Color對應DXF索引顏色的Dictionary
        /// </summary>
        public Dictionary<Color, int> SystemColor_DXFColor;
        public SaveFileDialog saveFileDialog1;
        Process ComputerAPPs;

        public DXFExporter()
        {
            saveFileDialog1 = new SaveFileDialog();
            ComputerAPPs = new Process();
            ComputerAPPs.StartInfo.UseShellExecute = false;
            ComputerAPPs.StartInfo.FileName = "Explore.exe";

            //建立系System.Drawing.Color對應DXF索引顏色的Dictionary
            SystemColor_DXFColor = new Dictionary<Color, int>();
            SystemColor_DXFColor.Add(Color.White, 7);
            SystemColor_DXFColor.Add(Color.Red, 1);
            SystemColor_DXFColor.Add(Color.Orange, 30);
            SystemColor_DXFColor.Add(Color.LawnGreen, 3);
            SystemColor_DXFColor.Add(Color.SkyBlue, 130);
            SystemColor_DXFColor.Add(Color.DeepSkyBlue, 140);
            SystemColor_DXFColor.Add(Color.Blue, 5);
        }

        public virtual void ExportDXF(Part DXF2Export, bool openFolder, bool openFile)
        {
            saveFileDialog1.Filter = "DXF Files(*.DXF)|*.DXF";
            saveFileDialog1.FileName = "GearProfile.DXF";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExportDXF(DXF2Export, saveFileDialog1.FileName, openFolder, openFile);
            }
            string path = System.IO.Path.GetDirectoryName(saveFileDialog1.FileName);
            System.Diagnostics.Process.Start(path);
        }

        public virtual void ExportDXF(Part DXF2Export, string filename, bool openFolder, bool openFile)
        {
            DXF2Export.Save2DxfFile(filename);

            if (openFolder)
            {
                string path = System.IO.Path.GetDirectoryName(filename);
                System.Diagnostics.Process.Start(path);
            }
            if (openFile)
                System.Diagnostics.Process.Start(filename);
        }

        public virtual void ExportDXF_netDxf(Part DXF2Export, string filename, bool openFolder, bool openFile)
        {
            netDxf.DxfDocument dxf = DXF2Export.ToDXFDocument();
            dxf.Save(filename);
            if (openFolder)
            {
                string path = System.IO.Path.GetDirectoryName(filename);
                System.Diagnostics.Process.Start(path);
            }
            if (openFile)
                System.Diagnostics.Process.Start(filename);
        }

        public virtual void ExportScript(Part DXF2Export, bool openFolder, bool openFile)
        {
            saveFileDialog1.Filter = "Script Files(*.scr)|*.scr";
            saveFileDialog1.FileName = "GearProfile.scr";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExportScript(DXF2Export, saveFileDialog1.FileName, openFolder, openFile);
            }
        }

        public virtual void ExportScript(Part DXF2Export, string filename, bool openFolder, bool openFile)
        {
            DXF2Export.Save2DxfFile(filename);

            if (openFolder)
            {
                string path = System.IO.Path.GetDirectoryName(filename);
                System.Diagnostics.Process.Start(path);
            }
            if(openFile)
                System.Diagnostics.Process.Start(filename);
        }
    }
}
