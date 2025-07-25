﻿using Panuon.WPF.Charts;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class RadarSeriesSegment
        : SegmentBase
    {
        #region Properties

        #region AxisStroke
        public Brush AxisStroke
        {
            get { return (Brush)GetValue(AxisStrokeProperty); }
            set { SetValue(AxisStrokeProperty, value); }
        }

        public static readonly DependencyProperty AxisStrokeProperty =
            DependencyProperty.Register("AxisStroke", typeof(Brush), typeof(RadarSeriesSegment), new PropertyMetadata(Brushes.LightGray, OnAffectsRenderPropertyChanged));
        #endregion

        #region AxisStrokeThickness
        public double AxisStrokeThickness
        {
            get { return (double)GetValue(AxisStrokeThicknessProperty); }
            set { SetValue(AxisStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty AxisStrokeThicknessProperty =
            DependencyProperty.Register("AxisStrokeThickness", typeof(double), typeof(RadarSeriesSegment), new PropertyMetadata(2d, OnAffectsRenderPropertyChanged));
        #endregion

        #region LabelForeground
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(RadarSeriesSegment), new PropertyMetadata(null, OnAffectsRenderPropertyChanged));
        #endregion

        #region LabelStroke
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(RadarSeriesSegment), new PropertyMetadata(null, OnAffectsRenderPropertyChanged));
        #endregion

        #region LabelStrokeThickness
        public double LabelStrokeThickness
        {
            get { return (double)GetValue(LabelStrokeThicknessProperty); }
            set { SetValue(LabelStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeThicknessProperty =
            DependencyProperty.Register("LabelStrokeThickness", typeof(double), typeof(RadarSeriesSegment), new PropertyMetadata(0.3, OnAffectsRenderPropertyChanged));
        #endregion

        #endregion
    }
}
