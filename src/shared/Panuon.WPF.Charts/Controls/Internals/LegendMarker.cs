using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    class LegendMarker
        : FrameworkElement
    {
        #region Properties

        #region Shape
        public MarkerShape Shape
        {
            get { return (MarkerShape)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(MarkerShape), typeof(LegendMarker), new FrameworkPropertyMetadata(MarkerShape.Circle, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LegendMarker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LegendMarker), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LegendMarker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Overrides

        protected override void OnRender(DrawingContext dc)
        {
            var drawingContext = new WPFDrawingContextImpl(dc);
            
            switch (Shape)
            {
                case MarkerShape.Circle:
                    drawingContext.DrawEllipse(
                    stroke: Stroke,
                        strokeThickness: StrokeThickness,
                    fill: Fill,
                    size: RenderSize,
                        centerPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.Triangle:
                    drawingContext.DrawTriangle(
                    stroke: Stroke,
                       strokeThickness: StrokeThickness,
                    fill: Fill,
                    size: RenderSize,
                       centerPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.Square:
                    drawingContext.DrawRectangle(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                    fill: Fill,
                    size: RenderSize,
                       centerPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.Diamond:
                    drawingContext.DrawDiamond(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                       fill: Fill,
                    size: RenderSize,
                       centerPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.Cross:
                    drawingContext.DrawCross(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                       fill: Fill,
                    size: RenderSize,
                       centerPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.Star:
                    drawingContext.DrawStar(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                       fill: Fill,
                    size: RenderSize,
                       centerPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.ArrowUp:
                    drawingContext.DrawArrowUp(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                       fill: Fill,
                    size: RenderSize,
                       targetPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.ArrowDown:
                    drawingContext.DrawArrowDown(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                       fill: Fill,
                    size: RenderSize,
                       targetPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
                case MarkerShape.Plus:
                    drawingContext.DrawPlus(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                       fill: Fill,
                    size: RenderSize,
                       centerPoint: new Point(RenderSize.Width / 2, RenderSize.Height / 2));
                    break;
            }
            base.OnRender(dc);
        }
        #endregion
    }
}
