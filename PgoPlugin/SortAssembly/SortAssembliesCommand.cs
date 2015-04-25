using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.SortAssembly
{

    // Menu: menu into which the item is added
    // MenuIcon: optional, icon to use for the menu item. Must be embedded as "Resource" (WPF-style resource) in the same assembly as the command type.
    // Header: text on the menu item
    // MenuCategory: optional, used for grouping related menu items together. A separator is added between different groups.
    // MenuOrder: controls the order in which the items appear (items are sorted by this value)
    [ExportMainMenuCommand(Menu = "_View", MenuIcon = "sort.png", Header = "Sort assemblies (A-Z)", MenuCategory = "Open", MenuOrder = 1.5)]
    // ToolTip: the tool tip
    // ToolbarIcon: The icon. Must be embedded as "Resource" (WPF-style resource) in the same assembly as the command type.
    // ToolbarCategory: optional, used for grouping related toolbar items together. A separator is added between different groups.
    // ToolbarOrder: controls the order in which the items appear (items are sorted by this value)
    [ExportToolbarCommand(ToolTip = "Sort assemblies by name", ToolbarIcon = "sort.png", ToolbarCategory = "Open", ToolbarOrder = 1.5)]
    class SortAssembliesCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            MainWindow.Instance.CurrentAssemblyList.Sort(new AssemblyComparer(true));
        }
    }
    // Menu: menu into which the item is added
    // MenuIcon: optional, icon to use for the menu item. Must be embedded as "Resource" (WPF-style resource) in the same assembly as the command type.
    // Header: text on the menu item
    // MenuCategory: optional, used for grouping related menu items together. A separator is added between different groups.
    // MenuOrder: controls the order in which the items appear (items are sorted by this value)
    [ExportMainMenuCommand(Menu = "_View", MenuIcon = "sortd.png", Header = "Sort assemblies (Z-A)", MenuCategory = "Open", MenuOrder = 1.5)]
    // ToolTip: the tool tip
    // ToolbarIcon: The icon. Must be embedded as "Resource" (WPF-style resource) in the same assembly as the command type.
    // ToolbarCategory: optional, used for grouping related toolbar items together. A separator is added between different groups.
    // ToolbarOrder: controls the order in which the items appear (items are sorted by this value)
    [ExportToolbarCommand(ToolTip = "Sort assemblies by name", ToolbarIcon = "sortd.png", ToolbarCategory = "Open", ToolbarOrder = 1.5)]
    class SortDAssembliesCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            MainWindow.Instance.CurrentAssemblyList.Sort(new AssemblyComparer(false));
        }
    }


    class AssemblyComparer : IComparer<LoadedAssembly>
    {
        bool _ascend;
        public AssemblyComparer(bool ascend)
        {
            this._ascend = ascend;
        }

        #region IComparer<LoadedAssembly> Members

        public int Compare(LoadedAssembly x, LoadedAssembly y)
        {
            var compareResult = x.ShortName.CompareTo(y.ShortName);
            if (!_ascend)
                return -compareResult;
            return compareResult;
        }

        #endregion
    }

}
