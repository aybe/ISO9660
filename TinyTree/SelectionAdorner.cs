using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using JetBrains.Annotations;

namespace TinyTree
{
    public sealed class SelectionAdorner : Adorner
    {
        public SelectionAdorner([NotNull] UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false;
            IsEnabledChanged += (sender, args) => { InvalidateVisual(); };
        }

        public Rect SelectionArea { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!IsEnabled) return;
            var x = new[] {SelectionArea.Left + 0.5d, SelectionArea.Right + 0.5d};
            var y = new[] {SelectionArea.Top + 0.5d, SelectionArea.Bottom + 0.5d};
            var set = new GuidelineSet(x, y);
            drawingContext.PushGuidelineSet(set);
            var fill = SystemColors.HighlightBrush.Clone();
            fill.Opacity = 0.4d;
            drawingContext.DrawRectangle(
                fill,
                new Pen(SystemColors.HighlightBrush, 1.0),
                SelectionArea
                );
        }
        //http://www.codeproject.com/Articles/209560/ListBox-drag-selection
        public static T FindChild<T>([NotNull] DependencyObject o) where T : class
        {
            if (o == null) throw new ArgumentNullException(nameof(o));
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(o);
            while (queue.Count > 0)
            {
                var child = queue.Dequeue();
                var obj = child as T;
                if (obj != null) return obj;

                var count = VisualTreeHelper.GetChildrenCount(child);
                for (var i = 0; i < count; i++)
                {
                    var item = VisualTreeHelper.GetChild(child, i);
                    queue.Enqueue(item);
                }
            }
            return null;
        }private static int GetRepeatRate()
{
    // The RepeatButton uses the SystemParameters.KeyboardSpeed as the
    // default value for the Interval property. KeyboardSpeed returns
    // a value between 0 (400ms) and 31 (33ms).
    const double Ratio = (400.0 - 33.0) / 31.0;
    return 400 - (int)(SystemParameters.KeyboardSpeed * Ratio);
}
    }
}