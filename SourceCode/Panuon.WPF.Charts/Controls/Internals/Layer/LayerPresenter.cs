using Panuon.WPF.Charts.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class LayerPresenter
        : FrameworkElement
    {
        #region Fields
        private LayersPanel _layersPanel;

        private ChartPanel _chartPanel;

        private UIElementCollection _children;
        #endregion

        #region Ctor
        public LayerPresenter(LayersPanel layersPanel,
            ChartPanel chartPanel)
        {
            _layersPanel = layersPanel;
            _chartPanel = chartPanel;

            _children = new UIElementCollection(this, this);
        }
        #endregion

        #region Properties

        public LayerBase Layer
        {
            get => _layer;
            set
            {
                if (_layer != null)
                {
                    _layer.InternalAddChild -= Layer_InternalAddChild;
                    _layer.InternalRemoveChild -= Layer_InternalRemoveChild;
                    _layer.InternalInvalidRender -= Layer_InternalInvalidRender;
                }
                if (value != null)
                {
                    value.InternalAddChild -= Layer_InternalAddChild;
                    value.InternalAddChild += Layer_InternalAddChild;

                    value.InternalRemoveChild -= Layer_InternalRemoveChild;
                    value.InternalRemoveChild += Layer_InternalRemoveChild;

                    value.InternalInvalidRender -= Layer_InternalInvalidRender;
                    value.InternalInvalidRender += Layer_InternalInvalidRender;
                }
                _layer = value;
            }
        }
        private LayerBase _layer;

        private void Layer_InternalInvalidRender()
        {
            InvalidateVisual();
        }

        private void Layer_InternalRemoveChild(UIElement child)
        {
            _children.Remove(child);
        }

        private void Layer_InternalAddChild(UIElement child)
        {
            _children.Add(child);
        }

        #endregion

        #region Overrides
        protected override void OnRender(DrawingContext context)
        {
            if (Layer == null
                || !_chartPanel.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chartPanel.CreateDrawingContext(context);
            var chartContext = _chartPanel.GetCanvasContext();
            var layerContext = _chartPanel.CreateLayerContext();

            Layer.Render(drawingContext,
                chartContext,
                layerContext);
        }
        #endregion

        #region Methods
        public void MouseIn()
        {
            var chartContext = _chartPanel.GetCanvasContext();
            var layerContext = _chartPanel.CreateLayerContext();

            Layer.MouseIn(chartContext, layerContext);
        }

        public void MouseOut()
        {
            var chartContext = _chartPanel.GetCanvasContext();
            var layerContext = _chartPanel.CreateLayerContext();

            Layer.MouseOut(chartContext, layerContext);
        }
        #endregion

        #region Functions
        //private IDrawingContext CreateDrawingContext(DrawingContext drawingContext)
        //{
        //    return new WPFDrawingContextImpl(drawingContext,
        //        ActualWidth,
        //        ActualHeight);
        //}

        private void CheckMinMaxValue(double minValue,
            double maxValue,
            out int resultMin,
            out int resultMax)
        {
            var min = (int)Math.Floor(minValue);
            var max = (int)Math.Ceiling(maxValue);

            var digit = Math.Max(1, max.ToString().Length - 1);
            var baseValue = Math.Pow(10d, digit);

            resultMin = (int)Math.Floor(min / baseValue) * (int)baseValue;
            resultMax = (int)Math.Ceiling(max / baseValue) * (int)baseValue;

        }
        #endregion
    }
}
