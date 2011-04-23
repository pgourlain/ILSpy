﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ICSharpCode.Decompiler.Ast
{
    public static class AstHumanReadable
    {
        static int counter = 0;
        static Dictionary<string, string> _dicoCecil = new Dictionary<string, string>();
        static Dictionary<string, bool> _readable = new Dictionary<string, bool>();

        #region constants
        public static readonly string Field = "field";
        public static readonly string Parameter = "parameter";
        public static readonly string GenericType = "T";
        public static readonly string NestedType = "nestedtype";
        public static readonly string Type = "type";
        public static readonly string Namespace = "namespace";
        public static readonly string Event = "event";
        public static readonly string Method = "method";
        public static readonly string Property = "property";
        public static readonly string Variable = "variable";
        public static readonly string Module = "module";
        public static readonly string Resource = "resource";

        #endregion

        private static bool IsReadable(string memberName)
        {
            bool result;
            if (!_readable.TryGetValue(memberName, out result))
            {
                result = memberName.Length > 0 && memberName.All(c => c >= 32 && c != 127 && !Char.IsControl(c));
                _readable.Add(memberName, result);
            }
            return result;
        }

        public static string MakeReadable(VariableDefinition var, string varName, string prefix)
        {
            return MakeReadable(varName, "var");
        }
        
        static string MakeReadable(string name, string prefix)
        {
            if (!IsReadable(name))
            {
                var key = string.Format("{0}_{1}", prefix, name);
                string result;
                if (!_dicoCecil.TryGetValue(key, out result))
                {
                    counter++;
                    result = string.Format("{0}_{1}", prefix, counter);
                    _dicoCecil.Add(key, result);
                }
                return result;
            }
            return name;
        }

        public static string MakeReadable(Mono.Cecil.Resource r, string resName, string prefix)
        {
            return MakeReadable(resName, "resource");
        }

        public static string MakeReadable(IMetadataTokenProvider type, string memberName, string prefix)
        {
            if (!IsReadable(memberName))
            {
                string key = string.Empty;
                if (prefix == AstHumanReadable.Namespace)
                {
                    if (string.IsNullOrEmpty(memberName))
                        return string.Empty;
                    //special case for namespace, it's associated to a type
                    key = string.Format("namespace_{0}", memberName);
                }
                else
                {
                    var methodDef = type as MethodDefinition;
                    if (methodDef != null)
                    {
                        if (methodDef != null && methodDef.HasOverrides)
                        {
                            //we should find the name of the base method
                            var m = methodDef.Overrides.First();
                            var baseMethodDef = m.Resolve();
                            return MakeReadable(baseMethodDef, memberName, prefix);
                        }
                        key = string.Format("{0}.{1}.{2}", methodDef.DeclaringType.Namespace, methodDef.DeclaringType.Namespace, methodDef.Name);
                    }
                    else if (type is MethodReference)
                    {
                        key = string.Format("{0}_{1}", type.MetadataToken.ToInt32(), memberName);
                    }
                    else
                        key = string.Format("{0}_{1}", type.MetadataToken.ToInt32(), memberName);
                }
                string result;
                if (!_dicoCecil.TryGetValue(key, out result))
                {
                    counter++;
                    if (string.IsNullOrEmpty(prefix))
                        result = string.Format("type_{0}", counter);
                    else
                        result = string.Format("{2}{0}_{1}", prefix, counter, GetPrefixType(type));
                    _dicoCecil.Add(key, result);
                }
                return result;
                
            }
            return memberName;
        }

        /// <summary>
        /// add prefix for serveral types like fields
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetPrefixType(IMetadataTokenProvider type)
        {
            var fr = type as FieldReference;
            if (fr != null)
            {
                StringBuilder sb = new StringBuilder("_");
                sb.Append(MakeReadable(fr.FieldType, fr.FieldType.Name, ""));
                sb.Append("_");
                return sb.ToString();
            }
            else
            {
                var tr = type as TypeReference;
                if (tr != null)
                {
                    if (tr.IsNested)
                    {
                        var typeDef = tr.ResolveWithinSameModule();
                        if (typeDef != null)
                        {
                            if (typeDef.IsInterface)
                                return "I";
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
