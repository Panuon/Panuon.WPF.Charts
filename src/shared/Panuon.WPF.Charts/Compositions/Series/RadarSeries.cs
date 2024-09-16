using System;
using System.Collections.Generic;

namespace Panuon.WPF.Charts
{
    public class RadarSeries
        : ValueProviderSegmentsSeriesBase<PieSeriesSegment>
    {
        #region Methods

        #region OnHighlighting
        protected override void OnHighlighting(IDrawingContext drawingContext, IChartContext chartContext, ILayerContext layerContext, in IList<SeriesTooltip> tooltips)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region OnRendering
        protected override void OnRendering(IDrawingContext drawingContext, IChartContext chartContext, double animationProgress)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}
