using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;

namespace PgoPlugin.Analyser
{

    [ExportContextMenuEntryAttribute(Header = "Derived types", Icon = null)]
    public sealed class DerivedTypesAnalyzeMenuEntry : IContextMenuEntry
    {
        public void Execute(TextViewContext context)
        {
            SingletonPane<PgoDerivedTypesAnalyzerPane>.Instance.Show();
        }

        public bool IsEnabled(TextViewContext context)
        {
            return true;
        }

        public bool IsVisible(TextViewContext context)
        {
            if (context.SelectedTreeNodes == null)
            {
                return context.Reference != null && context.Reference.Reference is TypeReference;
            }
            return false;
            //if (context.TreeView is AnalyzerTreeView && context.SelectedTreeNodes != null && context.SelectedTreeNodes.All(n => n.Parent.IsRoot))
            //    return false;
            //if (context.SelectedTreeNodes == null)
            //    return context.Reference != null && context.Reference.Reference is MemberReference;
            //return context.SelectedTreeNodes.All(n => n is IMemberTreeNode);
        }
    }
}
