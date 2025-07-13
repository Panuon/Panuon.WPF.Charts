using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class LineSeries
        : CartesianValueProviderSeriesBase
    {
        #region Fields
        private Dictionary<ICartesianCoordinate, CartesianCoordinateInfo> _coordinateInfos;
        #endregion

        #region Ctor
        static LineSeries()
        {
            ToggleHighlightLayer.Regist<LineSeries>(OnToggleHighlighting);
            //ScaleHighlightLayer.Regist<LineSeries>(OnScaleHighlighting);
        }
        #endregion

        #region Properties

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LineSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineSeries), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerShape
        public MarkerShape MarkerShape
        {
            get { return (MarkerShape)GetValue(MarkerShapeProperty); }
            set { SetValue(MarkerShapeProperty, value); }
        }

        public static readonly DependencyProperty MarkerShapeProperty =
            DependencyProperty.Register("MarkerShape", typeof(MarkerShape), typeof(LineSeries), new FrameworkPropertyMetadata(MarkerShape.Circle, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerStroke
        public Brush MarkerStroke
        {
            get { return (Brush)GetValue(MarkerStrokeProperty) ?? (Brush)GetValue(StrokeProperty); }
            set { SetValue(MarkerStrokeProperty, value); }
        }

        public static readonly DependencyProperty MarkerStrokeProperty =
            DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(LineSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerStrokeThickness
        public double? MarkerStrokeThickness
        {
            get { return (double?)GetValue(MarkerStrokeThicknessProperty) ?? (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(MarkerStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty MarkerStrokeThicknessProperty =
            DependencyProperty.Register("MarkerStrokeThickness", typeof(double?), typeof(LineSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerFill
        public Brush MarkerFill
        {
            get { return (Brush)GetValue(MarkerFillProperty) ?? (Brush)GetValue(FillProperty); }
            set { SetValue(MarkerFillProperty, value); }
        }

        public static readonly DependencyProperty MarkerFillProperty =
            DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(LineSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerSize
        public double MarkerSize
        {
            get { return (double)GetValue(MarkerSizeProperty); }
            set { SetValue(MarkerSizeProperty, value); }
        }

        public static readonly DependencyProperty MarkerSizeProperty =
            DependencyProperty.Register("MarkerSize", typeof(double), typeof(LineSeries), new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Events

        public event DrawingMarkerEventHandler DrawingMarker;
        #endregion

        #region Overrides

        #region OnRenderBegin
        protected override void OnRenderBegin(
            IDrawingContext drawingContext,
            ICartesianChartContext chartContext
)
        {
            var coordinates = chartContext.Coordinates;

            _coordinateInfos = new Dictionary<ICartesianCoordinate, CartesianCoordinateInfo>();
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
                    offsetY = coordinate.Offset;
                    offsetX = value == null ? (double?)null : chartContext.GetOffsetY((decimal)value);
                }

                if (offsetX == null || offsetY == null || value == null)
                {
                    _coordinateInfos.Add(coordinate, null);
                }
                else
                {
                    _coordinateInfos.Add(
                        coordinate,
                        new CartesianCoordinateInfo(new Point((double)offsetX, (double)offsetY), (decimal)value)
                    );
                }

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
            // 过滤出非null的坐标信息并按原顺序存储
            var validCoordinateInfos = _coordinateInfos
                .Where(kvp => kvp.Value != null)
                .ToList();

            if (validCoordinateInfos.Count < 2)
            {
                return;
            }

            var totalLength = 0d;
            var segmentLengths = new List<double>();

            for (int i = 0; i < validCoordinateInfos.Count - 1; i++)
            {
                double segmentLength = (validCoordinateInfos[i + 1].Value.Point - validCoordinateInfos[i].Value.Point).Length;
                segmentLengths.Add(segmentLength);
                totalLength += segmentLength;
            }

            var targetLength = totalLength * animationProgress;

            var accumulatedLength = 0d;
            var lastPoint = validCoordinateInfos[0].Value.Point;
            var toggleFill = MarkerFill ?? ((MarkerStroke == null || MarkerStrokeThickness == 0) ? Stroke : null);

            var strokeGeometry = new StreamGeometry();
            var fillGeometry = new StreamGeometry();

            using (var strokeCtx = strokeGeometry.Open())
            {
                using (var fillCtx = fillGeometry.Open())
                {
                    strokeCtx.BeginFigure(validCoordinateInfos[0].Value.Point, false, false);

                    if (!chartContext.SwapXYAxes)
                    {
                        fillCtx.BeginFigure(new Point(validCoordinateInfos[0].Value.Point.X, chartContext.CanvasHeight), true, true);
                    }
                    else
                    {
                        fillCtx.BeginFigure(new Point(0, validCoordinateInfos[0].Value.Point.Y), true, true);
                    }

                    fillCtx.LineTo(validCoordinateInfos[0].Value.Point, true, false);

                    for (int i = 0; i < segmentLengths.Count; i++)
                    {
                        var point = validCoordinateInfos[i + 1].Value.Point;
                        var segmentLength = segmentLengths[i];

                        if (accumulatedLength + segmentLength >= targetLength)
                        {
                            var remainingLength = targetLength - accumulatedLength;
                            var t = remainingLength / segmentLength;

                            var p1 = validCoordinateInfos[i].Value.Point;
                            var p2 = validCoordinateInfos[i + 1].Value.Point;

                            var x = p1.X + t * (p2.X - p1.X);
                            var y = p1.Y + t * (p2.Y - p1.Y);

                            strokeCtx.LineTo(new Point(x, y), true, false);
                            fillCtx.LineTo(new Point(x, y), true, false);

                            if (!chartContext.SwapXYAxes)
                            {
                                fillCtx.LineTo(new Point(x, chartContext.CanvasHeight), true, false);
                            }
                            else
                            {
                                fillCtx.LineTo(new Point(0, y), true, false);
                            }

                            break;
                        }

                        strokeCtx.LineTo(point, true, false);
                        fillCtx.LineTo(point, true, false);

                        lastPoint = point;
                        accumulatedLength += segmentLength;
                    }
                }
            }

            drawingContext.DrawGeometry(
               stroke: null,
               strokeThickness: 0,
               fill: Fill,
               fillGeometry
            );

            drawingContext.DrawGeometry(
                stroke: Stroke,
                strokeThickness: StrokeThickness,
                fill: null,
                strokeGeometry
            );

            #region Draw Toggle
            accumulatedLength = 0d;
            if (MarkerSize > 0
                && (MarkerFill != null || MarkerStroke != null))
            {
                for (int i = 0; i < segmentLengths.Count; i++)
                {
                    var coordinate = validCoordinateInfos[i].Key;
                    var coordinateInfo = validCoordinateInfos[i].Value;

                    var nextCoordinate = validCoordinateInfos[i + 1].Key;
                    var nextCoordinateInfo = validCoordinateInfos[i + 1].Value;

                    var segmentLength = segmentLengths[i];

                    DrawMarker(
                        drawingContext,
                        coordinate: coordinate,
                        value: (double)coordinateInfo.Value,
                        MarkerShape,
                        centerPoint: coordinateInfo.Point,
                        MarkerStroke,
                        MarkerStrokeThickness,
                        toggleFill,
                        MarkerSize);

                    if (animationProgress < 1
                        && accumulatedLength + segmentLength >= targetLength)
                    {
                        break;
                    }

                    //last Ellipse
                    DrawMarker(
                        drawingContext,
                        coordinate: nextCoordinate,
                        value: (double)nextCoordinateInfo.Value,
                        MarkerShape,
                        centerPoint: nextCoordinateInfo.Point,
                        MarkerStroke,
                        MarkerStrokeThickness,
                        toggleFill,
                        MarkerSize);

                    accumulatedLength += segmentLength;
                }
            }
            #endregion

            #region Draw Label
            accumulatedLength = 0d;
            if (ShowValueLabels)
            {
                for (int i = 0; i < validCoordinateInfos.Count; i++)
                {
                    var coordinate = validCoordinateInfos[i].Key;
                    var coordinateInfo = validCoordinateInfos[i].Value;

                    // 获取值，确保非null
                    var value = coordinate.GetValue(this);
                    if (value == null)
                        continue;

                    var label = CreateFormattedText(value.ToString(), Foreground);

                    var fill = Foreground;
                    var labelOffsetY = 0d;
                    if (InvertForeground != null)
                    {
                        switch (ValueLabelPlacement)
                        {
                            case SeriesLabelPlacement.Top:
                            case SeriesLabelPlacement.Bottom:
                                if (coordinateInfo.Point.Y < label.Height / 2)
                                {
                                    fill = InvertForeground;
                                }
                                labelOffsetY = 0;
                                break;
                            case SeriesLabelPlacement.Above:
                                labelOffsetY = Math.Max(0, coordinateInfo.Point.Y - label.Height) - MarkerSize;
                                if (coordinateInfo.Point.Y < label.Height / 2)
                                {
                                    fill = InvertForeground;
                                }
                                break;
                        }
                    }
                    drawingContext.DrawText(
                        label,
                        new Point(coordinateInfo.Point.X, labelOffsetY),
                        fill: fill,
                        stroke: ValueLabelStroke,
                        strokeThickness: ValueLabelStrokeThickness);

                    if (i < segmentLengths.Count)
                    {
                        if (animationProgress < 1
                            && accumulatedLength + segmentLengths[i] >= targetLength)
                        {
                            break;
                        }
                        accumulatedLength += segmentLengths[i];
                    }
                }
            }
            #endregion
        }
        #endregion

        #region OnRetrieveLegendEntries
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries()
        {
            yield return new SeriesLegendEntry(
                Title,
                markerShape: MarkerShape,
                markerStroke: MarkerStroke,
                markerStrokeThickness: MarkerStrokeThickness ?? 0,
                markerFill: MarkerFill);
        }
        #endregion

        #endregion

        #region Event Handlers
        public static void OnToggleHighlighting(
            ToggleHighlightLayer layer,
            LineSeries series,
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
                var coordinateInfo = series._coordinateInfos.ElementAt(coordinate.Index).Value;

                if (!chartContext.SwapXYAxes)
                {
                    series.DrawMarker(
                        drawingContext,
                        coordinate,
                        value: (double)coordinateInfo.Value,
                        series.MarkerShape,
                        centerPoint: new Point(coordinate.Offset, coordinateInfo.Point.Y),
                        stroke: series.Stroke ?? series.MarkerStroke,
                        strokeThickness: layer.HighlightMarkerStrokeThickness,
                        fill: layer.HighlightMarkerFill,
                        size: Math.Max(0, progress * layer.HighlightMarkerSize)
                    );
                }
                else
                {
                    series.DrawMarker(
                        drawingContext,
                        coordinate,
                        value: (double)coordinateInfo.Value,
                        series.MarkerShape,
                        centerPoint: new Point(coordinateInfo.Point.X, coordinate.Offset),
                        stroke: series.Stroke ?? series.MarkerStroke,
                        strokeThickness: layer.HighlightMarkerStrokeThickness,
                        fill: layer.HighlightMarkerFill,
                        size: Math.Max(0, progress * layer.HighlightMarkerSize)
                    );
                }
            }
        }

        //public static void OnScaleHighlighting(
        //    LayerBase layer,
        //    SeriesBase series,
        //    SeriesHighlightEventArgs args
        //)
        //{
        //    var scaleLayer = layer as ScaleHighlightLayer;

        //    var lineSeries = series as LineSeries;
        //    var drawingContext = args.DrawingContext;
        //    var chartContext = args.ChartContext;
        //    var layerContext = args.LayerContext;
        //    var coordinatesProgress = args.CoordinatesProgress;

        //    foreach (var coordinateProgress in coordinatesProgress)
        //    {
        //        var coordinate = coordinateProgress.Key;
        //        var progress = coordinateProgress.Value;

        //        if (progress == 0)
        //        {
        //            continue;
        //        }
        //        var point = lineSeries._valuePoints[coordinate.Index];
        //        Point? lastPoint = null;
        //        if (coordinate.Index > 0)
        //        {
        //            lastPoint = lineSeries._valuePoints[coordinate.Index - 1];
        //        }
        //        Point? nextPoint = null;
        //        if (coordinate.Index < chartContext.Coordinates.Count() - 1)
        //        {
        //            nextPoint = lineSeries._valuePoints[coordinate.Index + 1];
        //        }

        //        if (lastPoint != null)
        //        {
        //            drawingContext.DrawLine(
        //                stroke: lineSeries.Stroke,
        //                strokeThickness: lineSeries.StrokeThickness + progress * 2,
        //                startX: lastPoint.Value.X,
        //                startY: lastPoint.Value.Y,
        //                endX: point.X,
        //                endY: point.Y
        //            );
        //            drawingContext.DrawEllipse(
        //                stroke: lineSeries.Stroke,
        //                strokeThickness: layerContext.HighlightLayer.HighlightMarkerStrokeThickness,
        //                fill: layerContext.HighlightLayer.HighlightMarkerFill,
        //                radiusX: lineSeries.MarkerSize,
        //                radiusY: lineSeries.MarkerSize,
        //                startX: lastPoint.Value.X,
        //                startY: lastPoint.Value.Y
        //            );
        //        }
        //        if (nextPoint != null)
        //        {
        //            drawingContext.DrawLine(
        //                stroke: Stroke,
        //                strokeThickness: StrokeThickness + progress * 2,
        //                startX: point.X,
        //                startY: point.Y,
        //                endX: nextPoint.Value.X,
        //                endY: nextPoint.Value.Y
        //            );
        //            drawingContext.DrawEllipse(
        //                stroke: Stroke,
        //                strokeThickness: layerContext.HighlightLayer.HighlightMarkerStrokeThickness,
        //                fill: layerContext.HighlightLayer.HighlightMarkerFill,
        //                radiusX: lineSeries.MarkerSize,
        //                radiusY: lineSeries.MarkerSize,
        //                startX: nextPoint.Value.X,
        //                startY: nextPoint.Value.Y
        //            );
        //        }
        //        drawingContext.DrawEllipse(
        //            stroke: Stroke,
        //            strokeThickness: layerContext.HighlightLayer.HighlightMarkerStrokeThickness + progress * 2,
        //            fill: layerContext.HighlightLayer.HighlightMarkerFill,
        //            radiusX: lineSeries.MarkerSize + progress * 2,
        //            radiusY: lineSeries.MarkerSize + progress * 2,
        //            startX: coordinate.Offset,
        //            startY: point.Y
        //        );
        //    }
        //}
        #endregion

        #region Functions
        private void DrawMarker(
            IDrawingContext drawingContext,
            ICartesianCoordinate coordinate,
            double value,
            MarkerShape shape,
            Point centerPoint,
            Brush stroke,
            double? strokeThickness,
            Brush fill,
            double size)
        {
            var eventArgs = new DrawingMarkerEventArgs(
                drawingContext,
                this,
                coordinate,
                value,
                centerPoint,
                stroke: stroke,
                strokeThickness: strokeThickness ?? 0,
                fill: fill,
                size: size);
            DrawingMarker?.Invoke(drawingContext, eventArgs);
            if (!eventArgs.Cancel)
            {
                switch (shape)
                {
                    case MarkerShape.Circle:
                        drawingContext.DrawEllipse(
                            stroke: eventArgs.Stroke,
                            strokeThickness: eventArgs.StrokeThickness,
                            fill: eventArgs.Fill,
                            size: new Size(eventArgs.Size, eventArgs.Size),
                            centerPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.Triangle:
                        drawingContext.DrawTriangle(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           centerPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.Square:
                        drawingContext.DrawRectangle(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           centerPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.Diamond:
                        drawingContext.DrawDiamond(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           centerPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.Cross:
                        drawingContext.DrawCross(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           startPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.Star:
                        drawingContext.DrawStar(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           centerPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.ArrowUp:
                        drawingContext.DrawArrowUp(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           targetPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.ArrowDown:
                        drawingContext.DrawArrowDown(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           targetPoint: eventArgs.CenterPoint);
                        break;
                    case MarkerShape.Plus:
                        drawingContext.DrawPlus(
                           stroke: eventArgs.Stroke,
                           strokeThickness: eventArgs.StrokeThickness,
                           fill: eventArgs.Fill,
                           size: new Size(eventArgs.Size, eventArgs.Size),
                           centerPoint: eventArgs.CenterPoint);
                        break;
                }
            }
        }

        private static double CalculateRayLength(
            double radius,
            double angle
        )
        {
            var theta = angle * Math.PI / 180;

            var cx = radius / 2;
            var cy = radius / 2;

            var left = 0;
            var right = radius;
            var top = radius;
            var bottom = 0;

            var dx = Math.Cos(theta);
            var dy = Math.Sin(theta);

            var intersectionDistance = double.MaxValue;

            if (dx != 0)
            {
                if (dx > 0)
                {
                    var t = (right - cx) / dx;
                    var y = cy + t * dy;
                    if (y >= bottom && y <= top)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
                else
                {
                    var t = (left - cx) / dx;
                    var y = cy + t * dy;
                    if (y >= bottom && y <= top)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
            }

            if (dy != 0)
            {
                if (dy > 0)
                {
                    var t = (top - cy) / dy;
                    var x = cx + t * dx;
                    if (x >= left && x <= right)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
                else
                {
                    var t = (bottom - cy) / dy;
                    var x = cx + t * dx;
                    if (x >= left && x <= right)
                        intersectionDistance = Math.Min(intersectionDistance, t);
                }
            }

            return intersectionDistance;
        }
        #endregion
    }
}