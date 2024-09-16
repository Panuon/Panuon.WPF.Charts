using Panuon.WPF.Charts.Controls.Internals;
using Panuon.WPF.Charts.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class RadarSeries
        : ValueProviderSegmentsSeriesBase<RadarSeriesSegment>
    {
        #region Properties

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(RadarSeriesSegment), new PropertyMetadata(Brushes.Black, OnRenderPropertyChanged));
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

        #region OutterGridLineBrush
        public Brush OutterGridLineBrush
        {
            get { return (Brush)GetValue(OutterGridLineBrushProperty); }
            set { SetValue(OutterGridLineBrushProperty, value); }
        }

        public static readonly DependencyProperty OutterGridLineBrushProperty =
            DependencyProperty.Register("OutterGridLineBrush", typeof(Brush), typeof(RadarSeriesSegment), new PropertyMetadata(null));
        #endregion

        #region OutterGridLineThickness
        public double? OutterGridLineThickness
        {
            get { return (double?)GetValue(OutterGridLineThicknessProperty); }
            set { SetValue(OutterGridLineThicknessProperty, value); }
        }

        public static readonly DependencyProperty OutterGridLineThicknessProperty =
            DependencyProperty.Register("OutterGridLineThickness", typeof(double?), typeof(RadarSeries), new PropertyMetadata(null));
        #endregion

        #region GridLinesSpacing
        public GridLength GridLinesSpacing
        {
            get { return (GridLength)GetValue(GridLinesSpacingProperty); }
            set { SetValue(GridLinesSpacingProperty, value); }
        }

        public static readonly DependencyProperty GridLinesSpacingProperty =
            DependencyProperty.Register("GridLinesSpacing", typeof(GridLength), typeof(RadarSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto)));
        #endregion

        #region AxisStroke
        public Brush AxisStroke
        {
            get { return (Brush)GetValue(AxisStrokeProperty); }
            set { SetValue(AxisStrokeProperty, value); }
        }

        public static readonly DependencyProperty AxisStrokeProperty =
            DependencyProperty.Register("AxisStroke", typeof(Brush), typeof(RadarSeries), new PropertyMetadata(0));
        #endregion

        #endregion

        #region Methods

        #region OnHighlighting
        protected override void OnHighlighting(IDrawingContext drawingContext, IChartContext chartContext, ILayerContext layerContext, in IList<SeriesTooltip> tooltips)
        {
        }
        #endregion

        #region OnRendering
        protected override void OnRendering(IDrawingContext drawingContext, IChartContext chartContext, double animationProgress)
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
                        stroke: OutterGridLineBrush ?? chartContext.ChartPanel.GridLinesBrush,
                        strokThickness: OutterGridLineThickness ?? chartContext.ChartPanel.GridLinesThickness,
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
                        stroke: chartContext.ChartPanel.GridLinesBrush,
                        strokThickness: chartContext.ChartPanel.GridLinesThickness,
                        fill: null
                    );
                }
            }

            var angleOffset = -Math.PI / 2;
            var angleIncrement = 2 * Math.PI / segments.Count;
            for (int stickIndex = 0; stickIndex < segments.Count; stickIndex++)
            {
                var angle = stickIndex * angleIncrement + angleOffset;
                var x = centerX + radius * Math.Cos(angle);
                var y = centerY + radius * Math.Sin(angle);
                drawingContext.DrawLine(
                    gridline;
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
