// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.Decompiler;
using Mono.Cecil;
using ICSharpCode.Decompiler.Ast;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public sealed class TypeTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		readonly TypeDefinition type;
		readonly AssemblyTreeNode parentAssemblyNode;
		
		public TypeTreeNode(TypeDefinition type, AssemblyTreeNode parentAssemblyNode)
		{
			if (parentAssemblyNode == null)
				throw new ArgumentNullException("parentAssemblyNode");
			if (type == null)
				throw new ArgumentNullException("type");
			this.type = type;
			this.parentAssemblyNode = parentAssemblyNode;
			this.LazyLoading = true;
            this.Name = AstHumanReadable.MakeReadable(type, type.Name, AstHumanReadable.Type);
            this.Namespace = AstHumanReadable.MakeReadable(type, type.Namespace, AstHumanReadable.Namespace);
		}
		
		public TypeDefinition TypeDefinition {
			get { return type; }
		}
		
		public AssemblyTreeNode ParentAssemblyNode {
			get { return parentAssemblyNode; }
		}

        public string Namespace
        {
            get;
            private set;
        }
		
		public override object Text {
			get { return HighlightSearchMatch(this.Language.TypeToString(type, includeNamespace: false)); }
		}
		
		public bool IsPublicAPI {
			get {
				switch (type.Attributes & TypeAttributes.VisibilityMask) {
					case TypeAttributes.Public:
					case TypeAttributes.NestedPublic:
					case TypeAttributes.NestedFamily:
					case TypeAttributes.NestedFamORAssem:
						return true;
					default:
						return false;
				}
			}
		}
		
		public override FilterResult Filter(FilterSettings settings)
		{
			if (!settings.ShowInternalApi && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(this.Name)) {
				if (type.IsNested && !settings.Language.ShowMember(type))
					return FilterResult.Hidden;
				else
					return FilterResult.Match;
			} else {
				return FilterResult.Recurse;
			}
		}
		
		protected override void LoadChildren()
		{
			if (type.BaseType != null || type.HasInterfaces)
				this.Children.Add(new BaseTypesTreeNode(type));
			if (!type.IsSealed)
				this.Children.Add(new DerivedTypesTreeNode(parentAssemblyNode.AssemblyList, type));

            this.Children.AddRange(type.NestedTypes.Select(nestedType => new TypeTreeNode(nestedType, parentAssemblyNode)).OrderBy(x => x.Name));
            //foreach (TypeDefinition nestedType in type.NestedTypes.OrderBy(m => m.Name)) {
            //    this.Children.Add(new TypeTreeNode(nestedType, parentAssemblyNode));
            //}
            this.Children.AddRange(type.Fields.Select(field => new FieldTreeNode(field)).OrderBy(x => x.Name));
            
            //foreach (FieldDefinition field in type.Fields.OrderBy(m => m.Name))
            //{
            //    this.Children.Add(new FieldTreeNode(field));
            //}

            this.Children.AddRange(type.Properties.Select(property => new PropertyTreeNode(property)).OrderBy(x => x.Name));
            //foreach (PropertyDefinition property in type.Properties.OrderBy(m => m.Name))
            //{
            //    this.Children.Add(new PropertyTreeNode(property));
            //}

            this.Children.AddRange(type.Events.Select(ev => new EventTreeNode(ev)).OrderBy(x => x.Name));
            //foreach (EventDefinition ev in type.Events.OrderBy(m => m.Name))
            //{
            //    this.Children.Add(new EventTreeNode(ev));
            //}
			HashSet<MethodDefinition> accessorMethods = type.GetAccessorMethods();
            this.Children.AddRange(type.Methods.Where(method => !accessorMethods.Contains(method))
                .Select(m => new MethodTreeNode(m)).OrderBy(m => m.Name));
            //foreach (MethodDefinition method in type.Methods.OrderBy(m => m.Name)) {
            //    if (!accessorMethods.Contains(method)) {
            //        this.Children.Add(new MethodTreeNode(method));
            //    }
            //}
		}
		
		public override bool CanExpandRecursively {
			get { return true; }
		}
		
		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.DecompileType(type, output, options);
		}

		#region Icon
		public override object Icon
		{
			get { return GetIcon(type); }
		}

		public static ImageSource GetIcon(TypeDefinition type)
		{
			TypeIcon typeIcon = GetTypeIcon(type);
			AccessOverlayIcon overlayIcon = GetOverlayIcon(type);

			return Images.GetIcon(typeIcon, overlayIcon);
		}

		static TypeIcon GetTypeIcon(TypeDefinition type)
		{
			if (type.IsValueType) {
				if (type.IsEnum)
					return TypeIcon.Enum;
				else
					return TypeIcon.Struct;
			} else {
				if (type.IsInterface)
					return TypeIcon.Interface;
				else if (IsDelegate(type))
					return TypeIcon.Delegate;
				else if (IsStaticClass(type))
					return TypeIcon.StaticClass;
				else
					return TypeIcon.Class;
			}
		}

		private static AccessOverlayIcon GetOverlayIcon(TypeDefinition type)
		{
			AccessOverlayIcon overlay;
			switch (type.Attributes & TypeAttributes.VisibilityMask) {
				case TypeAttributes.Public:
				case TypeAttributes.NestedPublic:
					overlay = AccessOverlayIcon.Public;
					break;
				case TypeAttributes.NotPublic:
				case TypeAttributes.NestedAssembly:
				case TypeAttributes.NestedFamANDAssem:
					overlay = AccessOverlayIcon.Internal;
					break;
				case TypeAttributes.NestedFamily:
				case TypeAttributes.NestedFamORAssem:
					overlay = AccessOverlayIcon.Protected;
					break;
				case TypeAttributes.NestedPrivate:
					overlay = AccessOverlayIcon.Private;
					break;
				default:
					throw new NotSupportedException();
			}
			return overlay;
		}

		private static bool IsDelegate(TypeDefinition type)
		{
			return type.BaseType != null && type.BaseType.FullName == typeof(MulticastDelegate).FullName;
		}

		private static bool IsStaticClass(TypeDefinition type)
		{
			return type.IsSealed && type.IsAbstract;
		}

		#endregion
		
		MemberReference IMemberTreeNode.Member {
			get { return type; }
		}

        public override bool IsPublicAccess()
        {
            if (type != null)
            {
                return type.IsPublic;
            }
            return true;
        }
	}
}
