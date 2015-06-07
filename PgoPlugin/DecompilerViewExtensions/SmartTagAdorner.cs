using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PgoPlugin.DecompilerViewExtensions
{
    class SmartTagAdorner<TChild> : Adorner where TChild : FrameworkElement
    {
        private TChild _child;
        public SmartTagAdorner(UIElement adornedElement) : base(adornedElement)
        {

        }

        protected override int VisualChildrenCount
        {
            get
            {
                if (_child == null) return 0;
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            if (_child == null) throw new ArgumentOutOfRangeException();
            return _child;
        }

        public TChild Child
        {
            get { return _child; }
            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }
                _child = value;
                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Red, 2), new Rect(new Point(0, 0), this.AdornedElement.DesiredSize));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_child == null) return base.MeasureOverride(constraint);
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_child == null) return base.ArrangeOverride(finalSize);
            var r = this.LayoutTransform.TransformBounds(new Rect(new Point(0, 0), finalSize));
            _child.Arrange(r);
            return new Size(_child.ActualWidth, _child.ActualHeight);
        }
    }
}
