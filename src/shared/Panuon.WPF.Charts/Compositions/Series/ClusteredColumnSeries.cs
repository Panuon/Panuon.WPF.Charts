﻿using Panuon.WPF.Charts.Utils;
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

        #region Ctor
        static ClusteredColumnSeries()
        {
            ToggleHighlightLayer.Regist<ClusteredColumnSeries>(OnToggleHighlighting);
        }
        #endregion

        #region Properties

        #region Spacing
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(ClusteredColumnSeries), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ColumnWidth
        public GridLength ColumnWidth
        {
            get { return (GridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(GridLength), typeof(ClusteredColumnSeries), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Radius
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(ClusteredColumnSeries), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
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
            var clusterWidth = GridLengthUtil.GetActualValue(ColumnWidth, deltaX);
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
            var clusterWidth = GridLengthUtil.GetActualValue(ColumnWidth, deltaX);
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
                            height: chartContext.AreaHeight,
                            radiusX: Radius,
                            radiusY: Radius
                        );
                    }

                    drawingContext.DrawRectangle(
                        stroke: segment.Stroke,
                        strokeThickness: segment.StrokeThickness,
                        fill: segment.Fill,
                        startX: point.X - columnWidth / 2,
                        startY: chartContext.AreaHeight - (chartContext.AreaHeight - point.Y) * animationProgress,
                        width: columnWidth,
                        height: (chartContext.AreaHeight - point.Y) * animationProgress,
                        radiusX: Radius,
                        radiusY: Radius
                    );
                }
            }
        }
        #endregion

        #region OnHighlighting
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries(
            IChartContext chartContext,
            ILayerContext layerContext
        )
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinate = layerContext.GetCoordinate(position.X);

                foreach (var segment in Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffsetY(value);

                    yield return new SeriesLegendEntry(segment.Fill, segment.Title ?? coordinate.Title, value.ToString());
                }
            }
        }
        #endregion

        #endregion

        #region Event Handlers
        public static void OnToggleHighlighting(
            ToggleHighlightLayer layer,
            ClusteredColumnSeries series,
            IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            IDictionary<int, double> coordinatesProgress
        )
        {
            foreach (var coordinateProgress in coordinatesProgress)
            {
                var index = coordinateProgress.Key;
                var coordinate = chartContext.Coordinates.FirstOrDefault(c => c.Index == index);
                var progress = coordinateProgress.Value;

                if (progress == 0)
                {
                    continue;
                }

                var offsetX = coordinate.Offset;

                var deltaX = chartContext.AreaWidth / chartContext.Coordinates.Count();
                var clusterWidth = GridLengthUtil.GetActualValue(series.ColumnWidth, deltaX);
                var columnWidth = series.CalculateBarWidth(clusterWidth);

                var left = offsetX - clusterWidth / 2;
                foreach (var segment in series.Segments)
                {
                    var value = coordinate.GetValue(segment);
                    var offsetY = chartContext.GetOffsetY(value);
                    drawingContext.DrawEllipse(
                        stroke: segment.Fill,
                        strokeThickness: layer.HighlightToggleStrokeThickness,
                        fill: layer.HighlightToggleFill,
                        radiusX: progress * layer.HighlightToggleRadius,
                        radiusY: progress * layer.HighlightToggleRadius,
                        left + columnWidth / 2,
                        offsetY
                    );
                    left += (columnWidth + series.Spacing);
                }
            }
        }
        #endregion

        #region Functions
        private double CalculateBarWidth(double totalWidth)
        {
            return Math.Max(0, (totalWidth - (Segments.Count - 1) * Spacing) / Segments.Count);
        }

        #endregion

    }
}
