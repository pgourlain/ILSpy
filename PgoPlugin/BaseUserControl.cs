using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PgoPlugin.Debugger;
using ICSharpCode.ILSpy;

namespace PgoPlugin
{

    public enum PaneLocationEnumeration { Top, Center, Bottom };
    public class BaseUserControl<TPresenter> : System.Windows.Controls.UserControl, ICSharpCode.ILSpy.IPane
        where TPresenter : BasePresenter, new()

    {
        BasePresenter _presenter;
        protected BasePresenter Presenter { get { return _presenter; } }

        public BaseUserControl()
        {
            this.Loaded += new System.Windows.RoutedEventHandler(BaseUserControl_Loaded);
        }

        public void Show()
        {
            if (!IsVisible)
            {
                switch(this.PaneLocation)
                {
                    case PaneLocationEnumeration.Top :
                        MainWindow.Instance.ShowInTopPane(WindowTile, this);
                        break;
                    case PaneLocationEnumeration.Center :
                        System.Windows.MessageBox.Show("not implemented");
                        break;
                    case PaneLocationEnumeration.Bottom :
                        MainWindow.Instance.ShowInBottomPane(WindowTile, this);
                        break;
                }
            }
        }


        void BaseUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var presenter = new TPresenter();
            _presenter = presenter;
            this.DataContext = presenter;
            presenter.ViewReady();
        }

        #region IPane Members

        void ICSharpCode.ILSpy.IPane.Closed()
        {
            this.Presenter.ViewClose();
        }

        #endregion

        protected virtual string WindowTile
        {
            get
            {
                return "(Pgo)New Pane";
            }
        }

        protected virtual PaneLocationEnumeration PaneLocation
        {
            get
            {
                return PaneLocationEnumeration.Bottom;
            }
        }
    }
}
