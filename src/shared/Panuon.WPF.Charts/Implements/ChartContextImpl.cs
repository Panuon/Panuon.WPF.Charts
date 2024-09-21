using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    internal class ChartContextImpl
        : IChartContext
    {
        #region Fields
        #endregion

        #region Ctor
        internal ChartContextImpl(ChartBase chartPanel)
        {
            Chart = chartPanel;
        }
        #endregion

        #region Properties
        public ChartBase Chart { get; }

        public double AreaWidth => Chart._seriesPanel.RenderSize.Width;

        public double AreaHeight => Chart._seriesPanel.RenderSize.Height;

        public double MinValue => Chart.ActualMinValue;

        public double MaxValue => Chart.ActualMaxValue;

        public IEnumerable<ICoordinate> Coordinates => Chart.Coordinates;

        public IEnumerable<SeriesBase> Series => Chart.GetSeries();
        #endregion

        #region Methods
        public double GetOffsetY(double value)
        {
            var minMaxDelta = MaxValue - MinValue;
            return AreaHeight - AreaHeight * ((value - MinValue) / minMaxDelta);
        }
        #endregion
    }
}
