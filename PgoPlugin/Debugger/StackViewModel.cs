using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debugger;
using ICSharpCode.Decompiler;
//using ICSharpCode.ILSpy.Debugger;

namespace PgoPlugin.Debugger
{
    public class StackViewModel : ObservableObject
    {
        static SourceCodeMapping GetCurrentCodeMapping(StackFrame frame, out bool isMatch)
        {
            //// it works only with the first frame

            isMatch = false;
            //int key = frame.MethodInfo.MetadataToken;
            //List<MemberMapping> mapping;
            //// get the mapped instruction from the current line marker or the next one
            //if (DebugData.CodeMappings.TryGetValue(key, out mapping))
            //{
            //    return mapping.GetInstructionByTokenAndOffset(key, frame.IP, out isMatch);
            //}
            return null;
        }


        public static StackViewModel FromStackFrame(StackFrame frame)
        {
            bool isMatch;
            var mi = frame.MethodInfo;
            var map = GetCurrentCodeMapping(frame, out isMatch);
            return new StackViewModel
            {
                Method = mi.ToString(),
                SourceCodeLine = map != null ? map.SourceCodeLine : -1,
                CodeMapping = map
                ,Assembly = mi.DebugModule.Name

            };
        }

        public string Method { get; private set; }
        public int SourceCodeLine { get; private set; }
        public SourceCodeMapping CodeMapping { get; private set; }
        public string Assembly { get; private set; }
    }
}
