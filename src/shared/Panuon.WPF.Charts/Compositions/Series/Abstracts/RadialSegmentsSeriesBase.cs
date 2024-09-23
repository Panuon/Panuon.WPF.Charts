using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class RadialSegmentsSeriesBase
       : RadialSeriesBase
    {
        #region Fields
        private Dictionary<SegmentBase, Point> _segmentPoints;
        #endregion

        #region Properties

        #region ValuesMemberPath
        public string ValuesMemberPath
        {
            get { return (string)GetValue(ValuesMemberPathProperty); }
            set { SetValue(ValuesMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValuesMemberPathProperty =
            DependencyProperty.Register("ValuesMemberPath", typeof(string), typeof(RadialSegmentsSeriesBase));
        #endregion

        #endregion

        #region Methods
        public abstract IEnumerable<SegmentBase> GetSegments();
        #endregion
    }
}
