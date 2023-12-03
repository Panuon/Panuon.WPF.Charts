using Panuon.WPF.Charts.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class SeriesPresenter
        : FrameworkElement
    {
        #region Fields
        private SeriesPanel _seriesPanel;

        private ChartPanel _chartPanel;
        #endregion

        #region Ctor
        public SeriesPresenter(SeriesPanel seriesPanel,
            ChartPanel chartPanel)
        {
            _seriesPanel = seriesPanel;
            _chartPanel = chartPanel;
        }
        #endregion

        #region Properties
        public SeriesBase Series
        {
            get => _series;
            set
            {
                if (_series != null)
                {
                    _series.InternalInvalidRender -= Series_InternalInvalidRender;
                }
                if (value != null)
                {
                    value.InternalInvalidRender -= Series_InternalInvalidRender;
                    value.InternalInvalidRender += Series_InternalInvalidRender;
                }
                _series = value;
            }
        }

        private SeriesBase _series;
        #endregion

        #region Overrides
        protected override void OnRender(DrawingContext context)
        {
            if (Series == null
                || _chartPanel.Coordinates == null
                || !_chartPanel.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chartPanel.CreateDrawingContext(context);
            var chartContext = _chartPanel.GetCanvasContext();

            Series.Render(drawingContext,
                chartContext,
                _chartPanel.Coordinates);
        }
        #endregion

        #region Event Handlers
        private void Series_InternalInvalidRender()
        {
            InvalidateVisual();
        }
        #endregion
    }
}
