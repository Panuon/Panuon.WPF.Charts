using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class DotSeries
        : CartesianValueProviderSeriesBase
    {
        #region Fields
        private List<Point> _valuePoints;
        #endregion

        #region Ctor
        static DotSeries()
        {
            ToggleHighlightLayer.Regist<DotSeries>(OnToggleHighlighting);
        }
        #endregion

        #region Properties

        #region ToggleStroke
        public Brush ToggleStroke
        {
            get { return (Brush)GetValue(ToggleStrokeProperty); }
            set { SetValue(ToggleStrokeProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeProperty =
            DependencyProperty.Register("ToggleStroke", typeof(Brush), typeof(DotSeries), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ToggleStrokeThickness
        public double ToggleStrokeThickness
        {
            get { return (double)GetValue(ToggleStrokeThicknessProperty); }
            set { SetValue(ToggleStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ToggleStrokeThicknessProperty =
            DependencyProperty.Register("ToggleStrokeThickness", typeof(double), typeof(DotSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ToggleFill
        public Brush ToggleFill
        {
            get { return (Brush)GetValue(ToggleFillProperty); }
            set { SetValue(ToggleFillProperty, value); }
        }

        public static readonly DependencyProperty ToggleFillProperty =
            DependencyProperty.Register("ToggleFill", typeof(Brush), typeof(DotSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region ToggleRadius
        public double ToggleRadius
        {
            get { return (double)GetValue(ToggleRadiusProperty); }
            set { SetValue(ToggleRadiusProperty, value); }
        }

        public static readonly DependencyProperty ToggleRadiusProperty =
            DependencyProperty.Register("ToggleRadius", typeof(double), typeof(DotSeries), new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));
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
            var toggleFill = ToggleFill ?? ToggleStroke;

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
                    var remainingLength = targetLength - accumulatedLength;
                    var t = remainingLength / segmentLength;

                    var p1 = _valuePoints[i];
                    var p2 = _valuePoints[i + 1];

                    var x = p1.X + t * (p2.X - p1.X);
                    var y = p1.Y + t * (p2.Y - p1.Y);

                    return;
                }


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
        public static void OnToggleHighlighting(
           ToggleHighlightLayer layer,
           DotSeries series,
           IDrawingContext drawingContext,
           ICartesianChartContext chartContext,
           ILayerContext layerContext,
           IDictionary<int, double> coordinatesProgress
        )
        {
            foreach (var coordinateProgress in coordinatesProgress)
            {
                var coordinate = chartContext.Coordinates.FirstOrDefault(c => c.Index == coordinateProgress.Key);
                if (coordinate != null)
                {

                    var progress = coordinateProgress.Value;

                    if (progress == 0)
                    {
                        continue;
                    }
                    var point = series._valuePoints[coordinate.Index];

                    drawingContext.DrawEllipse(
                        stroke: series.ToggleStroke,
                        strokeThickness: layer.HighlightToggleStrokeThickness,
                        fill: layer.HighlightToggleFill,
                        radiusX: series.ToggleRadius + progress * (layer.HighlightToggleRadius - series.ToggleRadius),
                        radiusY: series.ToggleRadius + progress * (layer.HighlightToggleRadius - series.ToggleRadius),
                        startX: coordinate.Offset,
                        startY: point.Y
                    );
                }
            }
        }
        #endregion

        #region OnLegend
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries (
            ICartesianChartContext chartContext, 
            ILayerContext layerContext
        )
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinate = layerContext.GetCoordinate(position.X);
                var toggleBrush = ToggleFill ?? ToggleStroke;

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffsetY(value);

                yield return new SeriesLegendEntry(toggleBrush, Title ?? coordinate.Title, value.ToString());
            }
        }
        #endregion

        #endregion

        #region Functions
        #endregion
    }
}