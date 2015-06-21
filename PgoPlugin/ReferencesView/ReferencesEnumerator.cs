using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace PgoPlugin.ReferencesView
{
    class ReferencesEnumerator
    {

        public IEnumerable<ReferenceItem> OutputReferenceOf(MemberReference current)
        {
            return OutputReferenceOf(current, false);
        }
        public IEnumerable<ReferenceItem> OutputReferenceOf(MemberReference current, bool inSameAssembly)
        {
            List<ReferenceItem> l = new List<ReferenceItem>();
            IIsLinkResolver linkResolver = null;
            if (current is TypeDefinition)
            {
                linkResolver = new IsLinkOutOfthisType((TypeDefinition)current, inSameAssembly);
                //var types = current.Module.Types.Where(x => x.MetadataToken != current.MetadataToken);
                DoProcessTypes(new TypeDefinition[] { (TypeDefinition)current }, linkResolver, l);
            }
            else if (current is MethodDefinition)
            {
                linkResolver = new IsLinkOutOfthisType(((MethodDefinition)current).DeclaringType, inSameAssembly);
                DoProcessMethod((MethodDefinition)current, linkResolver, l);
            }
            else
            {
                l.Add(new ReferenceItem("not yet implement for this type"));
            }
            return l.Distinct(new ReferenceItemComparer());
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
                    l.Add(linkResolver.CreateLink(type, type.BaseType));
                }
                //attributes
                if (type.HasCustomAttributes)
                {
                    foreach (var item in type.CustomAttributes.Where(linkResolver.IsLink))
                    {
                        l.Add(linkResolver.CreateLink(type, item));
                    }
                }
                //
                if (type.HasGenericParameters)
                {
                    foreach (var item in type.GenericParameters.Where(linkResolver.IsLink))
                    {
                        l.Add(linkResolver.CreateLink(type, item));
                    }
                }

                if (type.HasInterfaces)
                {
                    foreach (var item in type.Interfaces.Where(linkResolver.IsLink))
                    {
                        l.Add(linkResolver.CreateLink(type, item));
                    }
                }

                if (type.HasProperties) DoProcessProperties(type, type.Properties, linkResolver, l);

                if (type.HasEvents) DoProcessEvents(type, type.Events, linkResolver, l);

                if (type.HasFields) DoProcessFields(type, type.Fields, linkResolver, l);
                if (type.HasMethods) DoProcessMethods(type, type.Methods, linkResolver, l);
            }

        }

        private void DoProcessEvents(TypeDefinition type, IEnumerable<EventDefinition> events, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            foreach (var ev in events)
            {
                DoProcessEvent(ev, linkResolver, l);
            }
        }

        private void DoProcessEvent(EventDefinition ev, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            if (linkResolver.IsLink(ev.EventType))
            {
                l.Add(linkResolver.CreateLink(ev, ev.EventType));
            }
            DoProcessCustomAttributes(ev, ev, linkResolver, l);
        }

        private void DoProcessFields(TypeDefinition type, IEnumerable<FieldDefinition> fields, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            foreach (var field in fields)
            {
                DoProcessField(field, linkResolver, l);
            }
        }

        private void DoProcessField(FieldDefinition field, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            if (linkResolver.IsLink(field.FieldType))
            {
                l.Add(linkResolver.CreateLink(field, field.FieldType));
            }
        }

        private void DoProcessProperties(TypeDefinition type, IEnumerable<PropertyDefinition> properties, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            foreach (var prop in properties)
            {
                DoProcessProperty(prop, linkResolver, l);
            }
        }

        private void DoProcessMethods(TypeDefinition type, IEnumerable<MethodDefinition> methods, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            foreach (var method in methods)
            {
                DoProcessMethod(method, linkResolver, l);
            }
        }

        private void DoProcessProperty(PropertyDefinition property, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            if (property != null)
            {
                if (linkResolver.IsLink(property.PropertyType))
                {
                    l.Add(linkResolver.CreateLink(property, property.PropertyType));
                }
                DoProcessCustomAttributes(property, property, linkResolver, l);
            }
        }

        private void DoProcessCustomAttributes(MemberReference source, ICustomAttributeProvider customAttributeProvider, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            if (customAttributeProvider != null)
            {
                if (customAttributeProvider.HasCustomAttributes)
                {
                    foreach (var item in customAttributeProvider.CustomAttributes.Where(linkResolver.IsLink))
                    {
                        l.Add(linkResolver.CreateLink(source, item));
                    }
                }
            }
        }

        private bool DoProcessMethod(MethodDefinition method, IIsLinkResolver linkResolver, List<ReferenceItem> l)
        {
            if (method != null)
            {
                if (method.ReturnType != null)
                {
                    if (linkResolver.IsLink(method.ReturnType))
                    {
                        l.Add(linkResolver.CreateLink(method, method.ReturnType));
                    }
                }
                DoProcessCustomAttributes(method, method, linkResolver, l);
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
                    l.Add(linkResolver.CreateLink(method, item.VariableType));
                }
            }
            if (body.HasExceptionHandlers)
            {
                foreach (var item in body.ExceptionHandlers.Where(x => linkResolver.IsLink(x.CatchType)))
                {
                    if (item.HandlerType == ExceptionHandlerType.Catch)
                    {
                        l.Add(linkResolver.CreateLink(method, item.CatchType));
                    }
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
                            l.Add(linkResolver.CreateLink(method, mr));
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
                    l.Add(linkResolver.CreateLink(method, parameter.ParameterType));
                }
                if (parameter.HasCustomAttributes)
                {
                    foreach (var item in parameter.CustomAttributes.Where(linkResolver.IsLink))
                    {
                        l.Add(linkResolver.CreateLink(method, item));
                    }
                }
            }
        }

    }
}
