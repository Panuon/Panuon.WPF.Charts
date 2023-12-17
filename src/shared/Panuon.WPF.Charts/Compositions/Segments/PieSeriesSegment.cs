using Panuon.WPF.Chart;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class PieSeriesSegment
        : SegmentBase
    {
        #region Properties

        #region LabelForeground
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(PieSeriesSegment), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region LabelStroke
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(PieSeriesSegment), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region LabelStrokeThickness
        public double LabelStrokeThickness
        {
            get { return (double)GetValue(LabelStrokeThicknessProperty); }
            set { SetValue(LabelStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeThicknessProperty =
            DependencyProperty.Register("LabelStrokeThickness", typeof(double), typeof(PieSeriesSegment), new PropertyMetadata(0.3, OnRenderPropertyChanged));
        #endregion

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(PieSeriesSegment), new PropertyMetadata(Brushes.Black, OnRenderPropertyChanged));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(PieSeriesSegment), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(PieSeriesSegment), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #endregion
    }
}
