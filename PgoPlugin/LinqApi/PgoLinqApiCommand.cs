using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.LinqApi
{
    [ExportMainMenuCommand(Menu = "_View", MenuIcon = "lambda.png", Header = "Linq Apis")]
    class PgoLinqApiCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            SingletonPane<PgoLinqApiPane>.Instance.Show();
        }

        public override bool CanExecute(object parameter)
        {                
            return true;
        }
    }
}
