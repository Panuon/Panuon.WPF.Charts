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

        public SeriesBase Series { get; set; }
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
            var canvasContext = _chartPanel.GetCanvasContext();

            Series.Render(drawingContext,
                canvasContext,
                _chartPanel.Coordinates);
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
