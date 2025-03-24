using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class CrosshairLayer
        : LayerBase
    {
        #region Properties

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(CrosshairLayer), new PropertyMetadata(Brushes.DimGray, OnInvalidRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CrosshairLayer), new PropertyMetadata(1d, OnInvalidRenderPropertyChanged));
        #endregion

        #region LineVisibility
        public CrosshairLineVisibility LineVisibility
        {
            get { return (CrosshairLineVisibility)GetValue(LineVisibilityProperty); }
            set { SetValue(LineVisibilityProperty, value); }
        }

        public static readonly DependencyProperty LineVisibilityProperty =
            DependencyProperty.Register("LineVisibility", typeof(CrosshairLineVisibility), typeof(CrosshairLayer), new PropertyMetadata(CrosshairLineVisibility.Both));
        #endregion

        #endregion

        #region Methods
        protected override void OnMouseIn(IChartContext chartContext)
        {
            InvalidateVisual();
        }

        protected override void OnMouseOut(IChartContext chartContext)
        {
            InvalidateVisual();
        }

        protected override void OnRender(
            IDrawingContext drawingContext,
            IChartContext chartContext)
        {
            var mousePosition = chartContext.GetMousePosition(MouseRelativeTarget.Layer);
            if (!mousePosition.HasValue)
            {
                return;
            }
            var valueOrDefault = mousePosition.GetValueOrDefault();
            var coordinate = chartContext.RetrieveCoordinate(valueOrDefault);
            if (coordinate != null)
            {
                if (LineVisibility == CrosshairLineVisibility.Both || LineVisibility == CrosshairLineVisibility.Vertical)
                {
                    drawingContext.DrawLine(Brushes.Gray, 1.0, new Point(coordinate.Offset, 0.0), new Point(coordinate.Offset, chartContext.CanvasHeight));
                }
                if (LineVisibility == CrosshairLineVisibility.Both || LineVisibility == CrosshairLineVisibility.Horizontal)
                {
                    drawingContext.DrawLine(Brushes.Gray, 1.0, new Point(0.0, valueOrDefault.Y), new Point(chartContext.CanvasWidth, valueOrDefault.Y));
                }
            }
        }
        #endregion
    }
}