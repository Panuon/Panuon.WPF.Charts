using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface IChartContext
    {
        double AreaWidth { get; }

        double AreaHeight { get; }

        double MinValue { get; }

        double MaxValue { get; }

        IEnumerable<ICoordinate> Coordinates { get; }

        IEnumerable<SeriesBase> Series { get; }

        double GetOffset(double value);

        double CalculateWidth(GridLength width);
    }
}
