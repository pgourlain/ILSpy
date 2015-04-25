using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.ILSpy;
using Mono.Cecil;

namespace PgoPlugin.CallStackParser
{
    class ExceptionStackTraceParser
    {
        Regex genericReplacement = new Regex("(['<'][^'>']*['>'])");

        string _originalStackTrace;
        List<CallStackParserModel> parsedMethods = new List<CallStackParserModel>();
        ConcurrentDictionary<MethodDefinition, string> methodDefsAsSTring = new ConcurrentDictionary<MethodDefinition, string>();

        public ExceptionStackTraceParser(string originalStackTrace)
        {
            _originalStackTrace = originalStackTrace;
            ParseStack();
        }

        private void ParseStack()
        {
            bool inHeader = false;
            var lines = _originalStackTrace.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in lines)
            {
                if (item.StartsWith("["))
                {
                    this.parsedMethods.Add(new CallStackParserModelHeader { Header = item });
                    if (!item.EndsWith("]"))
                    {
                        inHeader = true;
                    }
                    continue;
                }
                else if (inHeader)
                {
                    ((CallStackParserModelHeader)this.parsedMethods[this.parsedMethods.Count - 1]).Header += item;
                    if (item.EndsWith("]"))
                    {
                        inHeader = false;
                    }
                    continue;
                }
                string methodName, fullMethodName;
                int rawOffset;
                if (ParseStringMethod(item, out methodName, out fullMethodName, out rawOffset))
                {
                    this.parsedMethods.Add(new CallStackParserModelMethod { FullMethodName = fullMethodName, MethodName = methodName, Offset = rawOffset });
                }

            }
        }

        public IEnumerable<CallStackParserModel> Resolve(IEnumerable<LoadedAssembly> assemblies)
        {
            Parallel.ForEach(assemblies, Resolve);
            return this.parsedMethods;
        }

        private void Resolve(LoadedAssembly assembly)
        {
            if (parsedMethods == null || parsedMethods.Count <= 0)
                return;
            ModuleDefinition module = assembly.ModuleDefinition;
            if (module == null)
                return;
            //CancellationToken cancellationToken = cts.Token;
            foreach (TypeDefinition type in module.Types)
            {
                foreach (var m in AllMethodsOf(type))
                {
                    Parallel.ForEach(this.parsedMethods.OfType<CallStackParserModelMethod>(), new ParallelOptions(), x => { CheckMatch(m, x); });
                }
            }
        }

        private void CheckMatch(MethodDefinition m, CallStackParserModelMethod ms)
        {
            if (m != null && m.Name == ms.MethodName)
            {
                if (IsMatch(m, ms.FullMethodName))
                {
                    ms.Definition = m;

                    //TODO: mapping of offset
                    //var inst = m.Body.Instructions.Where(x => x.Offset <= ms.Offset).LastOrDefault();
                    //if (inst != null)
                    //{
                    //    System.Diagnostics.Trace.WriteLine("hourra ! hourra ! hourra !");
                    //}
                }
            }
        }

        private IEnumerable<MethodDefinition> AllMethodsOf(TypeDefinition type)
        {
            if (type.HasMethods)
            {
                foreach (var m in type.Methods)
                {
                    yield return m;
                }
            }
            if (type.HasProperties)
            {

            }
        }

        private bool ParseStringMethod(string stringMethod, out string methodName, out string fullMethodName, out int offset)
        {
            methodName = string.Empty;
            fullMethodName = string.Empty;
            offset = 0;
            //"System.Reflection.RuntimeMethodInfo.UnsafeInvokeInternal(Object obj, Object[] parameters, Object[] arguments) +192";
            var i = stringMethod.IndexOf(')');
            if (i < 0) return false;

            var t = stringMethod.Substring(0, i + 1).Split('(');
            if (t.Length < 2) return false;
            fullMethodName = t[0].TrimStart(' ', '\t') + "(" + RemoveVarNames(t[1]) + ")";

            var rawOffset = i < stringMethod.Length ? stringMethod.Substring(i + 1).Trim() : "+0";
            methodName = fullMethodName.Substring(0, fullMethodName.IndexOf('('));
            t = methodName.Split('.');
            methodName = t[t.Length - 1];

            rawOffset = rawOffset.Trim(' ', '+');
            if (int.TryParse(rawOffset, out offset))
            {
                return true;
            }

            return false;
        }

        private string RemoveVarNames(string v)
        {
            var t = v.Split(' ');
            var a = t.Where(x => x.IndexOfAny(new char[] { ',', ')' }) == -1).ToArray();
            return string.Join(",", a);
        }

        private bool IsMatch(MethodDefinition m, string s)
        {
            string mds;
            if (methodDefsAsSTring.TryGetValue(m, out mds))
            {
                if (string.Compare(mds, s, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }
            else
            {
                string ms = ToString(m);
                methodDefsAsSTring.TryAdd(m, ms);

                if (string.Compare(ms, s, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }
            //TODO:
            return false;
        }

        private string ToString(MethodDefinition m)
        {
            string ms = m.ToString();
            if (ms.IndexOf('<') >= 0)
            {
                ms = genericReplacement.Replace(ms, "");
            }
            ms = ms.Substring(ms.IndexOf(' ')).Trim();
            ms = ms.Replace("::", ".");
            var start = ms.Substring(0, ms.IndexOf('('));
            var args = ms.Substring(ms.IndexOf('('));
            args = string.Join(",", args.Split(',').Select(x => { var t = x.Split('.'); return t[t.Length - 1]; }));
            ms = start + "(" + args;


            return ms;
        }
    }
}
