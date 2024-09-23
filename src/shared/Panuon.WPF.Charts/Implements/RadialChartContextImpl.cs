using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    internal class RadialChartContextImpl
        : IRadialChartContext
    {
        #region Fields
        #endregion

        #region Ctor
        internal RadialChartContextImpl(RadialChart chart)
        {
            Chart = chart;
        }
        #endregion

        #region Properties
        public RadialChart Chart { get; }

        ChartBase IChartContext.Chart => Chart;

        public double AreaWidth => Chart._seriesPanel.RenderSize.Width;

        public double AreaHeight => Chart._seriesPanel.RenderSize.Height;

        public IEnumerable<SeriesBase> Series => Chart.GetSeries();

        #endregion
    }
}
