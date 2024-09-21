using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class GridLinesPanel
        : FrameworkElement
    {
        #region Fields
        private CartesianChart _chart;
        #endregion

        #region Ctor
        internal GridLinesPanel(CartesianChart chart)
        {
            _chart = chart;
        }
        #endregion

        #region Properties
       
        #region GridLinesVisibility
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(CartesianChartGridLinesVisibility), typeof(GridLinesPanel), new FrameworkPropertyMetadata(CartesianChartGridLinesVisibility.Both,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region GridLinesBrush
        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register("GridLinesBrush", typeof(Brush), typeof(GridLinesPanel), new FrameworkPropertyMetadata(Brushes.LightGray,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region GridLinesThickness
        public static readonly DependencyProperty GridLinesThicknessProperty =
            DependencyProperty.Register("GridLinesThickness", typeof(double), typeof(GridLinesPanel), new FrameworkPropertyMetadata(1d,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Overrides
        protected override void OnRender(DrawingContext context)
        {
            if (_chart.GridLinesBrush == null ||
                _chart.GridLinesThickness == 0 ||
                !_chart.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chart.CreateDrawingContext(context);
            var chartContext = _chart.GetCanvasContext();

            var pen = new Pen(_chart.GridLinesBrush, _chart.GridLinesThickness);
            pen.Freeze();

            if (_chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Vertical
                || _chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Both)
            {
                if (_chart.XAxis != null)
                {
                    foreach (var coordinateText in _chart.XAxis._formattedTexts)
                    {
                        var coordinate = coordinateText.Key;

                        var offsetX = coordinate.Offset;

                        drawingContext.DrawLine(_chart.GridLinesBrush,
                            _chart.GridLinesThickness,
                            offsetX,
                            0,
                            offsetX,
                            ActualHeight);
                    }
                }
            }
            if (_chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Horizontal
                || _chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Both)
            {
                if (_chart.YAxis != null)
                {
                    foreach (var valueText in _chart.YAxis._formattedTexts)
                    {
                        var value = valueText.Key;

                        var offsetY = chartContext.GetOffsetY(value);

                        drawingContext.DrawLine(_chart.GridLinesBrush,
                            _chart.GridLinesThickness,
                            0,
                            offsetY,
                            ActualWidth,
                            offsetY);
                    }
                }
            }
        }

        #endregion
    }
}
