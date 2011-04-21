using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ICSharpCode.NRefactory.CSharp
{
    public static class AstHumanReadable
    {
        static int counter = 0;
        static Dictionary<string, string> _dicoCecil = new Dictionary<string, string>();

        private static bool IsReadable(string memberName)
        {
            return memberName.All(c => c >= 32);
        }

        public static string MakeReadable(IMetadataTokenProvider type, string memberName, string prefix)
        {
            if (!IsReadable(memberName))
            {
                var key = string.Format("{0}_{1}", type.MetadataToken.ToInt32(), memberName);
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
                if (fr.FieldType.IsValueType)
                {
                    return fr.FieldType.Name.Substring(0,1).ToLower()+"_";
                }
                else if (string.CompareOrdinal("String", fr.FieldType.Name) == 0)
                {
                    return "s_";
                }
            }
            //var tRef = (type as TypeReference);
            //if (tRef != null)
            //{
            //    if (tRef.IsValueType)
            //}
            return string.Empty;
        }
    }
}
