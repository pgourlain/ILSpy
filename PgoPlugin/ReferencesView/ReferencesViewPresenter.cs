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
        string _lastAction;
        TaskCompletionSource<IEnumerable<ReferenceItem>> _currentRun;
        public ReferencesViewPresenter()
        {
            _lastAction = string.Empty;
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
            _lastAction = command;
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

        XElement node(string nodeName, string nodeLabel = "", string description ="")
        {
            if (string.IsNullOrEmpty(nodeLabel))
                nodeLabel = nodeName;
            return new XElement(name("Node"),
                new XAttribute("Id", nodeName),
                new XAttribute("Label", nodeLabel),
                new XAttribute("Description", description ?? "")
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
            var categories = new XElement(name("Categories"));
            document.Add(new XElement(n, nodes, links, categories, properties));
            string centerNodeName = _memberReference.FullName;
            nodes.Add(node(centerNodeName));

            var outputRef = _lastAction.StartsWith("Output");


            foreach (var item in this.Models.GroupBy(x => x.TypeFullName))
            {
                var newNode = node(item.Key);
                newNode.Add(new XAttribute("Group", "Expanded"));
                nodes.Add(newNode);

                foreach (var grp in item)
                {
                    string methodName = grp.Method != null ? grp.Method.Name : grp.TypeFullName;
                    string nKey = item.Key + "_" + methodName;
                    nKey = nKey.Replace(' ', '_');
                    newNode = node(nKey, methodName, grp.MethodName);
                    nodes.Add(newNode);
                    var l = link(item.Key, nKey);
                    l.Add(new XAttribute("Category", "Contains"));
                    links.Add(l);
                    if (outputRef)
                    {
                        links.Add(link(centerNodeName, nKey));
                    }
                    else
                    {
                        links.Add(link(nKey, centerNodeName));
                    }
                }
            }

            categories.Add(new XElement("Category", new XAttribute("Id", "Contains"),
                new XAttribute("Label", "Contains"),
                new XAttribute("CanBeDataDriven", "False"),
                new XAttribute("CanLinkedNodesBeDataDriven", "True"),
                new XAttribute("IsContainment", "True")));
            properties.Add(new XElement("Property", new XAttribute("Id", "Group"),
                new XAttribute("Label", "Label"),
                new XAttribute("DataType", "String")));
            properties.Add(new XElement("Property", new XAttribute("Id", "Group"),
                new XAttribute("Label", "Group"),
                new XAttribute("DataType", "Microsoft.VisualStudio.GraphModel.GraphGroupStyle")));
            properties.Add(new XElement("Property", new XAttribute("Id", "CanBeDataDriven"),
                new XAttribute("Label", "CanBeDataDriven"),
                new XAttribute("DataType", "Boolean")));
            properties.Add(new XElement("Property", new XAttribute("Id", "CanLinkedNodesBeDataDriven"),
                new XAttribute("Label", "CanLinkedNodesBeDataDriven"),
                new XAttribute("DataType", "Boolean")));
            properties.Add(new XElement("Property", new XAttribute("Id", "IsContainment"),
                new XAttribute("DataType", "Boolean")));
            return document;
        }
    }
}
