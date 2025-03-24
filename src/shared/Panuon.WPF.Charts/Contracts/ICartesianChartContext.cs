using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface ICartesianChartContext
        : IChartContext
    {
        bool SwapXYAxes { get; }

        double MinValue { get; }

        double MaxValue { get; }

        double CurrentOffset { get; }

        double SliceWidth { get; }

        double SliceHeight { get; }

        IEnumerable<ICartesianCoordinate> Coordinates { get; }

        new ICartesianCoordinate RetrieveCoordinate(Point position);

        double GetOffsetY(double value);
    }
}