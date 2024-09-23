using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public delegate void SeriesHighlightHandler<TLayer, TSeries>(
        TLayer layer,
        TSeries series,
        IDrawingContext drawingContext,
        IChartContext chartContext,
        ILayerContext layerContext,
        IDictionary<int, double> coordinatesProgress
    )
        where TLayer : LayerBase
        where TSeries : SeriesBase;

    public delegate void SeriesHighlightHandler(
        LayerBase layer,
        SeriesBase series,
        IDrawingContext drawingContext,
        IChartContext chartContext,
        ILayerContext layerContext,
        IDictionary<int, double> coordinatesProgress
    );

}
