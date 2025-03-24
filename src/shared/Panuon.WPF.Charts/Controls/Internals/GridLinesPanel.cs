using System.Diagnostics;
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
            ClipToBounds = true;
        }
        #endregion

        #region Properties
       
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
            var chartContext = _chart.GetCanvasContext() as ICartesianChartContext;

            var gridLinesBrush = _chart.GridLinesBrush;
            var gridLinesThickness = _chart.GridLinesThickness;
            var gridLinesDashArray = _chart.GridLinesDashArray;

            if (_chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Vertical
                || _chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Both)
            {
                if (!_chart.SwapXYAxes)
                {
                    if (_chart.XAxis != null)
                    {
                        foreach (var coordinateText in _chart.XAxis._labelOffsets)
                        {
                            var offsetX = coordinateText.Item2();

                            drawingContext.DrawLine(
                                gridLinesBrush,
                                gridLinesThickness,
                                new Point(offsetX, 0),
                                new Point(offsetX, ActualHeight),
                                dashArray: gridLinesDashArray);

                            if (_chart.Coordinates.Count == 5)
                            {
                                Debug.WriteLine(offsetX);
                            }
                            
                        }
                    }
                }
                else
                {
                    if (_chart.YAxis != null)
                    {
                        foreach (var valueText in _chart.XAxis._labelOffsets)
                        {
                            var offsetY = valueText.Item2();

                            drawingContext.DrawLine(
                                gridLinesBrush,
                                gridLinesThickness,
                                new Point(0, offsetY),
                                new Point(ActualWidth, offsetY),
                                dashArray: gridLinesDashArray);
                        }
                    }
                }
            }
            if (_chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Horizontal
                || _chart.GridLinesVisibility == CartesianChartGridLinesVisibility.Both)
            {
                if (!_chart.SwapXYAxes)
                {
                    if (_chart.YAxis != null)
                    {
                        foreach (var labelOffset in _chart.YAxis._labelOffsets)
                        {
                            var value = labelOffset.Value;
                            var offsetY = labelOffset.GetOffsetY();

                            var stroke = gridLinesBrush;
                            var strokeThickness = (double?)gridLinesThickness;
                            var dashArray = gridLinesDashArray;

                            _chart.RaiseDrawingHorizontalGridLine(value,
                                ref stroke,
                                ref strokeThickness,
                                ref dashArray);

                            if (stroke != null && strokeThickness != null)
                            {
                                drawingContext.DrawLine(
                                    stroke,
                                    (double)strokeThickness,
                                    new Point(0, offsetY),
                                    new Point(ActualWidth, offsetY),
                                    dashArray);
                            }
                        }
                    }
                }
                else
                {
                    if (_chart.XAxis != null)
                    {
                        foreach (var labelOffset in _chart.YAxis._labelOffsets)
                        {
                            var value = labelOffset.Value;
                            var offsetX = labelOffset.GetOffsetY();

                            var stroke = gridLinesBrush;
                            var strokeThickness = (double?)gridLinesThickness;
                            var dashArray = gridLinesDashArray;

                            _chart.RaiseDrawingHorizontalGridLine(value,
                                ref stroke,
                                ref strokeThickness,
                                ref dashArray);

                            drawingContext.DrawLine(
                                gridLinesBrush,
                                gridLinesThickness,
                                new Point(offsetX, 0),
                                new Point(offsetX, ActualHeight),
                                dashArray);
                        }
                    }
                }
            } 
        }

        #endregion
    }
}
