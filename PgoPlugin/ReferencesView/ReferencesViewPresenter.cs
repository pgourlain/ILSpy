using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ICSharpCode.ILSpy;
using Mono.Cecil;
using Mono.Cecil.Cil;
using PgoPlugin.UIHelper;

namespace PgoPlugin.ReferencesView
{
    public class ReferencesViewPresenter : FilteredPresenter<ReferenceItem>
    {
        TaskCompletionSource<IEnumerable<ReferenceItem>> _currentRun;
        public ReferencesViewPresenter()
        {

        }

        protected internal override void ViewClose()
        {
            //throw new NotImplementedException();
        }

        protected internal override void ViewReady()
        {
            //throw new NotImplementedException();
        }

        protected override bool OnFiltered(object value)
        {
            return base.OnFiltered(value);
        }

        AssemblyDefinition _targetAssembly;
        MemberReference _memberReference;
        internal void UpdateReferences(string command, MemberReference memberReference)
        {
            if (memberReference != null)
                _targetAssembly = memberReference.Module.Assembly;
            _memberReference = memberReference;
            RunAsync(() => FindReferences(command));
        }


        IEnumerable<ReferenceItem> FindReferences(string command)
        {
            var assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies();
            var typeDef = _memberReference as TypeDefinition;
            var refEnumerator = new ReferencesEnumerator();
            switch (command)
            {
                case "OutputReferences":
                    return refEnumerator.OutputReferenceOf(_memberReference);
                case "OutputReferencesInSameAssembly":
                    return refEnumerator.OutputReferenceOf(_memberReference, true);
                default:
                    if (typeDef != null)
                    {
                        return refEnumerator.InputReferenceOf(typeDef, assemblies.Select(x => x.AssemblyDefinition));
                    }
                break;
            }
                
            return new ReferenceItem[] { new ReferenceItem("not yet implemented") };
        }

        XName name(string name)
        {
            return XName.Get(name, "http://schemas.microsoft.com/vs/2009/dgml");
        }

        XElement node(string nodeName)
        {
            return new XElement(name("Node"),
                new XAttribute("Id", nodeName),
                new XAttribute("Label", nodeName)
                );
        }

        XElement link(string source, string target)
        {
            return new XElement(name("Link"),
                new XAttribute("Source", source),
                new XAttribute("Target", target)
                );
        }

        internal XDocument CreateDgml()
        {
            //this.Models
            XDocument document = new XDocument();
            var n = name("DirectedGraph");
            var nodes = new XElement(name("Nodes"));
            var links = new XElement(name("Links"));
            var properties = new XElement(name("Properties"));
            document.Add(new XElement(n, nodes, links, properties));
            nodes.Add(node(_memberReference.Name));

            foreach (var item in this.Models.GroupBy(x => x.TypeFullName))
            {
                var newNode = node(item.Key);
                nodes.Add(newNode);
                links.Add(link(_memberReference.Name, item.Key));
                foreach (var grp in item)
                {
                }
                //item.
            }

            properties.Add(new XElement("Property", new XAttribute("Label", "Label"),
                new XAttribute("DataType", "String")));
            return document;
        }
    }
}
