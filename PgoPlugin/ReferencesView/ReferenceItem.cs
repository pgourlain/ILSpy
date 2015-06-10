using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PgoPlugin.ReferencesView
{
    public class ReferenceItem
    {
        public ReferenceItem(string assemblyName)
        {
            this.AssemblyName = assemblyName;
        }
        public ReferenceItem(string assemblyName, string typeFullName) 
            : this(assemblyName)
        {
            this.TypeFullName = typeFullName;
        }
        public ReferenceItem(string assemblyName, string typeFullName, MethodReference method)
            : this(assemblyName, typeFullName)
        {
            this.Method = method;
        }

        public string AssemblyName { get; set; }
        public string TypeFullName { get; set; }
        public MethodReference Method { get; set; }

        public string MethodName
        {
            get
            {
                if (this.Method != null)
                {
                    return this.Method.FullName;
                }
                return string.Empty;
            }
        }
    }

    class ReferenceItemComparer : EqualityComparer<ReferenceItem>
    {
        public ReferenceItemComparer()
        {
        }

        public override bool Equals(ReferenceItem x, ReferenceItem y)
        {
            return x.AssemblyName == y.AssemblyName && x.TypeFullName == y.TypeFullName && x.MethodName == y.MethodName;
        }

        public override int GetHashCode(ReferenceItem obj)
        {
            return obj.AssemblyName.GetHashCode() ^ obj.TypeFullName.GetHashCode() ^ obj.MethodName.GetHashCode();
        }
    }


}
