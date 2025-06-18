using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class RadarSeries
        : RadialValueProviderSegmentsSeriesBase<RadarSeriesSegment>
    {
        #region Structs
        private struct RadarSeriesSegmentInfo
        {
            public double Angle { get; set; }

            public double Percent { get; set; }

            public FormattedText Label { get; set; }
        }
        #endregion

        #region Fields
        private Dictionary<RadarSeriesSegment, RadarSeriesSegmentInfo> _segmentInfos;
        #endregion

        #region Ctor
        static RadarSeries()
        {
            ToggleHighlightLayer.Regist<RadarSeries>(OnToggleHighlighting);
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
            DependencyProperty.Register("Fill", typeof(Brush), typeof(RadarSeries), new FrameworkPropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A000000")), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(RadarSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RadarSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region GridLinesVisibility
        public RadialChartGridLinesVisibility GridLinesVisibility
        {
            get { return (RadialChartGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(RadialChartGridLinesVisibility), typeof(RadarSeries), new FrameworkPropertyMetadata(RadialChartGridLinesVisibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region GridLinesBrush
        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register("GridLinesBrush", typeof(Brush), typeof(RadarSeries), new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region GridLinesThickness
        public double GridLinesThickness
        {
            get { return (double)GetValue(GridLinesThicknessProperty); }
            set { SetValue(GridLinesThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLinesThicknessProperty =
            DependencyProperty.Register("GridLinesThickness", typeof(double), typeof(RadarSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region GridLinesDashArray
        public DoubleCollection GridLinesDashArray
        {
            get { return (DoubleCollection)GetValue(GridLinesDashArrayProperty); }
            set { SetValue(GridLinesDashArrayProperty, value); }
        }

        public static readonly DependencyProperty GridLinesDashArrayProperty =
            DependencyProperty.Register("GridLinesDashArray", typeof(DoubleCollection), typeof(RadarSeries));
        #endregion

        #region OutterGridLineBrush
        public Brush OutterGridLineBrush
        {
            get { return (Brush)GetValue(OutterGridLineBrushProperty); }
            set { SetValue(OutterGridLineBrushProperty, value); }
        }

        public static readonly DependencyProperty OutterGridLineBrushProperty =
            DependencyProperty.Register("OutterGridLineBrush", typeof(Brush), typeof(RadarSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region OutterGridLineThickness
        public double? OutterGridLineThickness
        {
            get { return (double?)GetValue(OutterGridLineThicknessProperty); }
            set { SetValue(OutterGridLineThicknessProperty, value); }
        }

        public static readonly DependencyProperty OutterGridLineThicknessProperty =
            DependencyProperty.Register("OutterGridLineThickness", typeof(double?), typeof(RadarSeries), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region GridLinesSpacing
        public GridLength GridLinesSpacing
        {
            get { return (GridLength)GetValue(GridLinesSpacingProperty); }
            set { SetValue(GridLinesSpacingProperty, value); }
        }

        public static readonly DependencyProperty GridLinesSpacingProperty =
            DependencyProperty.Register("GridLinesSpacing", typeof(GridLength), typeof(RadarSeries), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region AxisStroke
        public Brush AxisStroke
        {
            get { return (Brush)GetValue(AxisStrokeProperty); }
            set { SetValue(AxisStrokeProperty, value); }
        }

        public static readonly DependencyProperty AxisStrokeProperty =
            RadarSeriesSegment.AxisStrokeProperty.AddOwner(typeof(RadarSeries), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region AxisStrokeThickness
        public double AxisStrokeThickness
        {
            get { return (double)GetValue(AxisStrokeThicknessProperty); }
            set { SetValue(AxisStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty AxisStrokeThicknessProperty =
            RadarSeriesSegment.AxisStrokeThicknessProperty.AddOwner(typeof(RadarSeries), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MinValue
        public decimal? MinValue
        {
            get { return (decimal?)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal?), typeof(RadarSeries), new PropertyMetadata(null));
        #endregion

        #region MaxValue
        public decimal? MaxValue
        {
            get { return (decimal?)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(decimal?), typeof(RadarSeries), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Events

        public event RadialChartGeneratingLabelRoutedEventHandler GeneratingLabel
        {
            add { AddHandler(GeneratingLabelEvent, value); }
            remove { RemoveHandler(GeneratingLabelEvent, value); }
        }

        public static readonly RoutedEvent GeneratingLabelEvent =
            EventManager.RegisterRoutedEvent("GeneratingLabel", RoutingStrategy.Bubble, typeof(RadialChartGeneratingLabelRoutedEventHandler), typeof(RadarSeries));
        #endregion

        #region Methods

        #region OnHighlighting
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries()
        {
            foreach (var segment in Segments)
            {
                yield return new SeriesLegendEntry(
                    segment.Title,
                    markerShape: MarkerShape.Circle,
                    markerStroke: segment.AxisStroke,
                    markerStrokeThickness: segment.AxisStrokeThickness,
                    markerFill: null); 
            }
        }
        #endregion

        #region OnRenderBegin
        protected override void OnRenderBegin(IDrawingContext drawingContext, IRadialChartContext chartContext)
        {
            base.OnRenderBegin(drawingContext, chartContext);

            _segmentInfos = new Dictionary<RadarSeriesSegment, RadarSeriesSegmentInfo>();

            var chartPanel = chartContext.Chart;
            var coordinates = chartContext.Coordinates;

            var index = 0;
            var angleDelta = 360.0 / Segments.Count;

            foreach (var coordinate in coordinates)
            {
                if (index >= Segments.Count)
                {
                    break;
                }
                var segment = Segments[index];
                var value = coordinate.GetValue(this) ?? 0m;
                var startAngle = angleDelta * index;

                CheckMinMaxValue(
                    (decimal)coordinates.Select(c => c.GetValue(this)).Where(c => c != null).Min(),
                    (decimal)coordinates.Select(c => c.GetValue(this)).Where(c => c != null).Max(),
                    out decimal minValue,
                    out decimal maxValue
                );
                var actualMaxValue = MaxValue ?? maxValue;
                var actualMinValue = MinValue ?? minValue;

                var generatingLabelArgs = new RadialChartGeneratingLabelRoutedEventArgs(
                    GeneratingLabelEvent,
                    label: segment.Title ?? coordinate.Label,
                    value: value,
                    totalValue: actualMaxValue
                );
                RaiseEvent(generatingLabelArgs);

                value = Math.Max(actualMinValue, Math.Min(actualMaxValue, value));
                _segmentInfos[segment] = new RadarSeriesSegmentInfo()
                {
                    Percent = (double)((actualMaxValue - value) / (actualMaxValue - actualMinValue)),
                    Angle = startAngle,
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
            var segments = Segments;
            if (segments.Count() < 3)
            {
                throw new InvalidOperationException("RadarSeries requires at least 3 segments to be formed.");
            }

            var chartPanel = chartContext.Chart;
            var coordinates = chartContext.Coordinates;

            var areaWidth = Math.Max(0, chartContext.CanvasWidth - chartContext.Chart.LabelSpacing * 2 - chartContext.Chart.FontSize * 2);
            var areaHeight = Math.Max(0, chartContext.CanvasHeight - chartContext.Chart.LabelSpacing * 2 - chartContext.Chart.FontSize * 2);

            var centerX = chartContext.CanvasWidth / 2;
            var centerY = chartContext.CanvasHeight / 2;
            var radius = Math.Min(areaWidth, areaHeight) / 2;

            #region GridLines

            if (GridLinesVisibility == RadialChartGridLinesVisibility.Visible)
            {
                var gridLinesSpacing = GridLengthUtil.GetActualValue(
                    GridLinesSpacing,
                    radius,
                    autoPercent: 0.3
                );
                var gridLinesCount = (int)Math.Ceiling(radius / gridLinesSpacing);
                gridLinesSpacing = radius / gridLinesCount;

                for (int gridLinesIndex = 0; gridLinesIndex < gridLinesCount; gridLinesIndex++)
                {
                    if (gridLinesIndex == gridLinesCount - 1)
                    {
                        DrawPolygon(
                            drawingContext: drawingContext,
                            sides: segments.Count,
                            centerX: centerX,
                            centerY: centerY,
                            radius: radius,
                            stroke: OutterGridLineBrush ?? GridLinesBrush,
                            strokThickness: OutterGridLineThickness ?? GridLinesThickness,
                            fill: null,
                            dashArray: null
                        );
                    }
                    else
                    {
                        DrawPolygon(
                            drawingContext: drawingContext,
                            sides: segments.Count,
                            centerX: centerX,
                            centerY: centerY,
                            radius: (gridLinesIndex + 1) * gridLinesSpacing,
                            stroke: GridLinesBrush,
                            strokThickness: GridLinesThickness,
                            fill: null,
                            dashArray: GridLinesDashArray
                        );
                    }
                }

            }
            #endregion

            #region Axis
            if (AxisStroke != null
                && AxisStrokeThickness > 0)
            {
                foreach (var segmentInfo in _segmentInfos)
                {
                    var segment = segmentInfo.Key;
                    var angle = segmentInfo.Value.Angle;
                    var radian = (angle - 90) * Math.PI / 180.0;
                    var x = centerX + radius * Math.Cos(radian);
                    var y = centerY + radius * Math.Sin(radian);

                    drawingContext.DrawLine(
                        stroke: segment.AxisStroke,
                        strokeThickness: segment.AxisStrokeThickness,
                        startPoint: new Point(centerX, centerY),
                        endPoint: new Point(x, y));
                }
            }
            #endregion

            #region RadarArea
            {
                var points = new List<Point>();

                foreach (var segmentInfo in _segmentInfos)
                {
                    var segment = segmentInfo.Key;
                    var angle = segmentInfo.Value.Angle;
                    var radian = (angle - 90) * Math.PI / 180.0;
                    var percent = segmentInfo.Value.Percent;

                    var x = centerX + radius * percent * Math.Cos(radian);
                    var y = centerY + radius * percent * Math.Sin(radian);

                    points.Add(new Point(x, y));
                }

                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(points[0], true, true);

                    for (int i = 1; i < points.Count; i++)
                    {
                        ctx.LineTo(points[i], true, false);
                    }

                    ctx.LineTo(points[0], true, false);
                }

                drawingContext.DrawGeometry(
                    stroke: Stroke,
                    strokeThickness: StrokeThickness,
                    fill: Fill,
                    geometry
                );
            }
            #endregion

            #region Text
            {
                foreach (var segmentInfo in _segmentInfos)
                {
                    var segment = segmentInfo.Key;
                    var formattedText = segmentInfo.Value.Label;
                    var angle = segmentInfo.Value.Angle;
                    var radian = (angle - 90) * Math.PI / 180.0;
                    var rayLength = CalculateRayLength(formattedText.Width, formattedText.Height, angle + 90);

                    var halfPoint = new Point(
                        centerX + (radius + chartContext.Chart.LabelSpacing + rayLength) * Math.Cos(radian),
                        centerY + (radius + chartContext.Chart.LabelSpacing + rayLength) * Math.Sin(radian));

                    if (segment.LabelStroke == null && segment.LabelForeground == null)
                    {
                        drawingContext.DrawText(
                            formattedText,
                            new Point(halfPoint.X, halfPoint.Y - formattedText.Height / 2));
                    }
                    else
                    {
                        drawingContext.DrawText(
                            formattedText,
                            startPoint: new Point(halfPoint.X, halfPoint.Y - formattedText.Height / 2),
                            fill: segment.LabelForeground,
                            stroke: segment.LabelStroke,
                            strokeThickness: segment.LabelStrokeThickness);
                    }
                }
            }
            #endregion
        }
        #endregion

        #endregion

        #region Event Handlers
        public static void OnToggleHighlighting(
            ToggleHighlightLayer layer,
            RadarSeries series,
            IDrawingContext drawingContext,
            IRadialChartContext chartContext,
            IDictionary<int, double> coordinatesProgress
        )
        {

        }
        #endregion

        #region Functions
        private static void DrawPolygon(
            IDrawingContext drawingContext,
            int sides,
            double centerX,
            double centerY,
            double radius,
            Brush stroke,
            double strokThickness,
            Brush fill,
            DoubleCollection dashArray
        )
        {
            var angleOffset = -Math.PI / 2;
            var angleIncrement = 2 * Math.PI / sides;

            var points = new Point[sides];

            for (int i = 0; i < sides; i++)
            {
                var angle = i * angleIncrement + angleOffset;
                var x = centerX + radius * Math.Cos(angle);
                var y = centerY + radius * Math.Sin(angle);
                points[i] = new Point(x, y);
            }

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(points[0], true, true);

                for (int i = 1; i < points.Length; i++)
                {
                    ctx.LineTo(points[i], true, false);
                }

                ctx.LineTo(points[0], true, false);
            }

            drawingContext.DrawGeometry(
                stroke, 
                strokThickness, 
                fill, 
                geometry,
                dashArray);
        }

        private void CheckMinMaxValue(
            decimal minValue,
            decimal maxValue,
            out decimal resultMin,
            out decimal resultMax
        )
        {
            var min = (int)Math.Floor(minValue);
            var max = (int)Math.Ceiling(maxValue);

            var digit = Math.Max(1, max.ToString().Length - 1);
            var baseValue = Math.Pow(10d, digit);

            resultMin = (int)Math.Floor(min / baseValue) * (int)baseValue;
            resultMax = (int)Math.Ceiling(max / baseValue) * (int)baseValue;
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
