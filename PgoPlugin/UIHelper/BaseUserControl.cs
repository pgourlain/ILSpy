﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.UIHelper
{

    public enum PaneLocationEnumeration { Top, Center, Bottom };
    public class BaseUserControl<TPresenter> : System.Windows.Controls.UserControl, ICSharpCode.ILSpy.IPane
        where TPresenter : BasePresenter, new()

    {
        TPresenter _presenter;
        protected TPresenter Presenter { get { return _presenter; } }

        public BaseUserControl()
        {
            var presenter = new TPresenter();
            _presenter = presenter;
            //this.DataContext = _presenter;
            this.Loaded += new System.Windows.RoutedEventHandler(UserControl_Loaded);
        }

        public void Show()
        {
            Show(null);
        }

        public void Show(params object[] parameter)
        {
            if (!IsVisible)
            {
                switch (this.PaneLocation)
                {
                    case PaneLocationEnumeration.Top:
                        MainWindow.Instance.ShowInTopPane(WindowTile, this);
                        this.SetParameters(parameter);
                        break;
                    case PaneLocationEnumeration.Center:
                        System.Windows.MessageBox.Show("not implemented");
                        break;
                    case PaneLocationEnumeration.Bottom:
                        MainWindow.Instance.ShowInBottomPane(WindowTile, this);
                        this.SetParameters(parameter);
                        break;
                }
            }
            else SetParameters(parameter);
        }

        protected virtual void SetParameters(object[] args)
        {
 
        }

        protected virtual void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DataContext = _presenter;
            _presenter.ViewReady();
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