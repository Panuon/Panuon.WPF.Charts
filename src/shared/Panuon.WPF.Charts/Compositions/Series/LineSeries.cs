using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region Properties

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LineSeries), new PropertyMetadata(null));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineSeries), new PropertyMetadata(Brushes.Black, OnRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineSeries), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region ToggleStroke
        public Brush ToggleStroke
        {
            get { return (Brush)GetValue(ToggleStrokeProperty); }
            set { SetValue(ToggleStrokeProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeProperty =
            DependencyProperty.Register("ToggleStroke", typeof(Brush), typeof(LineSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region ToggleStrokeThickness
        public double ToggleStrokeThickness
        {
            get { return (double)GetValue(ToggleStrokeThicknessProperty); }
            set { SetValue(ToggleStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeThicknessProperty =
            DependencyProperty.Register("ToggleStrokeThickness", typeof(double), typeof(LineSeries), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region ToggleFill
        public Brush ToggleFill
        {
            get { return (Brush)GetValue(ToggleFillProperty); }
            set { SetValue(ToggleFillProperty, value); }
        }

        public static readonly DependencyProperty ToggleFillProperty =
            DependencyProperty.Register("ToggleFill", typeof(Brush), typeof(LineSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region ToggleRadius
        public double ToggleRadius
        {
            get { return (double)GetValue(ToggleRadiusProperty); }
            set { SetValue(ToggleRadiusProperty, value); }
        }

        public static readonly DependencyProperty ToggleRadiusProperty =
            DependencyProperty.Register("ToggleRadius", typeof(double), typeof(LineSeries), new PropertyMetadata(3d, OnRenderPropertyChanged));
        #endregion

        #endregion

        #region Overrides

        #region OnRenderBegin
        protected override void OnRenderBegin(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            var coordinates = chartContext.Coordinates;

            _valuePoints = new List<Point>();
            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);
                var offsetX = coordinate.Offset;
                var offsetY = chartContext.GetOffsetY(value);

                _valuePoints.Add(
                    new Point(
                        x: coordinate.Offset,
                        y: chartContext.GetOffsetY(value)
                    )
                );
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

                    fillCtx.BeginFigure(new Point(_valuePoints[0].X, chartContext.AreaHeight), true, true);
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

                            fillCtx.LineTo(new Point(x, chartContext.AreaHeight), true, false);


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

        #region OnHighlighting
        protected override void OnHighlighting(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            IDictionary<ICoordinate, double> coordinateProgresses
        )
        {
            foreach (var coordinateProgress in coordinateProgresses)
            {
                var coordinate = coordinateProgress.Key;
                var progress = coordinateProgress.Value;

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffsetY(value);

                drawingContext.DrawEllipse(
                    stroke: Stroke,
                    strokeThickness: 2,
                    fill: Brushes.White,
                    radiusX: ToggleRadius + progress * 2,
                    radiusY: ToggleRadius + progress * 2,
                    startX: coordinate.Offset,
                    startY: offsetY
                );
            }
        }
        #endregion

        #region OnRetrieveLegendEntries
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries (
            IChartContext chartContext,
            ILayerContext layerContext
        )
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinate = layerContext.GetCoordinate(position.X);

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffsetY(value);
                yield return new SeriesLegendEntry(Stroke, Title ?? coordinate.Title, value.ToString());
            }
        }
        #endregion

        #endregion

        #region Functions
        #endregion
    }
}