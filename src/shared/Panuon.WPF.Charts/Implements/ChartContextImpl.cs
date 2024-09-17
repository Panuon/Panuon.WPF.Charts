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
            ChartPanel = chartPanel;
        }
        #endregion

        #region Properties
        public ChartBase ChartPanel { get; }

        public double AreaWidth => ChartPanel._seriesPanel.RenderSize.Width;

        public double AreaHeight => ChartPanel._seriesPanel.RenderSize.Height;

        public double MinValue => ChartPanel.ActualMinValue;

        public double MaxValue => ChartPanel.ActualMaxValue;

        public IEnumerable<ICoordinate> Coordinates => ChartPanel.Coordinates;

        public IEnumerable<SeriesBase> Series => ChartPanel.GetSeries();
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
