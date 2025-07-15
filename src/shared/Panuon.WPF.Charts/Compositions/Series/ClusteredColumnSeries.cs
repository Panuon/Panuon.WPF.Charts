using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public class ClusteredColumnSeries
        : CartesianSegmentsSeriesBase<ClusteredColumnSeriesSegment>
    {
        #region Fields
        private Dictionary<ClusteredColumnSeriesSegment, Dictionary<ICoordinate, Point?>> _segmentPoints;
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
            ICartesianChartContext chartContext
        )
        {
            var coordinates = chartContext.Coordinates;

            var delta = chartContext.SwapXYAxes
                ? chartContext.CanvasHeight / chartContext.Coordinates.Count()
                : chartContext.CanvasWidth / chartContext.Coordinates.Count();
            var clusterSize = GridLengthUtil.GetActualValue(ColumnWidth, delta);
            var columnSize = CalculateBarWidth(clusterSize);

            _segmentPoints = new Dictionary<ClusteredColumnSeriesSegment, Dictionary<ICoordinate, Point?>>();
            var coordinateEnumerator = coordinates.GetEnumerator();
            coordinateEnumerator.MoveNext();
            ICoordinate lastCoordinate = null;

            for (int i = 0; i < coordinates.Count(); i++)
            {
                var coordinate = coordinateEnumerator.Current;
                ICoordinate nextCoordinate = null;

                if (coordinateEnumerator.MoveNext())
                {
                    nextCoordinate = coordinateEnumerator.Current;
                }

                for(var j = 0; j < Segments.Count; j ++)
                {
                    var segment = Segments[j];
                    var value = coordinate.GetValue(segment);

                    double? offsetX = 0d;
                    double? offsetY = 0d;

                    var offset = coordinate.Offset - clusterSize / 2 + columnSize / 2 + j * (columnSize + Spacing);

                    if (coordinate.Offset < chartContext.CurrentOffset
                        && nextCoordinate != null
                        && nextCoordinate.Offset < chartContext.CurrentOffset)
                    {
                        continue;
                    }
                    else if (coordinate.Offset > chartContext.CurrentOffset + chartContext.SliceWidth
                        && lastCoordinate != null
                        && lastCoordinate.Offset > chartContext.CurrentOffset)
                    {
                        break;
                    }

                    if (!chartContext.SwapXYAxes)
                    {
                        offsetX = offset;
                        offsetY = value == null ? (double?)null : chartContext.GetOffsetY((decimal)value);
                    }
                    else
                    {
                        offsetX = value == null ? (double?)null : chartContext.GetOffsetY((decimal)value);
                        offsetY = offset;
                    }

                    if (!_segmentPoints.ContainsKey(segment))
                    {
                        _segmentPoints[segment] = new Dictionary<ICoordinate, Point?>();
                    }

                    _segmentPoints[segment].Add(coordinate, (offsetX == null || offsetY == null) ? (Point?)null : new Point((double)offsetX, (double)offsetY));

                    if (!chartContext.SwapXYAxes)
                    {
                        offsetX += (columnSize + Spacing);
                    }
                    else
                    {
                        offsetY += (columnSize + Spacing);
                    }
                }
            }
        }
        #endregion

        #region OnRendering
        protected override void OnRendering(
            IDrawingContext drawingContext,
            ICartesianChartContext chartContext,
            double animationProgress
        )
        {
            var coordinates = chartContext.Coordinates;

            var delta = chartContext.SwapXYAxes
                ? chartContext.CanvasHeight / chartContext.Coordinates.Count()
                : chartContext.CanvasWidth / chartContext.Coordinates.Count();
            var clusterWidth = GridLengthUtil.GetActualValue(ColumnWidth, delta);
            var columnSize = CalculateBarWidth(clusterWidth);

            foreach (var segmentPoint in _segmentPoints)
            {
                var segment = segmentPoint.Key;

                foreach (var coordinatePoint in segmentPoint.Value)
                {
                    var coordinate = coordinatePoint.Key;
                    if (coordinatePoint.Value != null)
                    {
                        var offsetX = (double)coordinatePoint.Value?.X;
                        var offsetY = (double)coordinatePoint.Value?.Y;

                        if (!chartContext.SwapXYAxes)
                        {
                            if (segment.BackgroundFill != null)
                            {
                                drawingContext.DrawRectangle(
                                    stroke: null,
                                    strokeThickness: 0,
                                    fill: segment.BackgroundFill,
                                    centerPoint: new Point(offsetX, chartContext.CanvasHeight / 2),
                                    size: new Size(columnSize, chartContext.CanvasHeight),
                                    radius: new Size(Radius, Radius));
                            }

                            var startY = chartContext.CanvasHeight - (chartContext.CanvasHeight - offsetY) * animationProgress;
                            var height = (chartContext.CanvasHeight - offsetY) * animationProgress;

                            drawingContext.DrawRectangle(
                                stroke: segment.Stroke,
                                strokeThickness: segment.StrokeThickness,
                                fill: segment.Fill,
                                centerPoint: new Point(offsetX, startY + height / 2),
                                size: new Size(columnSize, height),
                                radius: new Size(Radius, Radius));

                            if (ShowValueLabels)
                            {
                                var label = CreateFormattedText(coordinate.GetValue(segment).ToString(), Foreground);

                                var fill = segment.LabelForeground ?? Foreground;
                                var invertForeground = segment.InvertForeground ?? InvertForeground;
                                var labelOffsetY = 0d;
                                if (invertForeground != null)
                                {
                                    switch (ValueLabelPlacement)
                                    {
                                        case SeriesLabelPlacement.Top:
                                            if (offsetY < label.Height / 2)
                                            {
                                                fill = invertForeground;
                                            }
                                            labelOffsetY = 0;
                                            break;
                                        case SeriesLabelPlacement.Above:
                                            labelOffsetY = Math.Max(0, startY - label.Height);
                                            if (startY < label.Height / 2)
                                            {
                                                fill = invertForeground;
                                            }
                                            break;
                                        case SeriesLabelPlacement.Bottom:
                                            if (height < label.Height / 2)
                                            {
                                                fill = invertForeground;
                                            }
                                            labelOffsetY = chartContext.CanvasHeight - label.Height;
                                            break;
                                    }
                                }
                                drawingContext.DrawText(
                                    label,
                                    startPoint: new Point(offsetX, labelOffsetY),
                                    fill: fill,
                                    stroke: segment.LabelStroke ?? ValueLabelStroke,
                                    strokeThickness: segment.LabelStrokeThickness ?? ValueLabelStrokeThickness);
                            }
                        }
                        else
                        {
                            if (segment.BackgroundFill != null)
                            {
                                drawingContext.DrawRectangle(
                                    stroke: null,
                                    strokeThickness: 0,
                                    fill: segment.BackgroundFill,
                                    centerPoint: new Point(chartContext.CanvasWidth / 2, offsetY),
                                    size: new Size(chartContext.CanvasWidth, columnSize),
                                    radius: new Size(Radius, Radius));
                            }

                            var startY = offsetY - columnSize / 2;
                            var width = offsetX * animationProgress;

                            drawingContext.DrawRectangle(
                                stroke: segment.Stroke,
                                strokeThickness: segment.StrokeThickness,
                                fill: segment.Fill,
                                centerPoint: new Point(width / 2, startY + columnSize / 2),
                                size: new Size(width, columnSize),
                                radius: new Size(Radius, Radius));

                            if (ShowValueLabels)
                            {
                                var label = CreateFormattedText(coordinate.GetValue(segment).ToString(), segment.LabelForeground ?? Foreground);

                                var fill = segment.LabelForeground ?? Foreground;
                                var labelOffsetX = 0d;
                                if (InvertForeground != null)
                                {
                                    switch (ValueLabelPlacement)
                                    {
                                        case SeriesLabelPlacement.Top:
                                            if (chartContext.CanvasWidth - width < label.Width / 2)
                                            {
                                                fill = segment.InvertForeground ?? InvertForeground;
                                            }
                                            labelOffsetX = chartContext.CanvasWidth - label.Width;
                                            break;
                                        case SeriesLabelPlacement.Above:
                                            labelOffsetX = Math.Max(0, width + label.Width);
                                            if (chartContext.CanvasWidth - width < label.Width / 2)
                                            {
                                                fill = segment.InvertForeground ?? InvertForeground;
                                            }
                                            break;
                                        case SeriesLabelPlacement.Bottom:
                                            if (width < label.Width / 2)
                                            {
                                                fill = segment.InvertForeground ?? InvertForeground;
                                            }
                                            labelOffsetX = 0d;
                                            break;
                                    }
                                }
                                drawingContext.DrawText(
                                    label,
                                    startPoint: new Point(labelOffsetX, offsetY - label.Height / 2),
                                    fill: fill,
                                    stroke: segment.LabelStroke ?? ValueLabelStroke,
                                    strokeThickness: segment.LabelStrokeThickness ?? ValueLabelStrokeThickness);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region OnHighlighting
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries()
        {
            foreach (var segment in Segments)
            {
                yield return new SeriesLegendEntry(
                    segment.Title,
                    markerShape: MarkerShape.Circle,
                    markerStroke: segment.Stroke,
                    markerStrokeThickness: segment.StrokeThickness,
                    markerFill: segment.Fill);
            }
        }
        #endregion

        #endregion

        #region Event Handlers
        public static void OnToggleHighlighting(
            ToggleHighlightLayer layer,
            ClusteredColumnSeries series,
            IDrawingContext drawingContext,
            ICartesianChartContext chartContext,
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

                var deltaX = chartContext.CanvasWidth / chartContext.Coordinates.Count();
                var clusterWidth = GridLengthUtil.GetActualValue(series.ColumnWidth, deltaX);
                var columnWidth = series.CalculateBarWidth(clusterWidth);

                var left = offsetX - clusterWidth / 2;
                foreach (var segment in series.Segments)
                {
                    var value = coordinate.GetValue(segment);
                    if (value != null)
                    {
                        var offsetY = chartContext.GetOffsetY((decimal)value);
                        drawingContext.DrawEllipse(
                            stroke: segment.Fill,
                            strokeThickness: layer.HighlightMarkerStrokeThickness,
                            fill: layer.HighlightMarkerFill,
                            size: new Size(Math.Max(0, progress * layer.HighlightMarkerSize), Math.Max(0, progress * layer.HighlightMarkerSize)),
                            centerPoint: new Point(left + columnWidth / 2, offsetY));
                    }
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
