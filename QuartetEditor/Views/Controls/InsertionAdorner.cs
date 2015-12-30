using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace QuartetEditor.Views.Controls
{
    public class InsertionAdorner : ControlHostAdornerBase
    {
        private readonly HorizontalInsertionCursor _insertionCursor;

        public InsertionAdorner(UIElement adornedElement, VerticalAlignment position)
            : base(adornedElement)
        {
            this._insertionCursor = new HorizontalInsertionCursor();

            Host.Children.Add(this._insertionCursor);

            this._insertionCursor.SetValue(VerticalAlignmentProperty, position);


            this._insertionCursor.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        }
    }

    public class ControlHostAdornerBase : Adorner, IDisposable
    {
        /// <summary>
        /// ゴーストを表示するレイヤー
        /// </summary>
        private AdornerLayer _adornerLayer;

        /// <summary>
        /// ゴーストを表示するGrid
        /// </summary>
        protected Grid Host { get; set; }

        protected ControlHostAdornerBase(UIElement adornedElement)
            : base(adornedElement)
        {
            this._adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.Host = new Grid();

            if (AdornerLayer != null)
            {
                AdornerLayer.Add(this);
            }
        }


        public void Detach()
        {
            AdornerLayer.Remove(this);
        }

        protected AdornerLayer AdornerLayer
        {
            get { return this._adornerLayer; }
        }

        /// <summary>
        /// Override of VisualChildrenCount.
        /// Always return 1
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Host.Measure(constraint);
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Host.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (VisualChildrenCount <= index)
            {
                throw new IndexOutOfRangeException();
            }
            return Host;
        }

        #region Dispose

        private bool _disposed;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        ~ControlHostAdornerBase()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                Detach();
            }
        }

        #endregion

    }
}
