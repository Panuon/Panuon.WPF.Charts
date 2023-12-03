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
                     
                }
                if (value != null)
                {
                    value.InternalAddChild += Value_InternalAddChild;
                    value.InternalRemoveChild += Value_InternalRemoveChild;
                    value.InternalInvalidRender += Value_InternalInvalidRender;
                }
                _layer = value;
            }
        }

        private void Value_InternalInvalidRender()
        {
            InvalidateVisual();
        }

        private void Value_InternalRemoveChild(UIElement child)
        {
            _children.Remove(child);
        }

        private void Value_InternalAddChild(UIElement child)
        {
            _children.Add(child);
        }

        private LayerBase _layer;
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
            var canvasContext = _chartPanel.GetCanvasContext();
            var layerContext = _chartPanel.CreateLayerContext();

            Layer.Render(drawingContext,
                canvasContext,
                layerContext);
        }
        #endregion

        #region Methods
        public void MouseIn()
        {
            var canvasContext = _chartPanel.GetCanvasContext();
            var layerContext = _chartPanel.CreateLayerContext();

            Layer.MouseIn(canvasContext, layerContext);
        }

        public void MouseOut()
        {
            var canvasContext = _chartPanel.GetCanvasContext();
            var layerContext = _chartPanel.CreateLayerContext();

            Layer.MouseOut(canvasContext, layerContext);
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
