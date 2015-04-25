using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.ResourceFinder
{
    [ExportMainMenuCommand(Menu = "_View", MenuIcon = "resources.png", Header = "Resource finder")]
    class ResourceFinderCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            SingletonPane<ResourceFinderPane>.Instance.Show();
        }

        public override bool CanExecute(object parameter)
        {                
            return true;
        }
    }
}
