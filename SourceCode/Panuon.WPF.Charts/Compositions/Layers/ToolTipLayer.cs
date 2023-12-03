using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ToolTipLayer
        : LayerBase
    {
        protected override void OnMouseIn(IChartContext chartContext, ILayerContext layerContext)
        {
            InvalidRender();
        }

        protected override void OnMouseOut(IChartContext chartContext, ILayerContext layerContext)
        {
            InvalidRender();
        }

        protected override void OnRender(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext)
        {
            if (layerContext.GetMousePosition() is Point mousePosition)
            {
                var coordinate = layerContext.GetCoordinate(mousePosition.X);
                if (coordinate != null)
                {
                    foreach (var series in chartContext.Series)
                    {
                        series.Highlight(coordinate, drawingContext, chartContext);
                    }
                }
            }
        }
    }
}