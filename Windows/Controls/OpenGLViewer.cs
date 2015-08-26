using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTL.OpenGL.Plot;

namespace PTL.Windows.Controls
{
    public class OpenGLViewer : OpenGLWindow
    {
        public OpenGLView View;

        public OpenGLViewer()
            : base()
        {
            View = new OpenGLView(this);
        }
    }
}
