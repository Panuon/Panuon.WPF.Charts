using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class RadarSeries
        : RadialValueProviderSegmentsSeriesBase<RadarSeriesSegment>
    {
        #region Properties

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(RadarSeriesSegment), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A000000")), OnRenderPropertyChanged));
        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(RadarSeriesSegment), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RadarSeriesSegment), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region GridLinesVisibility
        public RadialChartGridLinesVisibility GridLinesVisibility
        {
            get { return (RadialChartGridLinesVisibility)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(RadialChartGridLinesVisibility), typeof(RadarSeries), new PropertyMetadata(RadialChartGridLinesVisibility.Visible, OnRenderPropertyChanged));
        #endregion

        #region GridLinesBrush
        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register("GridLinesBrush", typeof(Brush), typeof(RadarSeries), new PropertyMetadata(Brushes.LightGray, OnRenderPropertyChanged));
        #endregion

        #region GridLinesThickness
        public double GridLinesThickness
        {
            get { return (double)GetValue(GridLinesThicknessProperty); }
            set { SetValue(GridLinesThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLinesThicknessProperty =
            DependencyProperty.Register("GridLinesThickness", typeof(double), typeof(RadarSeries), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region OutterGridLineBrush
        public Brush OutterGridLineBrush
        {
            get { return (Brush)GetValue(OutterGridLineBrushProperty); }
            set { SetValue(OutterGridLineBrushProperty, value); }
        }

        public static readonly DependencyProperty OutterGridLineBrushProperty =
            DependencyProperty.Register("OutterGridLineBrush", typeof(Brush), typeof(RadarSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region OutterGridLineThickness
        public double? OutterGridLineThickness
        {
            get { return (double?)GetValue(OutterGridLineThicknessProperty); }
            set { SetValue(OutterGridLineThicknessProperty, value); }
        }

        public static readonly DependencyProperty OutterGridLineThicknessProperty =
            DependencyProperty.Register("OutterGridLineThickness", typeof(double?), typeof(RadarSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region GridLinesSpacing
        public GridLength GridLinesSpacing
        {
            get { return (GridLength)GetValue(GridLinesSpacingProperty); }
            set { SetValue(GridLinesSpacingProperty, value); }
        }

        public static readonly DependencyProperty GridLinesSpacingProperty =
            DependencyProperty.Register("GridLinesSpacing", typeof(GridLength), typeof(RadarSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto), OnRenderPropertyChanged));
        #endregion

        #region AxisStroke
        public Brush AxisStroke
        {
            get { return (Brush)GetValue(AxisStrokeProperty); }
            set { SetValue(AxisStrokeProperty, value); }
        }

        public static readonly DependencyProperty AxisStrokeProperty =
            DependencyProperty.Register("AxisStroke", typeof(Brush), typeof(RadarSeries), new PropertyMetadata(Brushes.LightGray, OnRenderPropertyChanged));
        #endregion

        #region AxisStrokeThickness
        public double AxisStrokeThickness
        {
            get { return (double)GetValue(AxisStrokeThicknessProperty); }
            set { SetValue(AxisStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty AxisStrokeThicknessProperty =
            DependencyProperty.Register("AxisStrokeThickness", typeof(double), typeof(RadarSeries), new PropertyMetadata(2d, OnRenderPropertyChanged));
        #endregion

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RadarSeries), new PropertyMetadata(0d));
        #endregion

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RadarSeries), new PropertyMetadata(10d));
        #endregion

        #endregion

        #region Methods

        #region OnHighlighting
        protected override void OnHighlighting(IDrawingContext drawingContext, IChartContext chartContext, ILayerContext layerContext, in IList<SeriesTooltip> tooltips)
        {
        }
        #endregion

        #region OnRendering
        protected override void OnRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        )
        {
            var segments = Segments;
            if (segments.Count < 3)
            {
                throw new InvalidOperationException("RadarSeries requires at least 3 segments to be formed.");
            }

            var chartPanel = chartContext.ChartPanel;
            var coordinates = chartContext.Coordinates;

            var centerX = chartContext.AreaWidth / 2;
            var centerY = chartContext.AreaHeight / 2;
            var radius = Math.Min(chartContext.AreaWidth, chartContext.AreaHeight) / 2;

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
                            centerX: chartContext.AreaWidth / 2,
                            centerY: chartContext.AreaHeight / 2,
                            radius: radius,
                            stroke: OutterGridLineBrush ?? GridLinesBrush,
                            strokThickness: OutterGridLineThickness ?? GridLinesThickness,
                            fill: null
                        );
                    }
                    else
                    {
                        DrawPolygon(
                            drawingContext: drawingContext,
                            sides: segments.Count,
                            centerX: chartContext.AreaWidth / 2,
                            centerY: chartContext.AreaHeight / 2,
                            radius: (gridLinesIndex + 1) * gridLinesSpacing,
                            stroke: GridLinesBrush,
                            strokThickness: GridLinesThickness,
                            fill: null
                        );
                    }
                }

            }
            #endregion

            #region Axis
            if (AxisStroke != null
                && AxisStrokeThickness > 1)
            {
                var angleOffset = -Math.PI / 2;
                var angleIncrement = 2 * Math.PI / segments.Count;
                for (int stickIndex = 0; stickIndex < segments.Count; stickIndex++)
                {
                    var angle = stickIndex * angleIncrement + angleOffset;
                    var x = centerX + radius * Math.Cos(angle);
                    var y = centerY + radius * Math.Sin(angle);

                    drawingContext.DrawLine(
                        stroke: AxisStroke,
                        strokeThickness: AxisStrokeThickness,
                        startX: centerX,
                        startY: centerY,
                        endX: x,
                        endY: y
                    );
                }
            }
            #endregion

            #region RadarArea
            {
                var angleOffset = -Math.PI / 2;
                var angleIncrement = 2 * Math.PI / segments.Count;
                var points = new List<Point>();

                for (int stickIndex = 0; stickIndex < segments.Count; stickIndex++)
                {
                    var segment = Segments[stickIndex];
                    var angle = stickIndex * angleIncrement + angleOffset;

                    var coordinate = coordinates.ElementAt(stickIndex);
                    var value = coordinate.GetValue(this);
                    var percent = value / Maximum;

                    var x = centerX + radius * percent * Math.Cos(angle);
                    var y = centerY + radius * percent * Math.Sin(angle);

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
        }
        #endregion

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
            Brush fill
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

            drawingContext.DrawGeometry(stroke, strokThickness, fill, geometry);
        }
        #endregion
    }
}
