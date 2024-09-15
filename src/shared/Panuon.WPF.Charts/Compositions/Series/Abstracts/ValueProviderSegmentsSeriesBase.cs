using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class ValueProviderSegmentsSeriesBase
       : ValueProviderSeriesBase
    {
        #region Fields
        private Dictionary<SegmentBase, Point> _segmentPoints;
        #endregion

        #region Methods
        public abstract IEnumerable<SegmentBase> GetSegments();
        #endregion
    }
}
