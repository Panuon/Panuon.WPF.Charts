using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ClusteredColumnSeries
        : CartesianSegmentsSeriesBase<ClusteredColumnSeriesSegment>
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

            var deltaX = chartContext.AreaWidth / chartContext.Coordinates.Count();
            var clusterWidth = GridLengthUtil.GetActualValue(Width, deltaX);
            var columnWidth = CalculateBarWidth(clusterWidth);

            var coordinates = chartContext.Coordinates;
            foreach (var coordinate in coordinates)
            {
                var offsetX = coordinate.Offset;

                var left = offsetX - clusterWidth / 2 + columnWidth / 2;
                foreach (var segment in Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffsetY(value);

                    if (!_segmentPoints.ContainsKey(segment))
                    {
                        _segmentPoints[segment] = new List<Point>();
                    }
                    _segmentPoints[segment].Add(new Point(left, offsetY));
                    left += (columnWidth + Spacing);
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

            var deltaX = chartContext.AreaWidth / chartContext.Coordinates.Count();
            var clusterWidth = GridLengthUtil.GetActualValue(Width, deltaX);
            var columnWidth = CalculateBarWidth(clusterWidth);

            foreach (var segmentPoint in _segmentPoints)
            {
                var segment = segmentPoint.Key;

                foreach (var point in segmentPoint.Value)
                {
                    if (segment.BackgroundFill != null)
                    {
                        drawingContext.DrawRectangle(
                            stroke: null,
                            strokeThickness: 0,
                            fill: segment.BackgroundFill,
                            startX: point.X - columnWidth / 2,
                            startY: 0,
                            width: columnWidth,
                            height: chartContext.AreaHeight
                        );
                    }

                    drawingContext.DrawRectangle(
                        stroke: segment.Stroke,
                        strokeThickness: segment.StrokeThickness,
                        fill: segment.Fill,
                        startX: point.X - columnWidth / 2,
                        startY: chartContext.AreaHeight - (chartContext.AreaHeight - point.Y) * animationProgress,
                        width: columnWidth,
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

                var deltaX = chartContext.AreaWidth / chartContext.Coordinates.Count();
                var clusterWidth = GridLengthUtil.GetActualValue(Width, deltaX);
                var columnWidth = CalculateBarWidth(clusterWidth);

                var left = offsetX - clusterWidth / 2;

                foreach (var segment in Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffsetY(value);
                    drawingContext.DrawEllipse(segment.Fill,
                        2,
                        Brushes.White,
                        5,
                        5,
                        left + columnWidth / 2, offsetY);
                    left += (columnWidth + Spacing);

                    tooltips.Add(new SeriesTooltip(segment.Fill, segment.Title ?? coordinate.Title, value.ToString()));
                }
            }
        }
        #endregion

        #endregion

        #region Functions
        private double CalculateBarWidth(double totalWidth)
        {
            return Math.Max(0, (totalWidth - (Segments.Count - 1) * Spacing) / Segments.Count);
        }
        #endregion

    }
}
