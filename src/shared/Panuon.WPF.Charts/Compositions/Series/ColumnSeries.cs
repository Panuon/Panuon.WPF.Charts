using Panuon.WPF.Charts.Utils;
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
        private List<Point> _valuePoints;
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
            var delta = chartContext.SwapXYAxes
                ? chartContext.AreaHeight / chartContext.Coordinates.Count()
                : chartContext.AreaWidth / chartContext.Coordinates.Count();
            var columnSize = chartContext.SwapXYAxes
                ? GridLengthUtil.GetActualValue(ColumnWidth, delta)
                : GridLengthUtil.GetActualValue(ColumnWidth, delta);

            foreach (var valuePoint in _valuePoints)
            {
                var offsetX = valuePoint.X;
                var offsetY = valuePoint.Y;

                if (!chartContext.SwapXYAxes)
                {
                    if (BackgroundFill != null)
                    {
                        drawingContext.DrawRectangle(
                            stroke: null,
                            strokeThickness: 0,
                            fill: BackgroundFill,
                            startX: offsetX - columnSize / 2,
                            startY: 0,
                            width: columnSize,
                            height: chartContext.AreaHeight,
                            radiusX: Radius,
                            radiusY: Radius
                        );
                    }

                    drawingContext.DrawRectangle(
                        stroke: Stroke,
                        strokeThickness: StrokeThickness,
                        fill: Fill,
                        startX: offsetX - columnSize / 2,
                        startY: chartContext.AreaHeight - (chartContext.AreaHeight - offsetY) * animationProgress,
                        width: columnSize,
                        height: (chartContext.AreaHeight - offsetY) * animationProgress,
                        radiusX: Radius,
                        radiusY: Radius
                    );
                }
                else
                {
                    if (BackgroundFill != null)
                    {
                        drawingContext.DrawRectangle(
                            stroke: null,
                            strokeThickness: 0,
                            fill: BackgroundFill,
                            startX: 0,
                            startY: offsetY - columnSize / 2,
                            width: offsetX,
                            height: columnSize,
                            radiusX: Radius,
                            radiusY: Radius
                        );
                    }

                    drawingContext.DrawRectangle(
                        stroke: Stroke,
                        strokeThickness: StrokeThickness,
                        fill: Fill,
                        startX: 0,
                        startY: offsetY - columnSize / 2,
                        width: offsetX * animationProgress,
                        height: columnSize,
                        radiusX: Radius,
                        radiusY: Radius
                    );
                }
            }
        }
        #endregion

        #region OnHighlighting
        protected override IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries(
            ICartesianChartContext chartContext
        )
        {
            if (chartContext.GetMousePosition(MouseRelativeTarget.Layer) is Point offset)
            {
                var coordinate = chartContext.RetrieveCoordinate(offset);

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffsetY(value);
                yield return new SeriesLegendEntry(Fill, Title ?? coordinate.Title, value.ToString());
            }
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
                var point = series._valuePoints[coordinate.Index];

                if (!chartContext.SwapXYAxes)
                {
                    drawingContext.DrawEllipse(
                        stroke: series.Fill,
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
                        stroke: series.Fill,
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
        #endregion
    }
}