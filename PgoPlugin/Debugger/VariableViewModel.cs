using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debugger;
using ICSharpCode.Decompiler;
using Debugger.MetaData;
using PgoPlugin.UIHelper;
//using ICSharpCode.ILSpy.Debugger;

namespace PgoPlugin.Debugger
{
    public class VariableViewModel : ObservableObject
    {
        public static VariableViewModel FromVar(DebugLocalVariableInfo v, StackFrame frame)
        {
            VariableViewModel result = new VariableViewModel();
            result.VariableName = v.Name;
            try
            {
                var Value = v.GetValue(frame);
                result.ValueUnderDebugger = Value;
                result.VariableValue = Value.InvokeToString();
                result.VariableType = Value.Type.ToString();
            }
            catch (GetValueException ex)
            {
                result.VariableValue = ex.Message;
                result.VariableType = string.Empty;
            }
            return result;
        }

        public static VariableViewModel FromParameter(DebugParameterInfo v, StackFrame frame)
        {
            VariableViewModel result = new VariableViewModel();
            result.VariableName = v.Name;
            try
            {
                var Value = v.GetValue(frame);
                result.ValueUnderDebugger = Value;
                result.VariableValue = Value.InvokeToString();
                result.VariableType = Value.Type.ToString();
            }
            catch (GetValueException ex)
            {
                result.VariableValue = ex.Message;
                result.VariableType = string.Empty;
            }
            return result;
        }

        public string VariableName { get; private set; }
        public string VariableType { get; set; }
        public string VariableValue { get; private set; }

        public Value ValueUnderDebugger { get; set; }

    }
}
