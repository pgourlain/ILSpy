using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace PgoPlugin.Debugger
{
    public abstract class BasePresenter : ObservableObject
    {
        Dispatcher _currentDispatcher;

        public BasePresenter()
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected void UiInvoke(Action act)
        {
            this._currentDispatcher.BeginInvoke(act);
        }

        internal protected abstract void ViewReady();

        internal protected abstract void ViewClose();

    }
}
