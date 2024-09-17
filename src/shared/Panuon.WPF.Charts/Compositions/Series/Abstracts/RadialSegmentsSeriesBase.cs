using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class RadialSegmentsSeriesBase
       : RadialSeriesBase, IChartSegmentsSeries
    {
        #region Fields
        private Dictionary<SegmentBase, Point> _segmentPoints;
        #endregion

        #region Methods
        public abstract IEnumerable<SegmentBase> GetSegments();
        #endregion
    }
}
