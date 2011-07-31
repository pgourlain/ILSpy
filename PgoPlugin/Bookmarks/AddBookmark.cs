using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;

namespace PgoPlugin.Bookmarks
{

    [ExportContextMenuEntry(Header = "_Add Bookmark", Icon="addbookmark.png")]
	public class AddBookmark : IContextMenuEntry
    {
        #region IContextMenuEntry Members

        public bool IsVisible(ICSharpCode.TreeView.SharpTreeNode[] selectedNodes)
        {
            return false;
        }

        public bool IsEnabled(ICSharpCode.TreeView.SharpTreeNode[] selectedNodes)
        {
            return false;
        }

        public void Execute(ICSharpCode.TreeView.SharpTreeNode[] selectedNodes)
        {
            foreach (var item in selectedNodes)
            {
                var path = Path(item);
                System.Windows.MessageBox.Show(path);
            }
        }

        private string Path(ICSharpCode.TreeView.SharpTreeNode item)
        {
            StringBuilder sb = new StringBuilder();

            while (!item.IsRoot)
            {
                if (sb.Length > 0)
                    sb.Insert(0, "/");
                sb.Insert(0, item.Text);
                item = item.Parent;
            }
            return sb.ToString();
        }

        #endregion
    }
}
