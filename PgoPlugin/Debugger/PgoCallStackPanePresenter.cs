using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ICSharpCode.ILSpy.Debugger.Services;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;

namespace PgoPlugin.Debugger
{
    class PgoCallStackPanePresenter : ObservableObject
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        Dispatcher _currentDispatcher;

        private void UiInvoke(Action act)
        {
            this._currentDispatcher.BeginInvoke(act);
        }

        public PgoCallStackPanePresenter()
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        internal void ViewReady()
        {
            ICSharpCode.ILSpy.Debugger.Services.DebuggerService.CurrentDebugger.IsProcessRunningChanged += new EventHandler(CurrentDebugger_IsProcessRunningChanged);
        }

        void CurrentDebugger_IsProcessRunningChanged(object sender, EventArgs e)
        {
            WindowsDebugger wd = DebuggerService.CurrentDebugger as WindowsDebugger;

            System.Diagnostics.Trace.WriteLine("Pgo : Begin CurrentDebugger_IsProcessRunningChanged");

            if (wd != null && wd.DebuggedProcess.IsPaused)
            {
                var callstack = wd.DebuggedProcess.SelectedThread.Callstack;
                var viewModels = callstack.Select(x => StackViewModel.FromStackFrame(x)).ToArray();
                UiInvoke(() =>
                {
                    _CurrentCallStack.Clear();
                    foreach (var item in viewModels)
                    {
                        _CurrentCallStack.Add(item);
                    }
                }); 
            }
            else
            {
                UiInvoke(() =>
                    {
                        _CurrentCallStack.Clear();
                    });
            }
            System.Diagnostics.Trace.WriteLine("Pgo : End CurrentDebugger_IsProcessRunningChanged");
        }


        public StackViewModel SelectedItem { get; set; }

        ObservableCollection<StackViewModel> _CurrentCallStack = new ObservableCollection<StackViewModel>();
        public ObservableCollection<StackViewModel> CurrentCallStack
        {
            get
            {
                return _CurrentCallStack;
            }
        }

        internal void NagivateTo()
        {
            if (this.SelectedItem != null)
            {
                if (this.SelectedItem.CodeMapping != null && this.SelectedItem.CodeMapping.MemberMapping != null)
                    ICSharpCode.ILSpy.MainWindow.Instance.JumpToReference(this.SelectedItem.CodeMapping.MemberMapping.MemberReference);
            }
            else
            {
                MessageBox.Show("selecteditem is null");
            }
        }
    }
}
