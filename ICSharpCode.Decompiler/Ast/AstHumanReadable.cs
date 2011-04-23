using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.Ast
{
    public static class AstHumanReadable
    {
        static int counter = 0;
        static Dictionary<string, string> _dicoCecil = new Dictionary<string, string>();
        static Dictionary<string, bool> _readable = new Dictionary<string, bool>();

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

        public static string MakeReadable(IMetadataTokenProvider type, string memberName, string prefix)
        {
            if (!IsReadable(memberName))
            {
                string key = string.Empty;
                if (prefix == "namespace")
                {
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
                //if (fr.FieldType.IsValueType)
                //{
                //    return fr.FieldType.Name.Substring(0, 1).ToLower() + "_";
                //}
                //else if (string.CompareOrdinal("String", fr.FieldType.Name) == 0)
                //{
                //    return "s_";
                //}
                //else
                //{
                //    //StringBuilder sb = new StringBuilder();
                //    //foreach (var c in fr.FieldType.Name)
                //    //{
                //    //    if (Char.IsUpper(c))
                //    //        sb.Append(char.ToLower(c));
                //    //}
                //    //return sb.ToString();
                //    //OR
                //    return new string(fr.FieldType.Name.Where(c => char.IsUpper(c)).Select(c => Char.ToLower(c)).ToArray());
                //}
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
