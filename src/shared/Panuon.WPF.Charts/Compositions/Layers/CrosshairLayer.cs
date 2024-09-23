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
            if (chartContext.GetMousePosition(MouseRelativeTarget.Layer) is Point position)
            {
                var coordinate = chartContext.RetrieveCoordinate(position);
                if (coordinate != null)
                {
                    if (LineVisibility == CrosshairLineVisibility.Both
                        || LineVisibility == CrosshairLineVisibility.YAxis)
                    {
                        //vertical line
                        drawingContext.DrawLine(
                            stroke: Brushes.Gray,
                            strokeThickness: 1,
                            startX: coordinate.OffsetX,
                            startY: 0,
                            endX: coordinate.OffsetX,
                            endY: chartContext.AreaHeight
                        );
                    }
                    if (LineVisibility == CrosshairLineVisibility.Both
                        || LineVisibility == CrosshairLineVisibility.XAxis)
                    {
                        drawingContext.DrawLine(
                            stroke: Brushes.Gray,
                            strokeThickness: 1, 
                            startX: 0, 
                            startY: position.Y, 
                            endX: chartContext.AreaWidth,
                            endY: position.Y
                        );
                    }
                }
            }
        }
        #endregion
    }
}