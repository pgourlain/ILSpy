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

        #region IContextMenuEntry Members

        public bool IsVisible(TextViewContext context)
        {
            return false;
        }

        public bool IsEnabled(TextViewContext context)
        {
            return false;
        }

        public void Execute(TextViewContext context)
        {
            foreach (var item in context.SelectedTreeNodes)
            {
                var path = Path(item);
                System.Windows.MessageBox.Show(path);
            }

        }

        #endregion
    }
}
