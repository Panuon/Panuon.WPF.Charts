using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace Panuon.WPF.Charts
{
    [ContentProperty(nameof(Segments))]
    public abstract class RadialSegmentsSeriesBase<TSegment>
        : RadialSegmentsSeriesBase
        where TSegment : ValueProviderSegmentBase
    {
        #region Ctor
        public RadialSegmentsSeriesBase()
        {
            Segments = new SegmentCollection<TSegment>();
            Segments.CollectionChanged += Segments_CollectionChanged;
        }
        #endregion

        #region Properties

        #region Segments
        public SegmentCollection<TSegment> Segments
        {
            get { return (SegmentCollection<TSegment>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register("Segments", typeof(SegmentCollection<TSegment>), typeof(RadialSegmentsSeriesBase<TSegment>));
        #endregion

        #endregion

        #region Methods
        public override IEnumerable<SegmentBase> GetSegments()
        {
            return Segments;
        }
        #endregion

        #region Event Handlers
        private void Segments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TSegment segment in e.OldItems)
                {
                    segment.InvalidRender -= Segment_InvalidRender;
                }
            }
            if (e.NewItems != null)
            {
                foreach (TSegment segment in e.NewItems)
                {
                    segment.InvalidRender += Segment_InvalidRender;
                }
            }
        }

        private void Segment_InvalidRender(object sender, System.EventArgs e)
        {
            InvalidateVisual();
        }
        #endregion
    }

}
