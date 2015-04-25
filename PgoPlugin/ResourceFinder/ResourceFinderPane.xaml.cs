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
using PgoPlugin.UIHelper;

namespace PgoPlugin.ResourceFinder
{

    /* ResourceFinderPane est utilisé dans le Xaml*/

    public class ResourceFinderPaneBase : BaseUserControl<ResourceFinderPanePresenter>
    {
        public ResourceFinderPaneBase()
        {

        }
    }
    /// <summary>
    /// Interaction logic for ResourceFinderPane.xaml
    /// </summary>
    public partial class ResourceFinderPane : ResourceFinderPaneBase
    {
        public ResourceFinderPane()
        {
            InitializeComponent();
            this.lvExtentions.MouseDoubleClick += new MouseButtonEventHandler(lvExtentions_MouseDoubleClick);
        }

        void lvExtentions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = lvExtentions.SelectedItem as ResourceItem;
            if (selectedItem != null)
            {

                MainWindow.Instance.JumpToReference(selectedItem.Resource);
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

        protected override string WindowTile
        {
            get
            {
                return "Resource finder";
            }
        }

        protected override void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.searchBox.Focus();
            base.UserControl_Loaded(sender, e);

        }
    }
}
