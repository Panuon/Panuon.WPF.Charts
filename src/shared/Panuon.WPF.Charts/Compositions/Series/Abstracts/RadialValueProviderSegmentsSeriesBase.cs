using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class RadialValueProviderSegmentsSeriesBase
       : RadialSeriesBase, IChartValueProvider
    {
        #region Fields
        private Dictionary<SegmentBase, Point> _segmentPoints;
        #endregion

        #region Properties

        #region Label
        public string Title
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(RadialValueProviderSegmentsSeriesBase), new PropertyMetadata(null));
        #endregion

        #region LabelMemberPath
        public string LabelMemberPath
        {
            get { return (string)GetValue(LabelMemberPathProperty); }
            set { SetValue(LabelMemberPathProperty, Title); }
        }

        public static readonly DependencyProperty LabelMemberPathProperty =
            DependencyProperty.Register("LabelMemberPath", typeof(string), typeof(RadialValueProviderSegmentsSeriesBase), new PropertyMetadata(null));
        #endregion

        #region ValueMemberPath
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(RadialValueProviderSegmentsSeriesBase));
        #endregion

        #endregion

        #region Methods
        public abstract IEnumerable<SegmentBase> GetSegments();
        #endregion
    }
}
