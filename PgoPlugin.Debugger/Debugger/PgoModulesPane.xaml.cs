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

namespace PgoPlugin.Debugger
{
    /// <summary>
    /// Interaction logic for PgoModulesPane.xaml
    /// </summary>
    public partial class PgoModulesPane : UserControl, IPane
    {
        PgoModulesPanePresenter presenter = new PgoModulesPanePresenter();

        public void Show()
        {
            if (!IsVisible)
            {
                MainWindow.Instance.ShowInBottomPane("Modules", this);
            }
            //Dispatcher.BeginInvoke(
            //    DispatcherPriority.Background,
            //    new Func<bool>(searchBox.Focus));
        }

        public PgoModulesPane()
        {
            InitializeComponent();
        }

        #region IPane Members

        public void Closed()
        {
            presenter.ViewClosed();
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = presenter;
            presenter.ViewReady();

        }


    }
}
