using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using System.ComponentModel.Composition;
using ICSharpCode.ILSpy.TreeNodes;

namespace PgoPlugin.AssemblyTooltip
{
    [Export(typeof(ITooltipMenuEntry))]
    class AssemblyTooltipProvider : ITooltipMenuEntry
    {
        #region ITooltipMenuEntry Members

        public string GetTooltip(ICSharpCode.TreeView.SharpTreeNode selectedNode)
        {
            var n = selectedNode as AssemblyTreeNode;
            if (n != null && n.LoadedAssembly != null)
            {
                return FormatString(n.LoadedAssembly, "");
            }
            var n1 = selectedNode as TypeTreeNode;
            if (n1 != null)
            {
                return FormatString(n1.ParentAssemblyNode.LoadedAssembly, n1.TypeDefinition.FullName + Environment.NewLine);
            }
            return null;
        }

        #endregion

        private static string FormatString(LoadedAssembly ass, string header)
        {
            var loadAssembly = ass;
            if (loadAssembly.AssemblyDefinition != null)
            {
                var ret = loadAssembly.AssemblyDefinition.FullName;
                ret += System.Environment.NewLine + loadAssembly.FileName;
                return header + ret;
            }
            return null;
        }
    }
}
