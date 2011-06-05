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

namespace PgoPlugin.Bookmarks
{
    /// <summary>
    /// Interaction logic for PgoBookmarksPane.xaml
    /// </summary>
    public partial class PgoBookmarksPane : UserControl
    {
        public PgoBookmarksPane()
        {
            InitializeComponent();
        }

        public void Show()
        {
            if (!IsVisible)
                MainWindow.Instance.ShowInTopPane("Boomarks", this);
            //Dispatcher.BeginInvoke(
            //    DispatcherPriority.Background,
            //    new Func<bool>(searchBox.Focus));
        }

    }
}
