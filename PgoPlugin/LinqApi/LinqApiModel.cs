using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PgoPlugin.LinqApi
{
    class LinqApiModel
    {
        public MethodDefinition Method { get; set; }
        public string ReturnType { get; set; }
        public string MethodName { get; set; }
        public string ExtendedType { get; set; }
        public string Parameters { get; set; }

        public string FullDefinition
        {
            get
            {
                return string.Format("{0} {1}{2}", this.ReturnType, this.MethodName, this.Parameters);
            }
        }

        string _assemblyName = string.Empty;
        public string AssemblyName
        {
            get
            {
                if (this.Method != null)
                {
                    _assemblyName =this.Method.DeclaringType.Module.Assembly.Name.FullName;
                }
                return _assemblyName; 
            }
        }
    }
}
