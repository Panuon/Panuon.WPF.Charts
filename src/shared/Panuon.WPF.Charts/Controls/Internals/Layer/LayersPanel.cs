using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class LayersPanel
        : FrameworkElement
    {
        #region Fields
        private ChartBase _chart;

        private UIElementCollection _children;

        private IList<ICoordinate> _coordinates;
        #endregion

        #region Ctor
        internal LayersPanel(ChartBase chartPanel)
        {
            _chart = chartPanel;
            _chart.Layers.CollectionChanged += ChartPanelLayers_CollectionChanged;

            _children = new UIElementCollection(this, this);
        }
        #endregion

        #region Overrides
        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (LayerBase child in _children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (LayerBase child in _children)
            {
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var chartContext = _chart.GetCanvasContext();

            foreach (LayerBase child in _children)
            {
                child.MouseIn(
                    chartContext
                );
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            var chartContext = _chart.GetCanvasContext();

            foreach (LayerBase child in _children)
            {
                child.MouseOut(
                    chartContext
                );
            }
        }
        #endregion

        #region Event Handlers
        private void ChartPanelLayers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (LayerBase layer in e.OldItems)
                {
                    _children.Remove(layer);
                }
            }
            if (e.NewItems != null)
            {
                foreach (LayerBase layer in e.NewItems)
                {
                    layer.OnAttached(_chart);
                    var index = _chart.Layers.IndexOf(layer);
                    _children.Insert(index, layer);
                }
            }
        }
        #endregion

        #region Functions
        #endregion
    }
}
