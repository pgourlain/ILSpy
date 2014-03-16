using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Debugger.Bookmarks;
using System.Xml.XPath;
using ICSharpCode.Decompiler.ILAst;
using Mono.Cecil;

namespace PgoPlugin.Debugger
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "SaveBreakpoints.png", Header = "Save Breakpoints", MenuOrder = 7.1, MenuCategory = "Others")]
    class SaveBreakPointsCommand : SimpleCommand
    {
        internal static string BreakpointsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ILSpy", "pgoplugin.breakpoints.xml");
        public override void Execute(object parameter)
        {
            XDocument doc = new XDocument();
            var breakpoints = new XElement("Breakpoints");
            doc.Add(breakpoints);
            foreach (BreakpointBookmark bmk in BookmarkManager.Bookmarks.OfType<BreakpointBookmark>())
            {
                breakpoints.Add(new XElement("breakpoint", new XAttribute("line", bmk.LineNumber),
                    new XAttribute("functiontoken", bmk.FunctionToken),
                    new XAttribute("memberReference", ToString(bmk.MemberReference)),
                    new XAttribute("ILRange", ToString(bmk.ILRange))
                    ));
                
            }
            if (!File.Exists(BreakpointsFileName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(BreakpointsFileName));
            }
            doc.Save(BreakpointsFileName);
        }

        private string ToString(ICSharpCode.Decompiler.ILAst.ILRange iLRange)
        {
            return string.Format("{0}-{1}", iLRange.From, iLRange.To);
        }

        private string ToString(Mono.Cecil.MemberReference memberReference)
        {
            return memberReference.DeclaringType.Module.Assembly.Name.Name + "/" +
                memberReference.DeclaringType.FullName + "/" + memberReference.FullName;
        }

        public override bool CanExecute(object parameter)
        {
            return BookmarkManager.Bookmarks.OfType<BreakpointBookmark>().Count() > 0;
        }
    }

    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "LoadBreakpoints.png", Header = "Load Breakpoints", MenuOrder = 7.1, MenuCategory = "Others")]
    class LoadBreakPointsCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            if (File.Exists(SaveBreakPointsCommand.BreakpointsFileName))
            {
                XDocument doc = XDocument.Load(SaveBreakPointsCommand.BreakpointsFileName);
                foreach (var item in doc.XPathSelectElements("//breakpoint"))
                {
                    ICSharpCode.NRefactory.TextLocation location = new ICSharpCode.NRefactory.TextLocation(int.Parse(item.Attribute("line").Value), 0);
                    int functionToken = int.Parse(item.Attribute("functiontoken").Value);
                    ILRange ilRange = new ILRange();
                    string[] FromTo = item.Attribute("ILRange").Value.Split('-');
                    ilRange.From = int.Parse(FromTo[0]);
                    ilRange.To = int.Parse(FromTo[1]);
                    MemberReference member = FindMemberReference(item.Attribute("memberReference").Value);
                    var bmk = new BreakpointBookmark(member, location, functionToken, ilRange, BreakpointAction.Break);
                    BookmarkManager.AddMark(bmk);
                }

            }
        }

        private MemberReference FindMemberReference(string toFind)
        {
            var pathToMember = toFind.Split('/');
            if (pathToMember.Length < 2) return null;
            var assemblyName = pathToMember.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(assemblyName)) return null;
            
            var ass = MainWindow.Instance.CurrentAssemblyList.GetAssemblies().FirstOrDefault(x => x.AssemblyDefinition.Name.Name == assemblyName);
            if (ass != null)
            {
                ass.WaitUntilLoaded();
                foreach (var module in ass.AssemblyDefinition.Modules)
	            {
                    var type = module.Types.FirstOrDefault(x => x.FullName == pathToMember[1]);
                    if (type != null)
                    {
                        var method = type.Methods.FirstOrDefault(x => x.FullName == pathToMember[2]);
                        if (method != null)
                            return method;
                    }
	            }
            }

            //MainWindow.Instance.CurrentAssemblyList.GetAssemblies().FirstOrDefault().
            return null;
        }

        public override bool CanExecute(object parameter)
        {
            return File.Exists(SaveBreakPointsCommand.BreakpointsFileName);
        }
    }
}
