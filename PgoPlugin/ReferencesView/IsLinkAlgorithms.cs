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
        ReferenceItem CreateLink(TypeDefinition type, TypeReference linkedTo);
        ReferenceItem CreateLink(TypeDefinition type, CustomAttribute item);
        ReferenceItem CreateLink(MemberReference method, CustomAttribute item);
        ReferenceItem CreateLink(MethodDefinition method, MethodReference mr);
        ReferenceItem CreateLink(MemberReference method, TypeReference returnType);
        ReferenceItem CreateLink(PropertyDefinition property, TypeReference propertyType);
    }

    class IsLinkToAssembly : IIsLinkResolver
    {
        AssemblyDefinition _targetAssembly;
        public IsLinkToAssembly(AssemblyDefinition targetAssembly)
        {
            _targetAssembly = targetAssembly;
        }

        protected string getAssemblyName(IMetadataScope scope)
        {
            AssemblyNameReference refName = scope as AssemblyNameReference;
            if (refName != null)
                return refName.FullName;
            return scope.Name;
        }

        public virtual bool IsLink(ModuleReference current)
        {
            return current.Name == _targetAssembly.Name.Name;
        }

        public virtual bool IsLink(TypeReference current)
        {
            return current != null && IsLink(current.Scope);
        }

        public virtual bool IsLink(MethodReference current)
        {
            return current != null && IsLink(current.DeclaringType.Scope);
        }

        public virtual bool IsLink(CustomAttribute current)
        {
            return IsLink(current.AttributeType);
        }

        public virtual bool IsLink(IMetadataScope current)
        {
            var md = (current as ModuleDefinition);
            if (md != null) return md.Assembly.Name.Name == _targetAssembly.Name.Name;
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

        #region CreateLinks
        public virtual ReferenceItem CreateLink(TypeDefinition type, TypeReference linkedTo)
        {
            return new ReferenceItem(type.Module.Assembly.FullName, type.FullName);
        }
        public virtual ReferenceItem CreateLink(TypeDefinition type, CustomAttribute linkedTo)
        {
            return new ReferenceItem(type.Module.Assembly.FullName, type.FullName);
        }
        public virtual ReferenceItem CreateLink(MemberReference method, CustomAttribute linkedTo)
        {
            return new ReferenceItem(method.DeclaringType.Module.Assembly.FullName, method.DeclaringType.FullName, method as MethodReference);
        }
        public virtual ReferenceItem CreateLink(MethodDefinition method, MethodReference linkedTo)
        {
            return new ReferenceItem(method.DeclaringType.Module.Assembly.FullName, method.DeclaringType.FullName, method);
        }
        public virtual ReferenceItem CreateLink(MemberReference method, TypeReference linkedTo)
        {
            return new ReferenceItem(method.DeclaringType.Module.Assembly.FullName, method.DeclaringType.FullName, method as MethodReference);
        }
        public virtual ReferenceItem CreateLink(PropertyDefinition property, TypeReference propertyType)
        {
            return new ReferenceItem(property.DeclaringType.Module.Assembly.FullName, property.DeclaringType.FullName);
        }

        #endregion
    }

    class IsLinkToType : IsLinkToAssembly
    {
        TypeDefinition _targetType;
        public IsLinkToType(TypeDefinition type) : base (type.Module.Assembly)
        {
            _targetType = type;
        }

        public override bool IsLink(TypeReference current)
        {
            return base.IsLink(current) && current.FullName == _targetType.FullName;
        }
    }

    /// <summary>
    /// in order to check that a link exist in assembly X.Assembly and not to type X
    /// </summary>
    class IsLinkOutOfthisType : IsLinkToAssembly
    {
        TypeDefinition _targetType;
        bool _checkInThisAssembly = true;
        public IsLinkOutOfthisType(TypeDefinition type, bool checkInThisAssembly) : base (type.Module.Assembly)
        {
            _targetType = type;
            _checkInThisAssembly = checkInThisAssembly;
        }
        public override bool IsLink(TypeReference current)
        {
            var result = current != null;
            if (_checkInThisAssembly)
            {
                result = result & base.IsLink(current);
            }
            return result && getTypeFulName(current) != _targetType.FullName;
        }

        public override bool IsLink(MethodReference current)
        {
            var result = current != null;
            if (_checkInThisAssembly)
            {
                result = result & base.IsLink(current);
            }
            return result && (current.DeclaringType.FullName != _targetType.FullName);
        }
        #region CreateLinks
        public override ReferenceItem CreateLink(TypeDefinition type, TypeReference linkedTo)
        {
            return new ReferenceItem(getAssemblyName(linkedTo.Scope), getTypeFulName(linkedTo));
        }

        /// <summary>
        /// retourne le type d'une reference qui peut être un tableau de tableau de tableau
        /// </summary>
        /// <param name="linkedTo"></param>
        /// <returns></returns>
        private string getTypeFulName(TypeReference linkedTo)
        {
            if (linkedTo.IsArray)
            {
                return getTypeFulName(((ArrayType)linkedTo).ElementType);
            }
            else
                return linkedTo.FullName;
        }

        public override ReferenceItem CreateLink(MethodDefinition method, MethodReference linkedTo)
        {
            return new ReferenceItem(getAssemblyName(linkedTo.DeclaringType.Scope), linkedTo.DeclaringType.FullName, linkedTo);
        }
        public override ReferenceItem CreateLink(MemberReference method, TypeReference linkedTo)
        {
            return new ReferenceItem(getAssemblyName(linkedTo.Scope), getTypeFulName(linkedTo));
        }
        public override ReferenceItem CreateLink(PropertyDefinition property, TypeReference linkedTo)
        {
            return new ReferenceItem(getAssemblyName(linkedTo.Scope), getTypeFulName(linkedTo));
        }

        #endregion
    }

    class IsLinkToMemberReference
    {

    }
}
