using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public delegate void SeriesHighlightHandler<TLayer, TSeries, TChartContext>(
        TLayer layer,
        TSeries series,
        IDrawingContext drawingContext,
        TChartContext chartContext,
        IDictionary<int, double> coordinatesProgress
    )
        where TLayer : LayerBase
        where TSeries : SeriesBase
        where TChartContext: IChartContext;

    public delegate void SeriesHighlightHandler(
        LayerBase layer,
        SeriesBase series,
        IDrawingContext drawingContext,
        IChartContext chartContext,
        IDictionary<int, double> coordinatesProgress
    );

}
