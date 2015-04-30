using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.ILSpy.TextView;
using Mono.Cecil;

namespace PgoPlugin.DecompilerViewExtensions
{
    [Export(typeof(IDecompilationViewEvents))]
    public class SmartTagPopup : IDecompilationViewEvents
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        SmartTagAdorner adOrner= null;
        [ImportingConstructor]
        public SmartTagPopup(DecompilerTextView decompilerTextView)
        {
            adOrner = new SmartTagAdorner(decompilerTextView);
            AdornerLayer.GetAdornerLayer(decompilerTextView).Add(adOrner);
        }

        public void MouseHover(DecompilerTextView decompilerTextView, MouseEventArgs mouseArgs, ReferenceSegment seg)
        {
            if (dispatcherTimer != null)
                dispatcherTimer.Stop();
            var content = GenerateSmartTag(seg);
            if (adOrner != null && adOrner.Child == null)
            {
                adOrner.Child = content;
            }
            else
            {
                adOrner.Child.Visibility = Visibility.Visible;
            }
            var position = mouseArgs.GetPosition(adOrner);
            position.X = 0;
            //position to specific Y, but on the left of DecompilerView
            adOrner.LayoutTransform = new TranslateTransform(0, position.Y);

        }

        public void MouseHoverStopped()
        {
            if (dispatcherTimer == null)
            {
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
                dispatcherTimer.Tick += DispatcherTimer_Tick;
            }
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (dispatcherTimer != null) dispatcherTimer.Stop();
            if (adOrner != null && adOrner.Child != null) adOrner.Child.Visibility = Visibility.Collapsed;
        }

        FrameworkElement GenerateSmartTag(ReferenceSegment segment)
        {
            if (segment != null && segment.Reference is MemberReference)
            {

                var cc = new SmartTagUI();
                cc.MouseEnter += Cc_MouseEnter;
                cc.MouseLeave += Cc_MouseLeave;
                return cc;
            }
            return null;
        }

        private void Cc_MouseLeave(object sender, MouseEventArgs e)
        {
            dispatcherTimer.Start();
        }

        private void Cc_MouseEnter(object sender, MouseEventArgs e)
        {
            dispatcherTimer.Stop();
        }
    }
}
