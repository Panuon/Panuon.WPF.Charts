using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ColumnSeries
        : CartesianValueProviderSeriesBase
    {
        #region Fields
        private Dictionary<ICoordinate, Point?> _coordinatePoints;
        #endregion

        #region Ctor
        static ColumnSeries()
        {
            ToggleHighlightLayer.Regist<ColumnSeries>(OnToggleHighlighting);
        }
        #endregion

        #region Properties

        #region BackgroundFill
        public Brush BackgroundFill
        {
            get { return (Brush)GetValue(BackgroundFillProperty); }
            set { SetValue(BackgroundFillProperty, value); }
        }

        public static readonly DependencyProperty BackgroundFillProperty =
            DependencyProperty.Register("BackgroundFill", typeof(Brush), typeof(ColumnSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ColumnSeries), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ColumnSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ColumnSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ColumnWidth
        public GridLength ColumnWidth
        {
            get { return (GridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(GridLength), typeof(ColumnSeries), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Radius
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(ColumnSeries), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
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

            _coordinatePoints = new Dictionary<ICoordinate, Point?>();
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

                var value = coordinate.GetValue(this);

                double? offsetX = 0d;
                double? offsetY = 0d;

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
                    offsetX = coordinate.Offset;
                    offsetY = value == null ? (double?)null : chartContext.GetOffsetY((decimal)value);
                }
                else
                {
                    offsetX = value == null ? (double?)null : chartContext.GetOffsetY((decimal)value);
                    offsetY = coordinate.Offset;
                }

                _coordinatePoints.Add(
                    coordinate,
                    (offsetX == null || offsetY == null) ? (Point?)null : new Point((double)offsetX, (double)offsetY)
                );

                lastCoordinate = coordinate;
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
            var delta = chartContext.SwapXYAxes
                ? chartContext.CanvasHeight / chartContext.Coordinates.Count()
                : chartContext.CanvasWidth / chartContext.Coordinates.Count();
            var columnSize = GridLengthUtil.GetActualValue(ColumnWidth, delta);

            foreach (var coordinatePoint in _coordinatePoints)
            {
                var coordinate = coordinatePoint.Key;
                if (coordinatePoint.Value != null)
                {
                    var offsetX = (double)coordinatePoint.Value?.X;
                    var offsetY = (double)coordinatePoint.Value?.Y;

                    if (!chartContext.SwapXYAxes)
                    {
                        if (BackgroundFill != null)
                        {
                            drawingContext.DrawRectangle(
                                stroke: null,
                                strokeThickness: 0,
                                fill: BackgroundFill,
                                centerPoint: new Point(offsetX, chartContext.CanvasHeight / 2),
                                size: new Size(columnSize, chartContext.CanvasHeight),
                                radius: new Size(Radius, Radius)
                            );
                        }

                        var startY = chartContext.CanvasHeight - (chartContext.CanvasHeight - offsetY) * animationProgress;
                        var height = (chartContext.CanvasHeight - offsetY) * animationProgress;

                        drawingContext.DrawRectangle(
                            stroke: Stroke,
                            strokeThickness: StrokeThickness,
                            fill: Fill,
                            centerPoint: new Point(offsetX, startY + height / 2),
                            size: new Size(columnSize, height),
                            radius: new Size(Radius, Radius));

                        if (ShowValueLabels)
                        {
                            var label = CreateFormattedText(coordinate.GetValue(this).ToString(), Foreground);

                            var fill = Foreground;
                            var labelOffsetY = 0d;
                            if (InvertForeground != null)
                            {
                                switch (ValueLabelPlacement)
                                {
                                    case SeriesLabelPlacement.Top:
                                        if (offsetY < label.Height / 2)
                                        {
                                            fill = InvertForeground;
                                        }
                                        labelOffsetY = 0;
                                        break;
                                    case SeriesLabelPlacement.Above:
                                        labelOffsetY = Math.Max(0, startY - label.Height);
                                        if (startY < label.Height / 2)
                                        {
                                            fill = InvertForeground;
                                        }
                                        break;
                                    case SeriesLabelPlacement.Bottom:
                                        if (height < label.Height / 2)
                                        {
                                            fill = InvertForeground;
                                        }
                                        labelOffsetY = chartContext.CanvasHeight - label.Height;
                                        break;
                                }
                            }

                            drawingContext.DrawText(
                                label,
                                startPoint: new Point(offsetX, labelOffsetY),
                                fill: fill,
                                stroke: ValueLabelStroke,
                                strokeThickness: ValueLabelStrokeThickness);
                        }
                    }
                    else
                    {
                        if (BackgroundFill != null)
                        {
                            drawingContext.DrawRectangle(
                                stroke: null,
                                strokeThickness: 0,
                                fill: BackgroundFill,
                                centerPoint: new Point(chartContext.CanvasWidth / 2, offsetY),
                                size: new Size(chartContext.CanvasWidth, columnSize),
                                radius: new Size(Radius, Radius));
                        }

                        var startY = offsetY - columnSize / 2;
                        var width = offsetX * animationProgress;

                        drawingContext.DrawRectangle(
                            stroke: Stroke,
                            strokeThickness: StrokeThickness,
                            fill: Fill,
                            centerPoint: new Point(offsetX * animationProgress / 2, startY + columnSize / 2),
                            size: new Size(offsetX * animationProgress, columnSize),
                            radius: new Size(Radius, Radius));

                        if (ShowValueLabels)
                        {
                            var label = CreateFormattedText(coordinate.GetValue(this).ToString(), Foreground);

                            var fill = Foreground;
                            var labelOffsetX = 0d;
                            if (InvertForeground != null)
                            {
                                switch (ValueLabelPlacement)
                                {
                                    case SeriesLabelPlacement.Top:
                                        if (chartContext.CanvasWidth - width < label.Width / 2)
                                        {
                                            fill = InvertForeground;
                                        }
                                        labelOffsetX = chartContext.CanvasWidth - label.Width;
                                        break;
                                    case SeriesLabelPlacement.Above:
                                        labelOffsetX = Math.Max(0, width + label.Width);
                                        if (chartContext.CanvasWidth - width < label.Width / 2)
                                        {
                                            fill = InvertForeground;
                                        }
                                        break;
                                    case SeriesLabelPlacement.Bottom:
                                        if (width < label.Width / 2)
                                        {
                                            fill = InvertForeground;
                                        }
                                        labelOffsetX = 0d;
                                        break;
                                }
                            }

                            drawingContext.DrawText(
                                label,
                                new Point(labelOffsetX, offsetY - label.Height / 2),
                                fill: fill,
                                stroke: ValueLabelStroke,
                                strokeThickness: ValueLabelStrokeThickness);
                        }

                    }
                }
            }
        }
        #endregion

        #region OnRetrieveLegendEntries
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries()
        {
            yield return new SeriesLegendEntry(
                Title,
                markerShape: MarkerShape.Circle,
                markerStroke: Stroke,
                markerStrokeThickness: StrokeThickness,
                markerFill: Fill);
        }
        #endregion

        #endregion

        #region Event Handlers
        public static void OnToggleHighlighting(
            ToggleHighlightLayer layer,
            ColumnSeries series,
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
                var point = series._coordinatePoints[coordinate];

                if (point != null)
                {
                    var offsetX = (double)point?.X;
                    var offsetY = (double)point?.Y;

                    if (!chartContext.SwapXYAxes)
                    {
                        drawingContext.DrawEllipse(
                            stroke: series.Fill,
                            strokeThickness: layer.HighlightMarkerStrokeThickness,
                            fill: layer.HighlightMarkerFill,
                            size: new Size(Math.Max(0, progress * layer.HighlightMarkerSize), Math.Max(0, progress * layer.HighlightMarkerSize)),
                            centerPoint: new Point(coordinate.Offset, offsetY)
                        );
                    }
                    else
                    {
                        drawingContext.DrawEllipse(
                            stroke: series.Fill,
                            strokeThickness: layer.HighlightMarkerStrokeThickness,
                            fill: layer.HighlightMarkerFill,
                            size: new Size(Math.Max(0, progress * layer.HighlightMarkerSize), Math.Max(0, progress * layer.HighlightMarkerSize)),
                            centerPoint: new Point(offsetX, coordinate.Offset)
                        );
                    }
                }
            }
        }
        #endregion
    }
}