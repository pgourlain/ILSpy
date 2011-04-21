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
        static Dictionary<AstType, string> _dico = new Dictionary<AstType, string>();
        static Dictionary<string, string> _dicoCecil = new Dictionary<string, string>();

        internal static string MakeReadable(AstType astType, string memberName)
        {
            //if (!IsReadable(memberName))
            //{
            //    string result;
            //    if (!_dico.TryGetValue(astType, out result))
            //    {
            //        counter++;
            //        result = string.Format("T_{0}", counter);
            //        _dico.Add(astType, result);
            //    }
            //    return result;
            //}
            return memberName;
        }

        private static bool IsReadable(string memberName)
        {
            return memberName.All(c => c >= 32);
        }

        public static string MakeReadable(MemberReference type, string memberName, string prefix)
        {
            if (!IsReadable(memberName))
            {
                var key = string.Format("{0}_{1}", type.MetadataToken.ToInt32(), memberName);
                string result;
                if (!_dicoCecil.TryGetValue(key, out result))
                {
                    counter++;
                    if (string.IsNullOrEmpty(prefix))
                        result = string.Format("Tref_{0}", counter);
                    else
                        result = string.Format("{0}_{1}", prefix, counter);
                    _dicoCecil.Add(key, result);
                }
                return result;
                
            }
            return memberName;
        }
    }
}
