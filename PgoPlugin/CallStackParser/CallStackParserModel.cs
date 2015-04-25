using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PgoPlugin.CallStackParser
{
    public class CallStackParserModel
    {
        public string DisplayText
        {
            get
            {
                return this.ToString();
            }
        }

        public virtual bool HasMethodDefinition
        {
            get
            {
                return false;
            }
        }
    }

    public class CallStackParserModelHeader : CallStackParserModel
    {
        public string Header { get; set; }

        public override string ToString()
        {
            return this.Header;
        }
    }

    public class CallStackParserModelMethod : CallStackParserModel
    {
        public MethodDefinition Definition { get; internal set; }
        public string FullMethodName { get; internal set; }
        public string MethodName { get; internal set; }
        public int Offset { get; internal set; }

        public override string ToString()
        {
            return FullMethodName;
        }

        public override bool HasMethodDefinition
        {
            get
            {
                return this.Definition != null;
            }
        }
    }
}
