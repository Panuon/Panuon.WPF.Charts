﻿using System;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class SegmentBase
        : DependencyObject, IChartArgument
    {
        #region Events
        internal event EventHandler InvalidRender;
        #endregion

        #region Properties

        #region Label
        public string Label
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(SegmentBase), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Protected Methods
        protected static void OnAffectsRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var segment = (SegmentBase)d;
            segment.InvalidRender?.Invoke(segment, EventArgs.Empty);
        }
        #endregion
    }
}