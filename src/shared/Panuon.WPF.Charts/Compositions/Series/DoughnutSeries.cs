using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class DoughnutSeries
        : RadialValueProviderSegmentsSeriesBase<DoughnutSeriesSegment>
    {
        #region Structs
        private struct DoughnutSeriesSegmentInfo
        {
            public double StartAngle { get; set; }

            public double Angle { get; set; }

            public FormattedText Label { get; set; }
        }
        #endregion

        #region Fields
        private Dictionary<DoughnutSeriesSegment, DoughnutSeriesSegmentInfo> _segmentInfos;
        #endregion

        #region Ctor
        static DoughnutSeries()
        {
            ToggleHighlightLayer.Regist<DoughnutSeries>(OnToggleHighlighting);
        }
        #endregion

        #region Properties

        #region Thickness
        public GridLength Thickness
        {
            get { return (GridLength)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(GridLength), typeof(DoughnutSeries), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Events

        public event RadialChartGeneratingLabelRoutedEventHandler GeneratingLabel
        {
            add { AddHandler(GeneratingLabelEvent, value); }
            remove { RemoveHandler(GeneratingLabelEvent, value); }
        }

        public static readonly RoutedEvent GeneratingLabelEvent =
            EventManager.RegisterRoutedEvent("GeneratingLabel", RoutingStrategy.Bubble, typeof(RadialChartGeneratingLabelRoutedEventHandler), typeof(DoughnutSeries));
        #endregion

        #region Overrides

        #region OnRenderBegin
        protected override void OnRenderBegin(
            IDrawingContext drawingContext, 
            IRadialChartContext chartContext
        )
        {
            base.OnRenderBegin(drawingContext, chartContext);

            _segmentInfos = new Dictionary<DoughnutSeriesSegment, DoughnutSeriesSegmentInfo>();

            var chartPanel = chartContext.Chart;
            var coordinates = chartContext.Coordinates;

            var index = 0;
            var totalValue = coordinates.Sum(c => c.GetValue(this));
            foreach (var coordinate in coordinates)
            {
                if (index >= Segments.Count)
                {
                    break;
                }
                var segment = Segments[index];
                var value = coordinate.GetValue(this);
                var startAngle = coordinate.StartAngle;
                var angle = coordinate.Angle;

                var generatingLabelArgs = new RadialChartGeneratingLabelRoutedEventArgs(
                    GeneratingLabelEvent,
                    label: segment.Title ?? coordinate.Label,
                    value: value,
                    totalValue: totalValue
                );
                RaiseEvent(generatingLabelArgs);

                _segmentInfos[segment] = new DoughnutSeriesSegmentInfo()
                {
                    StartAngle = startAngle,
                    Angle = angle,
                    Label = string.IsNullOrEmpty(generatingLabelArgs.Label)
                        ? null
                        : new FormattedText(
                            generatingLabelArgs.Label,
                            CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface(chartPanel.FontFamily, chartPanel.FontStyle, chartPanel.FontWeight, chartPanel.FontStretch),
                            chartPanel.FontSize,
                            chartPanel.Foreground
        #if NET452 || NET462 || NET472 || NET48
        #else
                            , VisualTreeHelper.GetDpi(chartPanel).PixelsPerDip
        #endif
                        )
                            {
                                TextAlignment = TextAlignment.Center
                            }
                };
                index++;
            }
        }
        #endregion

        #region OnRendering
        protected override void OnRendering(
            IDrawingContext drawingContext,
            IRadialChartContext chartContext,
            double animationProgress
        )
        {
            var chartPanel = chartContext.Chart;
            var coordinates = chartContext.Coordinates;

            var areaWidth = Math.Max(0, chartContext.CanvasWidth - chartContext.Chart.LabelSpacing * 2 - chartContext.Chart.FontSize * 2);
            var areaHeight = Math.Max(0, chartContext.CanvasHeight - chartContext.Chart.LabelSpacing * 2 - chartContext.Chart.FontSize * 2);

            var outterRadius = Math.Min(areaWidth, areaHeight) / 2;
            var thickness = GridLengthUtil.GetActualValue(Thickness, outterRadius, 0.2);

            var centerX = chartContext.CanvasWidth / 2;
            var centerY = chartContext.CanvasHeight / 2;

            var totalValue = coordinates.Select(c => c.GetValue(this)).Sum();
            var angleDelta = 360d / totalValue;

            foreach (var segmentInfo in _segmentInfos)
            {
                var segment = segmentInfo.Key;
                var startAngle = segmentInfo.Value.StartAngle * animationProgress;
                var angle = Math.Round(segmentInfo.Value.Angle, 2) * animationProgress;

                drawingContext.DrawArc(segment.Stroke,
                        segment.StrokeThickness,
                        segment.Fill,
                        centerPoint: new Point(centerX, centerY),
                        Math.Max(0, outterRadius - thickness),
                        outterRadius,
                        startAngle,
                        startAngle + angle
                    );
            }

            foreach (var segmentInfo in _segmentInfos)
            {
                var segment = segmentInfo.Key;
                var formattedText = segmentInfo.Value.Label;
                var startAngle = segmentInfo.Value.StartAngle * animationProgress;
                var angle = Math.Round(segmentInfo.Value.Angle, 2) * animationProgress;

                var currentAngle = startAngle + angle / 2;
                var radian = (currentAngle - 90) * Math.PI / 180.0;
                var rayLength = formattedText == null
                    ? 0
                    : CalculateRayLength(formattedText.Width, formattedText.Height, currentAngle + 90);

                var halfPoint = new Point(
                    centerX + (outterRadius + chartContext.Chart.LabelSpacing + rayLength) * Math.Cos(radian),
                    centerY + (outterRadius + chartContext.Chart.LabelSpacing + rayLength) * Math.Sin(radian)
                );

                if (formattedText != null)
                {
                    if (segment.LabelStroke == null && segment.LabelForeground == null)
                    {
                        drawingContext.DrawText(
                            formattedText,
                            new Point(halfPoint.X, halfPoint.Y - formattedText.Height / 2)
                        );
                    }
                    else
                    {
                        drawingContext.DrawText(
                            formattedText,
                            new Point(halfPoint.X, halfPoint.Y - formattedText.Height / 2),
                            segment.LabelForeground,
                            segment.LabelStroke,
                            segment.LabelStrokeThickness ?? 0
                        );
                    }
                }
            }
        }
        #endregion

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

        #region Event Handlers
        public static void OnToggleHighlighting(
            ToggleHighlightLayer layer,
            DoughnutSeries series,
            IDrawingContext drawingContext,
            IRadialChartContext chartContext,
            IDictionary<int, double> coordinatesProgress
        )
        {
            var areaWidth = Math.Max(0, chartContext.CanvasWidth - chartContext.Chart.LabelSpacing * 2 - chartContext.Chart.FontSize * 2);
            var areaHeight = Math.Max(0, chartContext.CanvasHeight - chartContext.Chart.LabelSpacing * 2 - chartContext.Chart.FontSize * 2);

            foreach (var coordinateProgress in coordinatesProgress)
            {
                var index = coordinateProgress.Key;
                var coordinate = chartContext.Coordinates.FirstOrDefault(c => c.Index == index);
                var progress = coordinateProgress.Value;

                if (progress == 0)
                {
                    continue;
                }

                var radius = Math.Min(areaWidth, areaHeight) / 2;
                var centerX = chartContext.CanvasWidth / 2;
                var centerY = chartContext.CanvasHeight / 2;

                var segmentInfo = series._segmentInfos.ElementAt(index);
                var segment = segmentInfo.Key;
                var startAngle = segmentInfo.Value.StartAngle;
                var angle = Math.Round(segmentInfo.Value.Angle, 2);

                var radians = (startAngle + angle / 2 - 90) * Math.PI / 180.0;
                var point = new Point(
                    centerX + (radius - progress * layer.HighlightMarkerSize / 2) * Math.Cos(radians),
                    centerY + (radius - progress * layer.HighlightMarkerSize / 2) * Math.Sin(radians)
                );

                drawingContext.DrawEllipse(
                    stroke: segment.Fill,
                    strokeThickness: layer.HighlightMarkerStrokeThickness,
                    fill: layer.HighlightMarkerFill,
                    size: new Size(progress * layer.HighlightMarkerSize, progress * layer.HighlightMarkerSize),
                    centerPoint: point
                );
            }
        }
        #endregion

        #region Functions
        private bool IsPointInsideSector(Point point,
            double centerX,
            double centerY,
            double radius,
            double startAngle,
            double endAngle)
        {
            var distance = Math.Sqrt(Math.Pow(point.X - centerX, 2) + Math.Pow(point.Y - centerY, 2));
            var angle = Math.Atan2(point.Y - centerY, point.X - centerX) * (180 / Math.PI);

            angle += 90;
            if (angle < 0)
            {
                angle += 360;
            }

            return distance <= radius && angle >= startAngle && angle <= endAngle;
        }

        private static double CalculateRayLength(
            double width,
            double height,
            double angle
        )
        {
            var theta = angle * Math.PI / 180;

            var cx = width / 2;
            var cy = height / 2;

            var left = 0;
            var right = width;
            var top = height;
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
