using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PgoPlugin.Bookmarks
{
    public class BookmarkModel
    {
        public BookmarkModel()
        {                
        }
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
        public string FullDefinition { get; set; }

        internal static BookmarkModel FromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var result = new BookmarkModel();
            var splittedPath = path.Split('/');
            result.AssemblyName = splittedPath.First();
            result.FullDefinition = path;
            result.TypeName = string.Join("/", splittedPath.Skip(1).ToArray());
            return result;
        }
    }
}
