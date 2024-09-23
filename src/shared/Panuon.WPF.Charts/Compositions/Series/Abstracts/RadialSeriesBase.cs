using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class RadialSeriesBase
        : SeriesBase
    {
        #region Methods

        #region OnRenderBegin
        protected internal sealed override void OnInternalRenderBegin(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            OnRenderBegin(
                drawingContext,
                (IRadialChartContext)chartContext
            );
        }

        protected virtual void OnRenderBegin(
            IDrawingContext drawingContext,
            IRadialChartContext chartContext
        )
        {
        }
        #endregion

        #region OnRendering
        protected internal sealed override void OnInternalRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        )
        {
            OnRendering(
                drawingContext,
                (IRadialChartContext)chartContext,
                animationProgress
            );
        }

        protected abstract void OnRendering(
            IDrawingContext drawingContext,
            IRadialChartContext chartContext,
            double animationProgress
        );
        #endregion

        #region OnRenderCompleted
        protected internal override void OnInternalRenderCompleted(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            OnRenderCompleted(
                drawingContext,
                (IRadialChartContext)chartContext
            );
        }

        protected virtual void OnRenderCompleted(
            IDrawingContext drawingContext,
            IRadialChartContext chartContext
        )
        {
        }
        #endregion

        #region OnRetrieveCoordinate
        protected internal sealed override ICoordinate OnInternalRetrieveCoordinate(
            IChartContext chartContext,
            ILayerContext layerContext,
            Point position
        )
        {
            return OnRetrieveCoordinate(
                (IRadialChartContext)chartContext,
                layerContext,
                position
            );
        }

        protected abstract ICoordinate OnRetrieveCoordinate(
            IRadialChartContext chartContext,
            ILayerContext layerContext,
            Point position
        );
        #endregion

        #region OnRetrieveLegendEntries
        protected internal sealed override IEnumerable<SeriesLegendEntry> OnInternalRetrieveLegendEntries(
            IChartContext chartContext,
            ILayerContext layerContext
        )
        {
            return OnRetrieveLegendEntries(
                (IRadialChartContext)chartContext,
                layerContext
            );
        }

        protected abstract IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries(
            IRadialChartContext chartContext,
            ILayerContext layerContext
        );
        #endregion

        #endregion
    }
}
