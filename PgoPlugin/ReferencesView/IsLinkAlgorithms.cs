using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PgoPlugin.ReferencesView
{

    interface IIsLinkResolver
    {
        bool IsLink(AssemblyNameReference current);

        bool IsLink(AssemblyNameDefinition current);
        bool IsLink(ModuleReference current);

        bool IsLink(IMetadataScope current);

        bool IsLink(TypeReference current);
        bool IsLink(CustomAttribute current);

        bool IsLink(MethodReference current);

    }

    class IsLinkToAssembly : IIsLinkResolver
    {
        AssemblyDefinition _targetAssembly;
        public IsLinkToAssembly(AssemblyDefinition targetAssembly)
        {
            _targetAssembly = targetAssembly;
        }

        public virtual bool IsLink(ModuleReference current)
        {
            return current.Name == _targetAssembly.Name.Name;
        }

        public virtual bool IsLink(TypeReference current)
        {
            return current != null && IsLink(current.Module.Assembly.Name);
        }

        public virtual bool IsLink(MethodReference current)
        {
            return current != null && IsLink(current.Module.Assembly.Name);
        }

        public virtual bool IsLink(CustomAttribute current)
        {
            return IsLink(current.AttributeType);
        }

        public virtual bool IsLink(IMetadataScope current)
        {
            return current.Name == _targetAssembly.Name.Name;
        }

        public virtual bool IsLink(AssemblyNameDefinition current)
        {
            if (current.Name == _targetAssembly.Name.Name && current.PublicKeyToken == _targetAssembly.Name.PublicKeyToken)
            {
                return true;
            }
            return false;
        }

        public virtual bool IsLink(AssemblyNameReference current)
        {
            return current.Name == _targetAssembly.Name.Name;
        }
    }

    class IsLinkToType : IsLinkToAssembly
    {
        public IsLinkToType(TypeDefinition type) : base (type.Module.Assembly)
        {

        }
    }

    class IsLinkToMemberReference
    {

    }
}
