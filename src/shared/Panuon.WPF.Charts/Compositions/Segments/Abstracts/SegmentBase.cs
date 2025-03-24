using System;
using System.Windows;
using System.Windows.Media;

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
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(SegmentBase), new PropertyMetadata(null));
        #endregion

        #region LabelForeground
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(SegmentBase), new PropertyMetadata(null, OnAffectsRenderPropertyChanged));
        #endregion

        #region ShowLabels
        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register("ShowLabels", typeof(bool), typeof(SegmentBase), new PropertyMetadata(false));
        #endregion

        #region InvertForeground
        public Brush InvertForeground
        {
            get { return (Brush)GetValue(InvertForegroundProperty); }
            set { SetValue(InvertForegroundProperty, value); }
        }

        public static readonly DependencyProperty InvertForegroundProperty =
            DependencyProperty.Register("InvertForeground", typeof(Brush), typeof(SegmentBase), new PropertyMetadata(null, OnAffectsRenderPropertyChanged));
        #endregion

        #region LabelStroke
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(SegmentBase), new PropertyMetadata(null, OnAffectsRenderPropertyChanged));
        #endregion

        #region LabelStrokeThickness
        public double? LabelStrokeThickness
        {
            get { return (double?)GetValue(LabelStrokeThicknessProperty); }
            set { SetValue(LabelStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeThicknessProperty =
            DependencyProperty.Register("LabelStrokeThickness", typeof(double?), typeof(SegmentBase), new PropertyMetadata(0.3, OnAffectsRenderPropertyChanged));
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