using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.ILSpy;
using Mono.Cecil;

namespace PgoPlugin.CallStackParser
{
    [ExportMainMenuCommand(Menu = "_View", MenuIcon = "callstackparser.png", Header = "Call stack parser")]
    class CallStackParserCommand : SimpleCommand
    {
        static Window windowParser;
        public override void Execute(object parameter)
        {
            if (windowParser != null)
            {
                windowParser.Activate();
            }
            else
            {
                windowParser = new Window
                {
                    Title = "Exception callstack parser",
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Content = new CallStackParserView(),
                    WindowStyle = WindowStyle.ToolWindow,
                    MaxHeight = 768
                };

                windowParser.Owner = MainWindow.Instance;
                windowParser.Closing += WindowParser_Closing;
                windowParser.Show();
            }

        }

        private void WindowParser_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool isActive = windowParser.IsActive;

            windowParser = null;
            if (isActive)
                MainWindow.Instance.Activate();
        }
    }
}
