using Debugger;
using Debugger.MetaData;
using ICSharpCode.ILSpy.Debugger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PgoPlugin.Debugger
{
    class Dumper
    {
        internal static string Evaluate(IDebugger debugger, string expression)
        {
            if (debugger.CanEvaluate)
            {
                try
                {
                    return debugger.GetValueAsString(expression);
                }
                catch(System.Exception ex)
                {
                    return string.Format("error while evaluating '{0}' : {1}", expression, ex);
                }
                //(debugger as WindowsDebugger).GetExpression()
            }
            return "debugger can't evaluate";
        }
    }
}
