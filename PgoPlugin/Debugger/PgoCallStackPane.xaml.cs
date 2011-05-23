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
using System.Windows.Threading;

namespace PgoPlugin.Debugger
{
    /// <summary>
    /// Interaction logic for PgoCallStack.xaml
    /// </summary>
    public partial class PgoCallStackPane : UserControl, IPane
    {
        static PgoCallStackPane instance;

        public static PgoCallStackPane Instance
        {
            get
            {
                if (instance == null)
                {
                    App.Current.VerifyAccess();
                    instance = new PgoCallStackPane();
                }
                return instance;
            }
        }

        PgoCallStackPanePresenter presenter = new PgoCallStackPanePresenter();

        public void Show()
        {
            if (!IsVisible)
                MainWindow.Instance.ShowInBottomPane("Call stack", this);
            //Dispatcher.BeginInvoke(
            //    DispatcherPriority.Background,
            //    new Func<bool>(searchBox.Focus));
        }

        private PgoCallStackPane()
        {
            InitializeComponent();
        }

        #region IPane Members

        public void Closed()
        {
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = presenter;
            presenter.ViewReady();
        }

        private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            presenter.NagivateTo();
        }
    }
}
