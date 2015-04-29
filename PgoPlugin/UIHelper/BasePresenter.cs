using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PgoPlugin.UIHelper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors")]
    public abstract class BasePresenter : ObservableObject
    {
        Dispatcher _currentDispatcher;

        public BasePresenter()
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors")]
        protected void UiInvoke(Action act)
        {
            this._currentDispatcher.BeginInvoke(act);
        }

        internal protected abstract void ViewReady();

        internal protected abstract void ViewClose();


        protected void RunAsync<T>(Func<T> source, Action<T> updateUI)
        {
            BeforeRunAsync();
            Task.Factory.StartNew<T>(source).ContinueWith(t =>
            {
                AfterRunAsync();
                updateUI(t.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected void BeforeRunAsync()
        {

        }

        protected void AfterRunAsync()
        {

        }
    }
}
