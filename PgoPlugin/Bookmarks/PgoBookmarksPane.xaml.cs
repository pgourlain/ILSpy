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

namespace PgoPlugin.Bookmarks
{

    /* PgoLinqApiPaneBase est utilisé dans le Xaml*/

    public class PgoBookmarksPaneBase : BaseUserControl<BookmarkPresenter>
    {
        public PgoBookmarksPaneBase()
        {

        }
    }
    /// <summary>
    /// Interaction logic for PgoBookmarksPane.xaml
    /// </summary>
    public partial class PgoBookmarksPane : PgoBookmarksPaneBase
    {
        public PgoBookmarksPane()
        {
            InitializeComponent();
            this.lvExtentions.MouseDoubleClick += new MouseButtonEventHandler(lvExtentions_MouseDoubleClick);
            this.Presenter.LoadBookmarks();
            MainWindow.Instance.Closing += Instance_Closing;
        }

        void Instance_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Presenter.SaveBookmarks();
        }

        void lvExtentions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = lvExtentions.SelectedItem as BookmarkModel;
            if (selectedItem != null)
            {
                MainWindow.Instance.JumpToPath(selectedItem.FullDefinition.Split('/'));
            }
        }

        protected override string WindowTile { get { return "Bookmarks"; } }

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


        internal void AddBookmark(BookmarkModel model)
        {
            if (model != null)
            {
                this.Presenter.Models.Add(model);
            }
        }
    }
}
