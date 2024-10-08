﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public abstract class LayerBase
        : ChartElementBase
    {
        #region Fields
        internal UIElementCollection _children;

        private ChartBase _chart;
        #endregion

        #region Ctor
        public LayerBase()
        {
            _children = new UIElementCollection(this, this);
        }
        #endregion

        #region Methods
        internal void OnAttached(ChartBase chart)
        {
            _chart = chart;
        }

        internal void MouseIn(
            IChartContext chartContext
        )
        {
            OnMouseIn(chartContext);
        }

        internal void MouseOut(
            IChartContext chartContext
        )
        {
            OnMouseOut(chartContext);
        }
        #endregion

        #region Overrides
        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach(UIElement child in _children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in _children)
            {
                child.Arrange(new Rect(finalSize));
            }
            return base.ArrangeOverride(finalSize);
        }

        protected sealed override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if(_chart == null)
            {
                return;
            }

            var context = _chart.CreateDrawingContext(drawingContext);
            var chartContext = _chart.GetCanvasContext();

            OnRender(
                context,
                chartContext
            );
        }

        protected sealed override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected sealed override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected sealed override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
        }
        #endregion

        #region Protected Methods
        protected abstract void OnMouseIn(
            IChartContext chartContext
        );

        protected abstract void OnMouseOut(
            IChartContext chartContext
        );

        protected abstract void OnRender(
            IDrawingContext drawingContext,
            IChartContext chartContext
        );

        protected void AddChild(UIElement child)
        {
            _children.Add(child);
        }

        protected void RemoveChild(UIElement child)
        {
            _children.Remove(child);
        }

        protected static void OnInvalidRenderPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var layer = (LayerBase)d;
            layer.InvalidateVisual();
        }
        #endregion
    }
}
