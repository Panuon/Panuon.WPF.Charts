using Panuon.WPF.Charts.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ColumnSeries
        : ValueProviderSeriesBase
    {
        #region Fields
        private List<Point> _valuePoints;
        #endregion

        #region Properties

        #region BackgroundFill
        public Brush BackgroundFill
        {
            get { return (Brush)GetValue(BackgroundFillProperty); }
            set { SetValue(BackgroundFillProperty, value); }
        }

        public static readonly DependencyProperty BackgroundFillProperty =
            DependencyProperty.Register("BackgroundFill", typeof(Brush), typeof(ColumnSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ColumnSeries), new PropertyMetadata(Brushes.Black, OnRenderPropertyChanged));

        #endregion

        #region Stroke
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ColumnSeries), new PropertyMetadata(null, OnRenderPropertyChanged));
        #endregion

        #region StrokeThickness
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ColumnSeries), new PropertyMetadata(1d, OnRenderPropertyChanged));
        #endregion

        #region ColumnWidth
        public GridLength ColumnWidth
        {
            get { return (GridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(GridLength), typeof(ColumnSeries), new PropertyMetadata(new GridLength(1, GridUnitType.Auto), OnRenderPropertyChanged));
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
            var deltaX = chartContext.AreaWidth / chartContext.Coordinates.Count();
            var columnWidth = GridLengthUtil.GetActualValue(ColumnWidth, deltaX);

            foreach (var valuePoint in _valuePoints)
            {
                var offsetX = valuePoint.X;
                var offsetY = valuePoint.Y;

                if (BackgroundFill != null)
                {
                    drawingContext.DrawRectangle(
                        stroke: null,
                        strokeThickness: 0,
                        fill: BackgroundFill,
                        startX: offsetX - columnWidth / 2,
                        startY: 0,
                        width: columnWidth,
                        height: chartContext.AreaHeight
                    );
                }

                drawingContext.DrawRectangle(
                       stroke: Stroke,
                       strokeThickness: StrokeThickness,
                       fill: Fill,
                       startX: offsetX - columnWidth / 2,
                       startY: chartContext.AreaHeight - (chartContext.AreaHeight - offsetY) * animationProgress,
                       width: columnWidth,
                       height: (chartContext.AreaHeight - offsetY) * animationProgress
                   );
            }
        }
        #endregion

        #region OnHighlighting
        protected override void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips)
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                var coordinate = layerContext.GetCoordinate(position.X);

                var value = coordinate.GetValue(this);
                var offsetY = chartContext.GetOffsetY(value);
                drawingContext.DrawEllipse(Fill, 2, Brushes.White, 5, 5, coordinate.Offset, offsetY);
                tooltips.Add(new SeriesTooltip(Fill, Title ?? coordinate.Title, value.ToString()));
            }

        }
        #endregion

        #endregion
    }
}