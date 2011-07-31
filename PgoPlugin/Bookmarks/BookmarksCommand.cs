﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.Bookmarks
{

    // Menu: menu into which the item is added
    // MenuIcon: optional, icon to use for the menu item. Must be embedded as "Resource" (WPF-style resource) in the same assembly as the command type.
    // Header: text on the menu item
    // MenuCategory: optional, used for grouping related menu items together. A separator is added between different groups.
    // MenuOrder: controls the order in which the items appear (items are sorted by this value)
    [ExportMainMenuCommand(Menu = "_View", MenuIcon = "bookmarks.png", Header = "Display boomarks", MenuCategory = "Open", MenuOrder = 1.5)]
    // ToolTip: the tool tip
    // ToolbarIcon: The icon. Must be embedded as "Resource" (WPF-style resource) in the same assembly as the command type.
    // ToolbarCategory: optional, used for grouping related toolbar items together. A separator is added between different groups.
    // ToolbarOrder: controls the order in which the items appear (items are sorted by this value)
    [ExportToolbarCommand(ToolTip = "Display boomarks", ToolbarIcon = "bookmarks.png", ToolbarCategory = "Open", ToolbarOrder = 1.5)]
    public class BookmarksCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            SingletonPane<PgoBookmarksPane>.Instance.Show();
        }

        public override bool CanExecute(object parameter)
        {
            return false;
        }
    }
}
