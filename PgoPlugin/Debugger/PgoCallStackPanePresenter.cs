using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
//using ICSharpCode.ILSpy.Debugger.Services;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using ICSharpCode.ILSpy.Debugger.Services;
using Debugger;
using Debugger.MetaData;
using PgoPlugin.UIHelper;

namespace PgoPlugin.Debugger
{
    class PgoVariablesCallStackPanePresenter : ObservableObject
    {
        Dispatcher _currentDispatcher;
        IDebugger m_currentDebugger;

        private void UiInvoke(Action act)
        {
            this._currentDispatcher.BeginInvoke(act);
        }

        public PgoVariablesCallStackPanePresenter()
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        internal void ViewReady()
        {
            DebuggerService.DebugStarted += new EventHandler(OnDebugStarted);
            DebuggerService.DebugStopped += new EventHandler(OnDebugStopped);
            if (DebuggerService.IsDebuggerStarted)
                OnDebugStarted(null, EventArgs.Empty);

            ICSharpCode.ILSpy.Debugger.Services.DebuggerService.CurrentDebugger.IsProcessRunningChanged += new EventHandler(OnProcessRunningChanged);
        }

        public VariableViewModel SelectedItem { get; set; }

        ObservableCollection<VariableViewModel> _CurrentCallStack = new ObservableCollection<VariableViewModel>();
        public ObservableCollection<VariableViewModel> CurrentCallStack
        {
            get
            {
                return _CurrentCallStack;
            }
        }

        internal void NagivateTo()
        {
            //if (this.SelectedItem != null)
            //{
            //    if (this.SelectedItem.CodeMapping != null && this.SelectedItem.CodeMapping.MemberMapping != null)
            //        ICSharpCode.ILSpy.MainWindow.Instance.JumpToReference(this.SelectedItem.CodeMapping.MemberMapping.MemberReference);
            //}
            //else
            //{
            //    MessageBox.Show("selecteditem is null");
            //}
        }

        internal void ViewClosed()
        {
            DebuggerService.DebugStarted -= new EventHandler(OnDebugStarted);
            DebuggerService.DebugStopped -= new EventHandler(OnDebugStopped);
            if (null != m_currentDebugger)
                OnDebugStopped(null, EventArgs.Empty);

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
            ClearPad();
        }

        void OnProcessRunningChanged(object sender, EventArgs args)
        {
            if (m_currentDebugger.IsProcessRunning)
                return;
            RefreshPad();
        }

        private void RefreshPad()
        {
            Process debuggedProcess = ((WindowsDebugger)m_currentDebugger).DebuggedProcess;
            if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedThread == null)
            {
                ClearPad();
                return;
            }

            StackFrame activeFrame = null;
            try
            {
                activeFrame = debuggedProcess.SelectedThread.GetCallstack(1).FirstOrDefault();
                if (activeFrame != null)
                {
                    _CurrentCallStack.Clear();
                    var parameters = activeFrame.MethodInfo.GetParameters();
                    foreach (var item in parameters.OfType<DebugParameterInfo>())
                    {
                        _CurrentCallStack.Add(VariableViewModel.FromParameter(item, activeFrame));
                    }
                    List<DebugLocalVariableInfo> vars = activeFrame.MethodInfo.GetLocalVariables();
                    foreach (var item in vars)
                    {
                        _CurrentCallStack.Add(VariableViewModel.FromVar(item, activeFrame));
                    }
                }
            }
            catch (System.Exception)
            {
                if (debuggedProcess == null || debuggedProcess.HasExited)
                {
                    // Process unexpectedly exited
                }
                else
                {
                    throw;
                }
            }
        }

        private void ClearPad()
        {
            UiInvoke(() => this._CurrentCallStack.Clear());
        }
    }
}
