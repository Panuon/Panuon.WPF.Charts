using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class GridLinesPanel
        : FrameworkElement
    {
        #region Fields
        private ChartPanel _chartPanel;
        #endregion

        #region Ctor
        internal GridLinesPanel(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
        }
        #endregion

        #region Properties
       
        #region GridLinesVisibility
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(ChartPanelGridLinesVisibility), typeof(GridLinesPanel), new FrameworkPropertyMetadata(ChartPanelGridLinesVisibility.Both,
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
            if (_chartPanel.GridLinesBrush == null ||
                _chartPanel.GridLinesThickness == 0 ||
                !_chartPanel.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chartPanel.CreateDrawingContext(context);
            var canvasContext = _chartPanel.CreateCanvasContext();

            var pen = new Pen(_chartPanel.GridLinesBrush, _chartPanel.GridLinesThickness);
            pen.Freeze();

            if (_chartPanel.GridLinesVisibility == ChartPanelGridLinesVisibility.Vertical
                || _chartPanel.GridLinesVisibility == ChartPanelGridLinesVisibility.Both)
            {
                foreach(var coordinateText in _chartPanel._xAxisPresenter._formattedTexts)
                {
                    var coordinate = coordinateText.Key;

                    var offsetX = canvasContext.GetOffsetX(coordinate.Index);

                    drawingContext.DrawLine(_chartPanel.GridLinesBrush,
                        _chartPanel.GridLinesThickness,
                        offsetX,
                        0,
                        offsetX,
                        ActualHeight);
                }
            }
            if (_chartPanel.GridLinesVisibility == ChartPanelGridLinesVisibility.Horizontal
                || _chartPanel.GridLinesVisibility == ChartPanelGridLinesVisibility.Both)
            {
                foreach (var valueText in _chartPanel._yAxisPresenter._formattedTexts)
                {
                    var value = valueText.Key;

                    var offsetY = canvasContext.GetOffsetY(value);

                    drawingContext.DrawLine(_chartPanel.GridLinesBrush,
                        _chartPanel.GridLinesThickness,
                        0,
                        offsetY,
                        ActualWidth,
                        offsetY);
                }
            }
        }

        #endregion
    }
}
