using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface ICartesianChartContext
        : IChartContext
    {
        new CartesianChart Chart { get; }

        double MinValue { get; }

        double MaxValue { get; }

        IEnumerable<ICartesianCoordinate> Coordinates { get; }

        new ICartesianCoordinate RetrieveCoordinate(Point position);

        double GetOffsetY(double value);
    }
}