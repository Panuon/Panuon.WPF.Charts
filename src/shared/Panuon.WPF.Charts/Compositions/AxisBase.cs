using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public abstract class AxisBase
        : ChartElementBase
    {
        #region Fields
        protected CartesianChart _chart;

        #endregion

        #region Ctor
        public AxisBase()
        {
            
        }
        #endregion

        #region RoutedEvent

        #endregion

        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(AxisBase), new PropertyMetadata(5d));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(AxisBase), new PropertyMetadata(1d));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(AxisBase), new PropertyMetadata(Brushes.Black));
        #endregion

        #region LabelMaxWidth
        public double LabelMaxWidth
        {
            get { return (double)GetValue(LabelMaxWidthProperty); }
            set { SetValue(LabelMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty LabelMaxWidthProperty =
            DependencyProperty.Register("LabelMaxWidth", typeof(double), typeof(AxisBase), new PropertyMetadata(100d));
        #endregion

        #region LabelMaxLineCount
        public int LabelMaxLineCount
        {
            get { return (int)GetValue(LabelMaxLineCountProperty); }
            set { SetValue(LabelMaxLineCountProperty, value); }
        }

        public static readonly DependencyProperty LabelMaxLineCountProperty =
            DependencyProperty.Register("LabelMaxLineCount", typeof(int), typeof(AxisBase), new PropertyMetadata(1));
        #endregion

        #region TicksSize
        public double TicksSize
        {
            get { return (double)GetValue(TicksSizeProperty); }
            set { SetValue(TicksSizeProperty, value); }
        }

        public static readonly DependencyProperty TicksSizeProperty =
            DependencyProperty.Register("TicksSize", typeof(double), typeof(AxisBase), new PropertyMetadata(3d));
        #endregion

        #region TicksBrush
        public Brush TicksBrush
        {
            get { return (Brush)GetValue(TicksBrushProperty); }
            set { SetValue(TicksBrushProperty, value); }
        }

        public static readonly DependencyProperty TicksBrushProperty =
            DependencyProperty.Register("TicksBrush", typeof(Brush), typeof(AxisBase), new PropertyMetadata(Brushes.Black));
        #endregion

        #endregion

        #region Internal Methods
        internal void OnAttached(CartesianChart chart)
        {
            _chart = chart;
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Functions

        #endregion

    }
}
