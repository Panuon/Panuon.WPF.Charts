using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ClusteredColumnSeries
        : SegmentsSeriesBase<ClusteredColumnSeriesSegment>
    {
        #region Fields
        private Dictionary<ClusteredColumnSeriesSegment, List<Point>> _segmentPoints;
        #endregion

        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(ClusteredColumnSeries), new PropertyMetadata(5d));
        #endregion

        #region Width
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(ClusteredColumnSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto), OnRenderPropertyChanged));
        #endregion

        #endregion

        #region Overrides

        #region OnRenderBegin
        protected override void OnRenderBegin(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            _segmentPoints = new Dictionary<ClusteredColumnSeriesSegment, List<Point>>();

            var coordinates = chartContext.Coordinates;
            foreach (var coordinate in coordinates)
            {
                var offsetX = coordinate.Offset;
                var totalWidth = chartContext.CalculateActualWidth(Width);

                var barWidth = CalculateBarWidth(totalWidth);
                var left = offsetX - totalWidth / 2 + barWidth / 2;
                foreach (var segment in Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffsetY(value);

                    if (!_segmentPoints.ContainsKey(segment))
                    {
                        _segmentPoints[segment] = new List<Point>();
                    }
                    _segmentPoints[segment].Add(new Point(left, offsetY));
                    left += (barWidth + Spacing);
                }
            }
        }
        #endregion

        #region OnRendering
        protected override void OnRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        )
        {
            var coordinates = chartContext.Coordinates;

            var totalWidth = chartContext.CalculateActualWidth(Width);
            var barWidth = CalculateBarWidth(totalWidth);

            foreach (var segmentPoint in _segmentPoints)
            {
                var segment = segmentPoint.Key;

                foreach (var point in segmentPoint.Value)
                {
                    drawingContext.DrawRectangle(
                        stroke: segment.Stroke,
                        strokeThickness: segment.StrokeThickness,
                        fill: segment.Fill,
                        startX: point.X - barWidth / 2,
                        startY: chartContext.AreaHeight - (chartContext.AreaHeight - point.Y) * animationProgress,
                        width: barWidth,
                        height: (chartContext.AreaHeight - point.Y) * animationProgress
                    );
                }
            }
        }
        #endregion

        #region OnHighlighting
        protected override void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips)
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinate = layerContext.GetCoordinate(position.X);

                var offsetX = coordinate.Offset;
                var totalWidth = chartContext.CalculateActualWidth(Width);
                var left = offsetX - totalWidth / 2;
                var barWidth = CalculateBarWidth(totalWidth);

                foreach (var segment in Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffsetY(value);
                    drawingContext.DrawEllipse(segment.Fill,
                        2,
                        Brushes.White,
                        5,
                        5,
                        left + barWidth / 2, offsetY);
                    left += (barWidth + Spacing);

                    tooltips.Add(new SeriesTooltip(segment.Fill, segment.Title ?? coordinate.Title, value.ToString()));
                }
            }
        }
        #endregion

        #endregion

        #region Functions
        private double CalculateBarWidth(double totalWidth)
        {
            return (totalWidth - (Segments.Count - 1) * Spacing) / Segments.Count;
        }
        #endregion

    }
}
