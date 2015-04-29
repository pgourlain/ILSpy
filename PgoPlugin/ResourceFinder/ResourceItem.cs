using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PgoPlugin.ResourceFinder
{
    public class ResourceItem
    {
        public string AssemblyName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }
        public Resource Resource { get; set; }
    }
}
