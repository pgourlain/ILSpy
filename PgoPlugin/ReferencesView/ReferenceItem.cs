using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PgoPlugin.ReferencesView
{
    public class ReferenceItem
    {

        public ReferenceItem(string name)
        {
            this.AssemblyName = name;
        }

        public string AssemblyName { get; set; }
    }
}
