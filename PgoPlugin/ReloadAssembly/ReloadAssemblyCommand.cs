using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;

namespace PgoPlugin.ReloadAssembly
{
    [ExportContextMenuEntry(Header = "_Reload", Icon = "refresh.png")]
    public class ReloadAssemblyCommand : IContextMenuEntry
    {
        class MyComparer : IComparer<LoadedAssembly>
        {
            List<string> _currentSort;
            public MyComparer(string[] currentSort)
            {
                _currentSort = new List<string>(currentSort);
            }

            #region IComparer<LoadedAssembly> Members

            public int Compare(LoadedAssembly x, LoadedAssembly y)
            {
                var ix = this._currentSort.IndexOf(x.FileName);
                var iy = this._currentSort.IndexOf(y.FileName);
                return ix.CompareTo(iy);
            }

            #endregion
        }


        #region IContextMenuEntry Members

        public bool IsVisible(TextViewContext context)
        {
            if (context!= null && context.SelectedTreeNodes != null)
            {
                return context.SelectedTreeNodes.All(x => x is AssemblyTreeNode);
            }
            return false;
        }

        public bool IsEnabled(TextViewContext context)
        {
            return true;
        }

        public void Execute(TextViewContext context)
        {
            var mainWindow = MainWindow.Instance;
            var currentSort = mainWindow.CurrentAssemblyList.GetAssemblies().Select(x => x.FileName).ToArray();

            var assemblies = context.SelectedTreeNodes.OfType<AssemblyTreeNode>().Select(x => x.LoadedAssembly);
            foreach (var ass in assemblies)
            {
                var fileName = ass.FileName;
                mainWindow.CurrentAssemblyList.Unload(ass);
                mainWindow.CurrentAssemblyList.OpenAssembly(fileName);
            }
            mainWindow.CurrentAssemblyList.Sort(new MyComparer(currentSort));

            //it doesn't works because selectedNodes.First() has no parent
            //var root = selectedNodes.First().Parent;
            //if (root != null)
            //{
            //    var qry1 = from x in root.Children.OfType<AssemblyTreeNode>()
            //               join y in assemblies on x.LoadedAssembly.FileName equals y.FileName
            //               select x;
            //    foreach (var item in qry1)
            //    {
            //        item.IsSelected = true;
            //    }
            //}

            //TODO: re-select previous
            //mainWindow.ass
        }

        #endregion
    }
}
