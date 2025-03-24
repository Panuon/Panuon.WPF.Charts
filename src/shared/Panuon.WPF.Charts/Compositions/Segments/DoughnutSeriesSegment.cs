using System;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class DoughnutSeriesSegment
        : SegmentBase
    {
        #region Properties

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(DoughnutSeriesSegment), new PropertyMetadata(Brushes.Black, OnAffectsRenderPropertyChanged));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(DoughnutSeriesSegment), new PropertyMetadata(null, OnAffectsRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DoughnutSeriesSegment), new PropertyMetadata(1d, OnAffectsRenderPropertyChanged));
        #endregion

        #endregion
    }
}
