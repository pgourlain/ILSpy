using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.Debugger
{

    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "callstack.png", Header = "Call stack", MenuCategory = "Open", MenuOrder = 1.5)]
    class CallStackCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            SingletonPane<PgoCallStackPane>.Instance.Show();
        }

        public override bool CanExecute(object parameter)
        {
            //return ICSharpCode.ILSpy.Debugger.Services.DebuggerService.CurrentDebugger.IsDebugging;
            return true;
        }
    }
}
