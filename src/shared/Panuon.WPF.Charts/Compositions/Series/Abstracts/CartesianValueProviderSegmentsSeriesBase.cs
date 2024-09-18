using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public abstract class CartesianValueProviderSegmentsSeriesBase
        : CartesianValueProviderSeriesBase, IChartSegmentsSeries
    {
        internal CartesianValueProviderSegmentsSeriesBase() { }

        public abstract IEnumerable<SegmentBase> GetSegments();
    }
}