using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.ILSpy;

namespace PgoPlugin.LinqApi
{
    /* PgoLinqApiPaneBase est utilisé dans le Xaml*/

    public class PgoLinqApiPaneBase : BaseUserControl<PgoLinqApiPanePresenter>
    {
        public PgoLinqApiPaneBase ()
	    {

	    }
    }

    /// <summary>
    /// Interaction logic for PgoLinqApiPane.xaml
    /// </summary>
    public partial class PgoLinqApiPane : PgoLinqApiPaneBase
    {
        public PgoLinqApiPane()
        {
            InitializeComponent();
        }

        protected override string WindowTile
        {
            get
            {
                return "Linq Apis";
            }
        }

        void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && lvExtentions.HasItems)
            {
                e.Handled = true;
                lvExtentions.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                lvExtentions.SelectedIndex = 0;
            }
        }

    }
}
