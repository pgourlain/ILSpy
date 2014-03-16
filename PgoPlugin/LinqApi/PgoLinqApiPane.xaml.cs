using System.Windows;
using System.Windows.Input;
using ICSharpCode.ILSpy;
using PgoPlugin.UIHelper;

namespace PgoPlugin.LinqApi
{
    /* PgoLinqApiPaneBase est utilisé dans le Xaml*/

    public class PgoLinqApiPaneBase : BaseUserControl<PgoLinqApiPanePresenter>
    {
        public PgoLinqApiPaneBase()
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
            this.lvExtentions.MouseDoubleClick += new MouseButtonEventHandler(lvExtentions_MouseDoubleClick);
        }

        void lvExtentions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = lvExtentions.SelectedItem as LinqApiModel;
            if (selectedItem != null)
            {
                MainWindow.Instance.JumpToReference(selectedItem.Method);
            }
        }

        protected override string WindowTile { get { return "Linq Apis"; } }

        void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && lvExtentions.HasItems)
            {
                e.Handled = true;
                lvExtentions.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                lvExtentions.SelectedIndex = 0;
            }
        }

        protected override void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.searchBox.Focus();
            base.UserControl_Loaded(sender, e);
        }

    }
}
