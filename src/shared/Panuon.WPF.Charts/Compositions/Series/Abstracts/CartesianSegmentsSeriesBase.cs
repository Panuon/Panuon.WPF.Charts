using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public abstract class CartesianSegmentsSeriesBase
        : CartesianSeriesBase, IChartSegmentsSeries
    {
        internal CartesianSegmentsSeriesBase() { }

        public abstract IEnumerable<SegmentBase> GetSegments();
    }
}