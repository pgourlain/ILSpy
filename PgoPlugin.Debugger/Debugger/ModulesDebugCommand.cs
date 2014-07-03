using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.Debugger
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "callstack.png", Header = "Modules")]
    class ModulesDebugCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            SingletonPane<PgoModulesPane>.Instance.Show();
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
