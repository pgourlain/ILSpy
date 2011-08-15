using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.TreeView;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace ICSharpCode.ILSpy
{

    public interface ITooltipMenuEntry
    {
        string GetTooltip(SharpTreeNode selectedNode);
    }

    class TooltipEntryProvider
    {
        public static void Add(SharpTreeView treeView)
        {
            var provider = new TooltipEntryProvider(treeView);
            treeView.TooltipItemOpening += provider.treeView_TooltipItemOpening;
        }

        private TooltipEntryProvider(SharpTreeView treeView)
        {
            this.treeView = treeView;
            App.CompositionContainer.ComposeParts(this);
        }

        readonly SharpTreeView treeView;

        [ImportMany(typeof(ITooltipMenuEntry))]
        Lazy<ITooltipMenuEntry>[] entries = null;

        void treeView_TooltipItemOpening(object sender, TooltipItemOpeningRoutedEventArgs e)
        {
            e.ToolTip = entries.Select(x => x.Value)
                                .Select(x => x.GetTooltip(e.Node))
                                .Aggregate((string)null, (acc,x) => {
                                    if (string.IsNullOrEmpty(acc))
                                        return x;
                                    else
                                        return acc + System.Environment.NewLine + x;
                                    }); 
        }
    }
}
