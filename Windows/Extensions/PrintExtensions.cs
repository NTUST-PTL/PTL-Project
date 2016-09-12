using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.IO.Packaging;

namespace PTL.Windows.Extensions
{
    public static class PrintExtensions
    {
        public static void ConvertToPDF(this Visual visual, string filename)
        {
            //Convert visual to XPS
            MemoryStream lMemoryStream = new MemoryStream();
            Package package = Package.Open(lMemoryStream, FileMode.Create);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            writer.Write(visual);
            doc.Close();
            package.Close();

            //Convert XPS to PDF
            var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(lMemoryStream);

            PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, filename, 0);
        }
    }
}
