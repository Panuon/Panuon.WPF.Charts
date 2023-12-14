using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public interface ICoordinate
    {
        string Title { get; }

        int Index { get; }

        double Offset { get; }

        double GetValue(IChartValueProvider seriesOrSegment);
    }
}
