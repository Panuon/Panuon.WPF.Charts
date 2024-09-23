using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface IRadialChartContext
        : IChartContext
    {
        new RadialChart Chart { get; }

        IEnumerable<IRadialCoordinate> Coordinates { get; }

        new IRadialCoordinate RetrieveCoordinate(Point position);
    }
}
