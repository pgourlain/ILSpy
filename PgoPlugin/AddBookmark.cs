using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin
{

    [ExportContextMenuEntry(Header = "_Add Bookmark", Icon="addbookmark.png")]
	public class AddBookmark : IContextMenuEntry
    {
        #region IContextMenuEntry Members

        public bool IsVisible(ICSharpCode.TreeView.SharpTreeNode[] selectedNodes)
        {
            return true;
        }

        public bool IsEnabled(ICSharpCode.TreeView.SharpTreeNode[] selectedNodes)
        {
            return true;
        }

        public void Execute(ICSharpCode.TreeView.SharpTreeNode[] selectedNodes)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
