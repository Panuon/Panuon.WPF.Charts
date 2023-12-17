using Panuon.WPF.Charts.Implements;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        private ChartPanel _chartPanel;

        private UIElementCollection _children;

        private readonly List<LayerPresenter> _layerPresenters =
            new List<LayerPresenter>();

        private IList<ICoordinate> _coordinates;
        #endregion

        #region Ctor
        internal LayersPanel(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
            _chartPanel.Layers.CollectionChanged += ChartPanelLayers_CollectionChanged;

            _children = new UIElementCollection(this, this);
        }
        #endregion

        #region Overrides
        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (LayerPresenter child in _children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (LayerPresenter child in _children)
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
            foreach (LayerPresenter child in _children)
            {
                child.MouseIn();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            foreach (LayerPresenter child in _children)
            {
                child.MouseOut();
            }
        }
        #endregion

        #region Methods
        public new void InvalidateVisual()
        {
            foreach (LayerPresenter child in _children)
            {
                child.InvalidateVisual();
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
                    var presenter = _layerPresenters.FirstOrDefault(x => x.Layer == layer);
                    if (presenter != null)
                    {
                        presenter.Layer = null; //Clear

                        _children.Remove(presenter);
                        _layerPresenters.Remove(presenter);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (LayerBase layer in e.NewItems)
                {
                    var presenter = _layerPresenters.FirstOrDefault(x => x.Layer == layer);
                    if (presenter == null)
                    {
                        presenter = new LayerPresenter(this, _chartPanel)
                        {
                            Layer = layer
                        };
                        _children.Add(presenter);
                        _layerPresenters.Add(presenter);
                    }
                }
            }
        }
        #endregion

        #region Functions
        #endregion
    }
}
