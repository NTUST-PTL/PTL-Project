using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PTL.Geometry;
using PTL.Data;

namespace PTL.Windows.Controls
{
    /// <summary>
    /// OpenGLHost.xaml 的互動邏輯
    /// </summary>
    public partial class OpenGLHost : UserControl
    {
        public ExObservableCollection<ICanPlotInOpenGL> EntityCollection
        {
            get { return (ExObservableCollection<ICanPlotInOpenGL>)GetValue(EntityCollectionProperty); }
            set { SetValue(EntityCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EntityCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EntityCollectionProperty =
            DependencyProperty.Register("EntityCollection", typeof(ExObservableCollection<ICanPlotInOpenGL>), typeof(OpenGLHost), new PropertyMetadata(null, RunEntityCollectionChanged));

        public event EventHandler EntityCollectionChanged;
        static void RunEntityCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as OpenGLHost).OpenGLViewer.View.Things2Show = e.NewValue as ExObservableCollection<ICanPlotInOpenGL>;
            (d as OpenGLHost)?.EntityCollectionChanged?.Invoke(d, null);
        }

        public OpenGLHost()
        {
            InitializeComponent();
        }
    }
}
