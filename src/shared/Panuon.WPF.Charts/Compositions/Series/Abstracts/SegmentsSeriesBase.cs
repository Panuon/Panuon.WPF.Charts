using Panuon.WPF.Chart;
using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public abstract class SegmentsSeriesBase
        : SeriesBase
    {
        internal SegmentsSeriesBase() { }

        public abstract IEnumerable<ValueProviderSegmentBase> GetSegments();
    }
}
