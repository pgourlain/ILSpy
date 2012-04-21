using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ICSharpCode.ILSpy.Debugger.Services;

namespace PgoPlugin.Debugger
{
    class PgoModulesPanePresenter : ObservableObject
    {
        Dispatcher _currentDispatcher;
        IDebugger m_currentDebugger;

        private void UiInvoke(Action act)
        {
            this._currentDispatcher.BeginInvoke(act);
        }

        public PgoModulesPanePresenter()
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        internal void ViewClosed()
        {
            DebuggerService.DebugStarted -= new EventHandler(OnDebugStarted);
            DebuggerService.DebugStopped -= new EventHandler(OnDebugStopped);
            if (null != m_currentDebugger)
                OnDebugStopped(null, EventArgs.Empty);

        }

        internal void ViewReady()
        {
            DebuggerService.DebugStarted += new EventHandler(OnDebugStarted);
            DebuggerService.DebugStopped += new EventHandler(OnDebugStopped);
            if (DebuggerService.IsDebuggerStarted)
                OnDebugStarted(null, EventArgs.Empty);

            ICSharpCode.ILSpy.Debugger.Services.DebuggerService.CurrentDebugger.IsProcessRunningChanged += new EventHandler(OnProcessRunningChanged);

        }

        void OnDebugStarted(object sender, EventArgs args)
        {
            m_currentDebugger = DebuggerService.CurrentDebugger;
            m_currentDebugger.IsProcessRunningChanged += new EventHandler(OnProcessRunningChanged);

            OnProcessRunningChanged(null, EventArgs.Empty);
        }

        void OnDebugStopped(object sender, EventArgs args)
        {
            m_currentDebugger.IsProcessRunningChanged -= new EventHandler(OnProcessRunningChanged);
            m_currentDebugger = null;
            //ClearPad();
        }

        void OnProcessRunningChanged(object sender, EventArgs args)
        {
            if (m_currentDebugger.IsProcessRunning)
                return;
            //RefreshPad();
        }
    }
}
