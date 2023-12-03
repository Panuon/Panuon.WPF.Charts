﻿using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class CrossLineLayer
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
                    drawingContext.DrawLine(Brushes.Gray, 1, coordinate.Offset, 0, coordinate.Offset, canvasContext.AreaHeight);
                    drawingContext.DrawLine(Brushes.Gray, 1, 0, mousePosition.Y, canvasContext.AreaWidth, mousePosition.Y);
                }
            }
        }
    }
}