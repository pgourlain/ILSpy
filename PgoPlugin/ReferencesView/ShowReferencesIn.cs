using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using Mono.Cecil;

namespace PgoPlugin.ReferencesView
{

    public class ShowReferencesBase : IContextMenuEntry
    {
        public virtual void Execute(TextViewContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsEnabled(TextViewContext context)
        {
            return true;
        }

        public virtual bool IsVisible(TextViewContext context)
        {
            var result = context != null && context.SelectedTreeNodes != null && context.SelectedTreeNodes.Length == 1;
            if (result)
            {

            }
            return result;
        }
        protected MemberReference GetMemberReference(SharpTreeNode node)
        {
            var member = node as IMemberTreeNode;
            if (member != null) return member.Member;
            return null;
        }
    }

    [ExportContextMenuEntry(Header = "Show References In", Category ="References")]
    public class ShowReferencesIn : ShowReferencesBase
    {
        public override void Execute(TextViewContext context)
        {
            var memberRef = GetMemberReference(context.SelectedTreeNodes.First());
            SingletonPane<ReferencesView>.Instance.Show("InputReferences", memberRef);
        }
    }

    [ExportContextMenuEntry(Header = "Show References Out", Category = "References")]
    public class ShowReferencesOut : ShowReferencesBase
    {
        public override void Execute(TextViewContext context)
        {
            var memberRef = GetMemberReference(context.SelectedTreeNodes.First());
            SingletonPane<ReferencesView>.Instance.Show("OutputReferences", memberRef);
        }
    }

    [ExportContextMenuEntry(Header = "Show References Out in current assembly", Category = "References")]
    public class ShowReferencesOutInSameAssembly : ShowReferencesBase
    {
        public override void Execute(TextViewContext context)
        {
            var memberRef = GetMemberReference(context.SelectedTreeNodes.First());
            SingletonPane<ReferencesView>.Instance.Show("OutputReferencesInSameAssembly", memberRef);
        }
    }
}
