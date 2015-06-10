using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            _targetAssembly = memberReference.Module.Assembly;
            _memberReference = memberReference;
            RunAsync(() => FindReferences());
        }


        IEnumerable<ReferenceItem> FindReferences()
        {
            var assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies();
            var typeDef = _memberReference as TypeDefinition;
            if (typeDef != null)
            {
                return new ReferencesEnumerator().InputReferenceOf(typeDef, assemblies.Select(x => x.AssemblyDefinition));
            }
            return new ReferenceItem[] { new ReferenceItem("not yet implemented") };
        }
    }
}
