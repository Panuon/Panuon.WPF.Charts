using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class CartesianSeriesBase
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
                (ICartesianChartContext)chartContext
            );
        }

        protected virtual void OnRenderBegin(
            IDrawingContext drawingContext,
            ICartesianChartContext chartContext
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
                (ICartesianChartContext)chartContext,
                animationProgress
            );
        }

        protected abstract void OnRendering(
            IDrawingContext drawingContext,
            ICartesianChartContext chartContext,
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
                (ICartesianChartContext)chartContext
            );
        }

        protected virtual void OnRenderCompleted(
            IDrawingContext drawingContext,
            ICartesianChartContext chartContext
        )
        {
        }
        #endregion

        #region OnRetrieveLegendEntries
        protected internal sealed override IEnumerable<SeriesLegendEntry> OnInternalRetrieveLegendEntries()
        {
            return OnRetrieveLegendEntries();
        }

        protected abstract IEnumerable<SeriesLegendEntry> OnRetrieveLegendEntries();
        #endregion

        #endregion
    }
}
