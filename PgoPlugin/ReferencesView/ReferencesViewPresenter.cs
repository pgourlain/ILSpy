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
        internal void UpdateReferences(MemberReference memberReference)
        {
            _targetAssembly = memberReference.Module.Assembly;
            _memberReference = memberReference;
            RunAsync(() => FindReferences());
        }


        IEnumerable<ReferenceItem> FindReferences()
        {
            List<ReferenceItem> l = new List<ReferenceItem>();
            var assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies();
            DoProcessTypes(_targetAssembly.Modules.SelectMany(x => x.Types), l);
            foreach (var item in assemblies)
            {
                if (CheckAssemblyReferences(item.AssemblyDefinition))
                {
                    DoProcessTypes(item.AssemblyDefinition.Modules.SelectMany(x => x.Types), l);
                }
            }
            return l;
        }

        private void DoProcessTypes(IEnumerable<TypeDefinition> types, List<ReferenceItem> l)
        {
            //check Base Type
            //check interface
            //check attributes on class, methods
            //check parameters
            //check body method
            foreach (var type in types)
            {
                //base Type
                if (IsLink(type.BaseType))
                {
                    //report.Report(TypeLink.CreateLinkViaBaseType(type));
                    l.Add(new ReferenceItem("coucou"));
                }
                //attributes
                if (type.HasCustomAttributes && type.CustomAttributes.Any(IsLink))
                {
                    l.Add(new ReferenceItem("coucou"));
                    //report.Report(string.Format("{0} via attribute", type.FullName));
                }
                //
                if (type.HasGenericParameters && type.GenericParameters.Any(IsLink))
                {
                    l.Add(new ReferenceItem("coucou"));
                }

                if (type.HasInterfaces)
                {
                    if (type.Interfaces.Any(IsLink))
                    {
                        l.Add(new ReferenceItem("coucou"));
                    }
                }

                //if (type.HasProperties) DoProcessProperties(type, type.Properties, report);

                //if (type.HasEvents) DoProcessEvents(type, type.Events, report);

                //if (type.HasFields) DoProcessFields(type, type.Fields, report);
                if (type.HasMethods) DoProcessMethods(type, type.Methods, l);

                //if (type.HasNestedTypes)
                //{
                //    DoProcessTypes(type.NestedTypes, report);
                //}
            }
        }

        private bool CheckAssemblyReferences(AssemblyDefinition assDefinition)
        {
            if (assDefinition.Modules.Where(x => x.HasAssemblyReferences)
                            .SelectMany(x => x.AssemblyReferences)
                            .Where(IsLink).Any() ||
                assDefinition.Modules.Where(x => x.HasModuleReferences)
                            .SelectMany(x => x.ModuleReferences)
                            .Where(IsLink).Any())
            {
                return true;
            }
            return false;
        }

        private void DoProcessMethods(TypeDefinition type, IEnumerable<MethodDefinition> methods, List<ReferenceItem> l)
        {
            foreach (var method in methods)
            {
                DoProcessMethod(method, l);
            }
        }

        private bool DoProcessMethod(MethodDefinition method , List<ReferenceItem> l)
        {
            if (method != null)
            {
                if (method.HasCustomAttributes)
                {
                    foreach (var item in method.CustomAttributes.Where(IsLink))
                    {
                        //report.Report(TypeLink.CreateLinkViaCustomAttribute(method, item));
                    }
                }
                if (method.HasGenericParameters)
                {
                    //Report(method.DeclaringType, method.GenericParameters.Where(IsLink), report);
                }
                if (method.HasParameters) DoProcessParameters(method, method.Parameters, l);
                if (method.HasBody) DoProcessBodyMethod(method, method.Body, l);
            }
            return false;
        }

        private void DoProcessBodyMethod(MethodDefinition method, MethodBody body, List<ReferenceItem> l)
        {
            if (body.HasVariables)
            {
                foreach (var item in body.Variables.Where(x => IsLink(x.VariableType)))
                {
                    //report.Report(string.Format("{0}.{1} via variable in method", method.DeclaringType.FullName, method.Name));
                    //report.Report(TypeLink.CreateLinkViaTypeReference(method.DeclaringType, item.VariableType));
                }
            }
            if (body.HasExceptionHandlers)
            {
                foreach (var item in body.ExceptionHandlers.Where(x => IsLink(x.CatchType)))
                {
                    //report.Report(string.Format("{0}.{1} via exception in method", method.DeclaringType.FullName, method.Name));
                    //report.Report(TypeLink.CreateLinkViaTypeReference(method.DeclaringType, item.CatchType));
                }
            }

            foreach (var instruction in body.Instructions)
            {
                switch (instruction.OpCode.Code)
                {
                    case Code.Call:
                    case Code.Calli:
                    case Code.Callvirt:
                        MethodReference mr = instruction.Operand as MethodReference;
                        if (mr != null && IsLink(mr))
                        {
                            l.Add(new ReferenceItem("tagada"));
                            //report.Report(TypeLink.CreateLinkViaMethodCall(method, mr));
                        }
                        break;
                }
            }
        }

        private void DoProcessParameters(MethodDefinition method, IEnumerable<ParameterDefinition> parameters, List<ReferenceItem> l)
        {
            foreach (var parameter in parameters)
            {
                if (IsLink(parameter.ParameterType))
                {
                    //report.Report(TypeLink.CreateLinkViaTypeReference(method.DeclaringType, parameter.ParameterType));
                }
                if (parameter.HasCustomAttributes)
                {
                    foreach (var item in parameter.CustomAttributes.Where(IsLink))
                    {
                        //report.Report(string.Format("{0}.{1} via attribute on parameter method", method.DeclaringType.FullName, method.Name));
                        //report.Report(TypeLink.CreateLinkViaTypeReference(method.DeclaringType, item.AttributeType));
                    }
                }
            }
        }

        #region Link
        private bool IsLink(AssemblyNameReference current)
        {
            return current.Name == _targetAssembly.Name.Name;
        }

        private bool IsLink(AssemblyNameDefinition current)
        {
            if (current.Name == _targetAssembly.Name.Name && current.PublicKeyToken == _targetAssembly.Name.PublicKeyToken)
            {
                return true;
            }
            return false;
        }
        private bool IsLink(ModuleReference current)
        {
            return current.Name == _targetAssembly.Name.Name;
        }

        private bool IsLink(IMetadataScope current)
        {
            return current.Name == _targetAssembly.Name.Name;
        }

        private bool IsLink(TypeReference current)
        {
            return current != null && current.MetadataToken == _memberReference.MetadataToken;
        }
        private bool IsLink(CustomAttribute current)
        {
            return IsLink(current.AttributeType);
        }

        private bool IsLink(MethodReference current)
        {
            return (current != null && current.MetadataToken == _memberReference.MetadataToken);
        }
        #endregion
    }
}
