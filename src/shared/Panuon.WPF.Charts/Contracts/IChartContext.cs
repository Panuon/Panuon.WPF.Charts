using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public interface IChartContext
    {
        ChartBase Chart { get; }

        double AreaWidth { get; }

        double AreaHeight { get; }

        IEnumerable<SeriesBase> Series { get; }
    }
}
