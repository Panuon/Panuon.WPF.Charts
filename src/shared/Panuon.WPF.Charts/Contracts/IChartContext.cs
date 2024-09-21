using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public interface IChartContext
    {
        ChartBase Chart { get; }

        double AreaWidth { get; }

        double AreaHeight { get; }

        double MinValue { get; }

        double MaxValue { get; }

        IEnumerable<ICoordinate> Coordinates { get; }

        IEnumerable<SeriesBase> Series { get; }

        double GetOffsetY(double value);
    }
}
