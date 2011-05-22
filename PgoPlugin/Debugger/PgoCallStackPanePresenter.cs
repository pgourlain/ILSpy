using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ICSharpCode.ILSpy.Debugger.Services;

namespace PgoPlugin.Debugger
{
    class PgoCallStackPanePresenter : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        internal void ViewReady()
        {
            ICSharpCode.ILSpy.Debugger.Services.DebuggerService.CurrentDebugger.IsProcessRunningChanged += new EventHandler(CurrentDebugger_IsProcessRunningChanged);
        }

        void CurrentDebugger_IsProcessRunningChanged(object sender, EventArgs e)
        {
            WindowsDebugger wd = DebuggerService.CurrentDebugger as WindowsDebugger;

            System.Diagnostics.Trace.WriteLine("Pgo : Begin CurrentDebugger_IsProcessRunningChanged");

            //if (wd != null)
            //{
            //    var callstack = wd.DebuggedProcess.SelectedThread.Callstack;
            //    if (callstack != null)
            //    {
            //        foreach (var stack in callstack)
            //        {
            //            stack.MethodInfo.ToString();
            //            System.Diagnostics.Trace.WriteLine("Pgo : stack : " + stack.MethodInfo.ToString());
            //        }
            //    }
            //}
            System.Diagnostics.Trace.WriteLine("Pgo : End CurrentDebugger_IsProcessRunningChanged");
        }
    }
}
