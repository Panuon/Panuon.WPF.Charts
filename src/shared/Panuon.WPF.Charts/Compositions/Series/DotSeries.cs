using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class DotSeries
        : CartesianValueProviderSeriesBase
    {
        #region Fields
        private List<Point?> _valuePoints;
        #endregion

        #region Ctor
        static DotSeries()
        {
            ToggleHighlightLayer.Regist<DotSeries>(OnToggleHighlighting);
        }
        #endregion

        #region Properties

        #region MarkerStroke
        public Brush MarkerStroke
        {
            get { return (Brush)GetValue(MarkerStrokeProperty); }
            set { SetValue(MarkerStrokeProperty, value); }
        }

        public static readonly DependencyProperty MarkerStrokeProperty =
            DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(DotSeries), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerStrokeThickness
        public double MarkerStrokeThickness
        {
            get { return (double)GetValue(MarkerStrokeThicknessProperty); }
            set { SetValue(MarkerStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty MarkerStrokeThicknessProperty =
            DependencyProperty.Register("MarkerStrokeThickness", typeof(double), typeof(DotSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerFill
        public Brush MarkerFill
        {
            get { return (Brush)GetValue(MarkerFillProperty); }
            set { SetValue(MarkerFillProperty, value); }
        }

        public static readonly DependencyProperty MarkerFillProperty =
            DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(DotSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MarkerSize
        public double MarkerSize
        {
            get { return (double)GetValue(MarkerSizeProperty); }
            set { SetValue(MarkerSizeProperty, value); }
        }

        public static readonly DependencyProperty MarkerSizeProperty =
            DependencyProperty.Register("MarkerSize", typeof(double), typeof(DotSeries), new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));
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

            _valuePoints = new List<Point?>();
            foreach (var coordinate in coordinates)
            {
                var value = coordinate.GetValue(this);

                double? offsetX = 0d;
                double? offsetY = 0d;

                if (!chartContext.SwapXYAxes)
                {
                    offsetX = coordinate.Offset;
                    offsetY = value == null ? null : (double?)chartContext.GetOffsetY((decimal)value);
                }
                else
                {
                    offsetX = value == null ? null : (double?)chartContext.GetOffsetY((decimal)value);
                    offsetY = coordinate.Offset;
                }

                _valuePoints.Add((offsetX == null || offsetY == null) ? (Point?)null : new Point((double)offsetX, (double)offsetY));
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
            var validPoints = _valuePoints.Where(p => p.HasValue).Select(p => p.Value).ToList();

            if (validPoints.Count < 2)
            {
                return;
            }

            var totalLength = 0d;
            var segmentLengths = new List<double>();

            for (int i = 0; i < validPoints.Count - 1; i++)
            {
                double segmentLength = (validPoints[i + 1] - validPoints[i]).Length;
                segmentLengths.Add(segmentLength);
                totalLength += segmentLength;
            }

            var targetLength = totalLength * animationProgress;

            var accumulatedLength = 0d;
            var lastPoint = validPoints[0];
            var toggleFill = MarkerFill ?? MarkerStroke;

            for (int i = 0; i < segmentLengths.Count; i++)
            {
                var point = validPoints[i + 1];
                var segmentLength = segmentLengths[i];

                if (animationProgress >= 0)
                {
                    drawingContext.DrawEllipse(
                        stroke: MarkerStroke,
                        strokeThickness: MarkerStrokeThickness,
                        toggleFill,
                        size: new Size(MarkerSize, MarkerSize),
                        centerPoint: validPoints[i]);
                }

                if (animationProgress == 1 && i == segmentLengths.Count - 1)
                {
                    drawingContext.DrawEllipse(
                        stroke: MarkerStroke,
                        strokeThickness: MarkerStrokeThickness,
                        fill: toggleFill,
                        size: new Size(MarkerSize, MarkerSize),
                        centerPoint: validPoints.Last());
                }

                if (accumulatedLength + segmentLength >= targetLength)
                {
                    var remainingLength = targetLength - accumulatedLength;
                    var t = remainingLength / segmentLength;

                    var p1 = validPoints[i];
                    var p2 = validPoints[i + 1];

                    var x = p1.X + t * (p2.X - p1.X);
                    var y = p1.Y + t * (p2.Y - p1.Y);

                    return;
                }

                drawingContext.DrawEllipse(
                    stroke: MarkerStroke,
                    strokeThickness: MarkerStrokeThickness,
                    fill: toggleFill,
                    size: new Size(MarkerSize, MarkerSize),
                    centerPoint: point);

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

                if (point != null)
                {
                    var offsetX = (double)point?.X;
                    var offsetY = (double)point?.Y;

                    if (!chartContext.SwapXYAxes)
                    {
                        drawingContext.DrawEllipse(
                            stroke: series.MarkerStroke ?? series.MarkerFill,
                            strokeThickness: layer.HighlightMarkerStrokeThickness,
                            fill: layer.HighlightMarkerFill,
                            size: new Size(progress * layer.HighlightMarkerSize, progress * layer.HighlightMarkerSize),
                            centerPoint: new Point(coordinate.Offset, offsetY));
                    }
                    else
                    {
                        drawingContext.DrawEllipse(
                            stroke: series.MarkerStroke ?? series.MarkerFill,
                            strokeThickness: layer.HighlightMarkerStrokeThickness,
                            fill: layer.HighlightMarkerFill,
                            size: new Size(progress * layer.HighlightMarkerSize, progress * layer.HighlightMarkerSize),
                            centerPoint: new Point(offsetX, coordinate.Offset));
                    }
                }
            }
        }

        #endregion

        #region OnLegend
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries()
        {
            yield return new SeriesLegendEntry(
                    Title,
                    markerShape: MarkerShape.Circle,
                    markerStroke: MarkerStroke,
                    markerStrokeThickness: MarkerStrokeThickness,
                    markerFill: MarkerFill);
        }
        #endregion

        #endregion

        #region Functions
        #endregion
    }
}