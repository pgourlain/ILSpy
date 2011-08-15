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
                var loadAssembly = n.LoadedAssembly;
                if (loadAssembly.AssemblyDefinition != null)
                {
                    var ret = loadAssembly.AssemblyDefinition.FullName;
                    ret += System.Environment.NewLine + loadAssembly.FileName;
                    return ret;
                }
            }
            return null;
        }

        #endregion
    }
}
