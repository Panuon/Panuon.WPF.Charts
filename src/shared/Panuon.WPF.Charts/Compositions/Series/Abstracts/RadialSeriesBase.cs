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
