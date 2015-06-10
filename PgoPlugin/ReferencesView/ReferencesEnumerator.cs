using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace PgoPlugin.ReferencesView
{
    class ReferencesEnumerator
    {

        public IEnumerable<ReferenceItem> OutputReferenceOf(MemberReference current)
        {
            return null;
        }
        public IEnumerable<ReferenceItem> OutputReferenceOf(MemberReference current, bool inSameAssembly)
        {
            return null;
        }

        public IEnumerable<ReferenceItem> InputReferenceOf(TypeDefinition current, IEnumerable<AssemblyDefinition> assemblies)
        {
            List<ReferenceItem> l = new List<ReferenceItem>();
            var linkResolver = new IsLinkToType(current);
            //retrait de la selection le type en cours
            var types = assemblies.SelectMany(x => x.Modules).SelectMany(x => x.Types)
                .Where(x => x.MetadataToken != current.MetadataToken);
            DoProcessTypes(types, linkResolver, l);
            return l.Distinct(new ReferenceItemComparer());
        }


        private void DoProcessTypes(IEnumerable<TypeDefinition> types, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            //check Base Type
            //check interface
            //check attributes on class, methods
            //check parameters
            //check body method
            foreach (var type in types)
            {
                //base Type
                if (linkResolver.IsLink(type.BaseType))
                {
                    l.Add(new ReferenceItem(type.Module.Assembly.FullName, type.FullName));
                }
                //attributes
                if (type.HasCustomAttributes)
                {
                    foreach (var item in type.CustomAttributes.Where(linkResolver.IsLink))
                    {
                        l.Add(new ReferenceItem(type.Module.Assembly.FullName, type.FullName));
                    }
                }
                //
                if (type.HasGenericParameters)
                {
                    foreach (var item in type.GenericParameters.Where(linkResolver.IsLink))
                    {
                        l.Add(new ReferenceItem(type.Module.Assembly.FullName, type.FullName));
                    }
                }

                if (type.HasInterfaces)
                {
                    foreach (var item in type.Interfaces.Where(linkResolver.IsLink))
                    {
                        l.Add(new ReferenceItem(type.Module.Assembly.FullName, type.FullName));
                    }
                }

                //if (type.HasProperties) DoProcessProperties(type, type.Properties, report);

                //if (type.HasEvents) DoProcessEvents(type, type.Events, report);

                //if (type.HasFields) DoProcessFields(type, type.Fields, report);
                if (type.HasMethods) DoProcessMethods(type, type.Methods, linkResolver, l);

                //if (type.HasNestedTypes)
                //{
                //    DoProcessTypes(type.NestedTypes, report);
                //}
            }

        }

        private void DoProcessMethods(TypeDefinition type, IEnumerable<MethodDefinition> methods, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            foreach (var method in methods)
            {
                DoProcessMethod(method, linkResolver, l);
            }
        }

        private bool DoProcessMethod(MethodDefinition method, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            if (method != null)
            {
                if (method.HasCustomAttributes)
                {
                    foreach (var item in method.CustomAttributes.Where(linkResolver.IsLink))
                    {
                        l.Add(new ReferenceItem(method.Module.Assembly.FullName, method.DeclaringType.FullName, method));
                    }
                }
                if (method.HasGenericParameters)
                {
                    //Report(method.DeclaringType, method.GenericParameters.Where(IsLink), report);
                }
                if (method.HasParameters) DoProcessParameters(method, method.Parameters, linkResolver, l);
                if (method.HasBody) DoProcessBodyMethod(method, method.Body, linkResolver, l);
            }
            return false;
        }
        private void DoProcessBodyMethod(MethodDefinition method, MethodBody body, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            if (body.HasVariables)
            {
                foreach (var item in body.Variables.Where(x => linkResolver.IsLink(x.VariableType)))
                {
                    //report.Report(string.Format("{0}.{1} via variable in method", method.DeclaringType.FullName, method.Name));
                    //report.Report(TypeLink.CreateLinkViaTypeReference(method.DeclaringType, item.VariableType));
                }
            }
            if (body.HasExceptionHandlers)
            {
                foreach (var item in body.ExceptionHandlers.Where(x => linkResolver.IsLink(x.CatchType)))
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
                        if (mr != null && linkResolver.IsLink(mr))
                        {
                            l.Add(new ReferenceItem(method.Module.Assembly.FullName, method.DeclaringType.FullName, method));
                        }
                        break;
                }
            }
        }

        private void DoProcessParameters(MethodDefinition method, IEnumerable<ParameterDefinition> parameters, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            foreach (var parameter in parameters)
            {
                if (linkResolver.IsLink(parameter.ParameterType))
                {
                    //report.Report(TypeLink.CreateLinkViaTypeReference(method.DeclaringType, parameter.ParameterType));
                }
                if (parameter.HasCustomAttributes)
                {
                    foreach (var item in parameter.CustomAttributes.Where(linkResolver.IsLink))
                    {
                        //report.Report(string.Format("{0}.{1} via attribute on parameter method", method.DeclaringType.FullName, method.Name));
                        //report.Report(TypeLink.CreateLinkViaTypeReference(method.DeclaringType, item.AttributeType));
                    }
                }
            }
        }

    }
}
