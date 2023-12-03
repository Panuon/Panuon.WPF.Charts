using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ToolTipLayer
        : LayerBase
    {
        protected override void OnMouseIn(ICanvasContext canvasContext, ILayerContext layerContext)
        {
            InvalidRender();
        }

        protected override void OnMouseOut(ICanvasContext canvasContext, ILayerContext layerContext)
        {
            InvalidRender();
        }

        protected override void OnRender(IDrawingContext drawingContext,
            ICanvasContext canvasContext,
            ILayerContext layerContext)
        {
            if (layerContext.GetMousePosition() is Point mousePosition)
            {
                var coordinate = layerContext.GetCoordinate(mousePosition.X);
                if (coordinate != null)
                {
                    var seriesValues = layerContext.GetSeriesValue(coordinate.Index);
                    if (seriesValues != null)
                    {
                        foreach(var seriesValue in seriesValues )
                        {
                            var series = seriesValue.Key;
                            var value = seriesValue.Value;

                            var offsetY = canvasContext.GetOffset(value);

                            if(series is LineSeries lineSeries)
                            {
                            drawingContext.DrawEllipse(null, 0, lineSeries.Stroke, 5, 5, coordinate.Offset, offsetY);
                            }
                        }
                    }
                }
            }
        }
    }
}