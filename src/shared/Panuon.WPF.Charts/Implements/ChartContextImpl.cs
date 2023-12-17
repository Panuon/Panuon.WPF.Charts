using Panuon.WPF.Charts.Controls.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Panuon.WPF.Charts
{
    internal class ChartContextImpl
        : IChartContext
    {
        #region Fields
        #endregion

        #region Ctor
        internal ChartContextImpl(ChartPanel chartPanel)
        {
            ChartPanel = chartPanel;
        }
        #endregion

        #region Properties
        public ChartPanel ChartPanel { get; }

        public double AreaWidth => ChartPanel._seriesPanel.RenderSize.Width;

        public double AreaHeight => ChartPanel._seriesPanel.RenderSize.Height;

        public double MinValue => ChartPanel.MinValue;

        public double MaxValue => ChartPanel.MaxValue;

        public IEnumerable<ICoordinate> Coordinates => ChartPanel.Coordinates;

        public IEnumerable<SeriesBase> Series => ChartPanel.Series;
        #endregion

        #region Methods
        public double GetOffset(double value)
        {
            var minMaxDelta = MaxValue - MinValue;
            return AreaHeight - AreaHeight * ((value - MinValue) / minMaxDelta);
        }

        public double CalculateWidth(GridLength width)
        {
            var deltaX = AreaWidth / ChartPanel.Coordinates.Count();

            if (width.IsAbsolute)
            {
                return width.Value;
            }
            else if (width.IsStar)
            {
                return (deltaX * width.Value);
            }

            else
            {
                return deltaX / 2;
            }
        }
        #endregion
    }
}
