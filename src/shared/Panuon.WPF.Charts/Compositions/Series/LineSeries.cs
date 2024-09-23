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
        private List<Point> _valuePoints;
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

        #region ToggleStroke
        public Brush ToggleStroke
        {
            get { return (Brush)GetValue(ToggleStrokeProperty); }
            set { SetValue(ToggleStrokeProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeProperty =
            DependencyProperty.Register("ToggleStroke", typeof(Brush), typeof(LineSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ToggleStrokeThickness
        public double ToggleStrokeThickness
        {
            get { return (double)GetValue(ToggleStrokeThicknessProperty); }
            set { SetValue(ToggleStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeThicknessProperty =
            DependencyProperty.Register("ToggleStrokeThickness", typeof(double), typeof(LineSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ToggleFill
        public Brush ToggleFill
        {
            get { return (Brush)GetValue(ToggleFillProperty); }
            set { SetValue(ToggleFillProperty, value); }
        }

        public static readonly DependencyProperty ToggleFillProperty =
            DependencyProperty.Register("ToggleFill", typeof(Brush), typeof(LineSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ToggleRadius
        public double ToggleRadius
        {
            get { return (double)GetValue(ToggleRadiusProperty); }
            set { SetValue(ToggleRadiusProperty, value); }
        }

        public static readonly DependencyProperty ToggleRadiusProperty =
            DependencyProperty.Register("ToggleRadius", typeof(double), typeof(LineSeries), new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));
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

            _valuePoints = new List<Point>();
            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);

                double offsetX = 0d;
                double offsetY = 0d;

                if (!chartContext.SwapXYAxes)
                {
                    offsetX = coordinate.Offset;
                    offsetY = chartContext.GetOffsetY(value);
                }
                else
                {
                    offsetX = chartContext.GetOffsetY(value);
                    offsetY = coordinate.Offset;
                }

                _valuePoints.Add(
                    new Point(
                        x: offsetX,
                        y: offsetY
                    )
                );
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
            if (_valuePoints.Count < 2)
            {
                return;
            }

            var totalLength = 0d;
            var segmentLengths = new List<double>();

            for (int i = 0; i < _valuePoints.Count - 1; i++)
            {
                double segmentLength = (_valuePoints[i + 1] - _valuePoints[i]).Length;
                segmentLengths.Add(segmentLength);
                totalLength += segmentLength;
            }

            var targetLength = totalLength * animationProgress;

            var accumulatedLength = 0d;
            var lastPoint = _valuePoints[0];
            var toggleFill = ToggleFill ?? ((ToggleStroke == null || ToggleStrokeThickness == 0) ? Stroke : null);

            var strokeGeometry = new StreamGeometry();
            var fillGeometry = new StreamGeometry();

            using (var strokeCtx = strokeGeometry.Open())
            {
                using (var fillCtx = fillGeometry.Open())
                {
                    strokeCtx.BeginFigure(_valuePoints[0], false, false);

                    if (!chartContext.SwapXYAxes)
                    {
                        fillCtx.BeginFigure(new Point(_valuePoints[0].X, chartContext.AreaHeight), true, true);
                    }
                    else
                    {
                        fillCtx.BeginFigure(new Point(0, _valuePoints[0].Y), true, true);
                    }

                    fillCtx.LineTo(_valuePoints[0], true, false);

                    for (int i = 0; i < segmentLengths.Count; i++)
                    {
                        var point = _valuePoints[i + 1];
                        var segmentLength = segmentLengths[i];

                        if (accumulatedLength + segmentLength >= targetLength)
                        {
                            var remainingLength = targetLength - accumulatedLength;
                            var t = remainingLength / segmentLength;

                            var p1 = _valuePoints[i];
                            var p2 = _valuePoints[i + 1];

                            var x = p1.X + t * (p2.X - p1.X);
                            var y = p1.Y + t * (p2.Y - p1.Y);

                            strokeCtx.LineTo(new Point(x, y), true, false);
                            fillCtx.LineTo(new Point(x, y), true, false);

                            if (!chartContext.SwapXYAxes)
                            {
                                fillCtx.LineTo(new Point(x, chartContext.AreaHeight), true, false);
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

            accumulatedLength = 0d;
            lastPoint = _valuePoints[0];
            for (int i = 0; i < segmentLengths.Count; i++)
            {
                var point = _valuePoints[i + 1];
                var segmentLength = segmentLengths[i];

                if (animationProgress >= 0)
                {
                    drawingContext.DrawEllipse(
                        ToggleStroke,
                        ToggleStrokeThickness,
                        toggleFill,
                        ToggleRadius,
                        ToggleRadius,
                        _valuePoints[i].X,
                        _valuePoints[i].Y
                    );
                }
                if (animationProgress == 1)
                {
                    drawingContext.DrawEllipse(
                        ToggleStroke,
                        ToggleStrokeThickness,
                        toggleFill,
                        ToggleRadius,
                        ToggleRadius,
                        _valuePoints.Last().X,
                        _valuePoints.Last().Y
                    );
                }

                if (accumulatedLength + segmentLength >= targetLength)
                {
                    break;
                }

                //last Ellipse
                drawingContext.DrawEllipse(
                    ToggleStroke,
                    ToggleStrokeThickness,
                    toggleFill,
                    ToggleRadius,
                    ToggleRadius,
                    point.X,
                    point.Y
                );

                lastPoint = point;
                accumulatedLength += segmentLength;
            }
        }
        #endregion

        #region OnRetrieveLegendEntries
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries (
            ICartesianChartContext chartContext
        )
        {
            if (chartContext.GetMousePosition(MouseRelativeTarget.Layer) is Point offset)
            {
                var coordinate = chartContext.RetrieveCoordinate(offset);

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffsetY(value);
                yield return new SeriesLegendEntry(Stroke, Title ?? coordinate.Title, value.ToString());
            }
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
                var point = series._valuePoints[coordinate.Index];

                if (!chartContext.SwapXYAxes)
                {
                    drawingContext.DrawEllipse(
                        stroke: series.ToggleStroke ?? series.Fill,
                        strokeThickness: layer.HighlightToggleStrokeThickness,
                        fill: layer.HighlightToggleFill,
                        radiusX: progress * layer.HighlightToggleRadius,
                        radiusY: progress * layer.HighlightToggleRadius,
                        startX: coordinate.Offset,
                        startY: point.Y
                    );
                }
                else
                {
                    drawingContext.DrawEllipse(
                        stroke: series.ToggleStroke ?? series.Fill,
                        strokeThickness: layer.HighlightToggleStrokeThickness,
                        fill: layer.HighlightToggleFill,
                        radiusX: progress * layer.HighlightToggleRadius,
                        radiusY: progress * layer.HighlightToggleRadius,
                        startX: point.X,
                        startY: coordinate.Offset
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
        //                strokeThickness: layerContext.HighlightLayer.HighlightToggleStrokeThickness,
        //                fill: layerContext.HighlightLayer.HighlightToggleFill,
        //                radiusX: lineSeries.ToggleRadius,
        //                radiusY: lineSeries.ToggleRadius,
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
        //                strokeThickness: layerContext.HighlightLayer.HighlightToggleStrokeThickness,
        //                fill: layerContext.HighlightLayer.HighlightToggleFill,
        //                radiusX: lineSeries.ToggleRadius,
        //                radiusY: lineSeries.ToggleRadius,
        //                startX: nextPoint.Value.X,
        //                startY: nextPoint.Value.Y
        //            );
        //        }
        //        drawingContext.DrawEllipse(
        //            stroke: Stroke,
        //            strokeThickness: layerContext.HighlightLayer.HighlightToggleStrokeThickness + progress * 2,
        //            fill: layerContext.HighlightLayer.HighlightToggleFill,
        //            radiusX: lineSeries.ToggleRadius + progress * 2,
        //            radiusY: lineSeries.ToggleRadius + progress * 2,
        //            startX: coordinate.Offset,
        //            startY: point.Y
        //        );
        //    }
        //}
        #endregion

        #region Functions
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