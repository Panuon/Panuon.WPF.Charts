using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public interface ICartesianChartContext
        : IChartContext
    {
        new CartesianChart Chart { get; }

        double MinValue { get; }

        double MaxValue { get; }

        IEnumerable<ICoordinate> Coordinates { get; }

        double GetOffsetY(double value);
    }
}