using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.ILSpy.TextView
{
    public interface IDecompilationViewEvents
    {
        void MouseHoverStopped();
        void MouseHover(DecompilerTextView decompilerTextView, MouseEventArgs mouseArgs, ReferenceSegment seg);
    }
}
