﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace Panuon.WPF.Charts
{
    [ContentProperty(nameof(Segments))]
    public abstract class ValueProviderSegmentsSeriesBase<TSegment>
        : ValueProviderSegmentsSeriesBase
        where TSegment : SegmentBase
    {
        #region Ctor
        public ValueProviderSegmentsSeriesBase()
        {
            Segments = new SegmentCollection<TSegment>();
        }
        #endregion

        #region Properties

        #region Arguments
        public SegmentCollection<TSegment> Segments
        {
            get { return (SegmentCollection<TSegment>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register("Segments", typeof(SegmentCollection<TSegment>), typeof(ValueProviderSegmentsSeriesBase<TSegment>), new PropertyMetadata(null, OnSegmentsChanged));
        #endregion

        #endregion

        #region Methods
        public override IEnumerable<SegmentBase> GetSegments()
        {
            return Segments;
        }
        #endregion

        #region Event Handlers
        private static void OnSegmentsChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion
    }

}
