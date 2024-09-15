using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class SeriesBase
        : DependencyObject
    {
        #region Properties

        #endregion

        #region Internal Events
        internal delegate void OnInvalidRender();

        internal event OnInvalidRender InternalInvalidRender;
        #endregion

        #region Methods

        #region Protected Methods
        protected static void OnRenderPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var series = (SeriesBase)d;
            series.InvalidRender();
        }

        protected void InvalidRender()
        {
            InternalInvalidRender?.Invoke();
        }
        #endregion

        #region Internal Methods
        internal void BeginRender(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            OnRenderBegin(
                drawingContext,
                chartContext
            );
        }

        internal void Render(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        )
        {
            OnRendering(
                drawingContext: drawingContext,
                chartContext: chartContext,
                animationProgress: animationProgress
            );
        }

        internal void CompleteRender(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
            OnRenderCompleted(
                drawingContext: drawingContext,
                chartContext: chartContext
            );
        }

        internal void Highlight(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips
        )
        {
            OnHighlighting(drawingContext,
                chartContext,
                layerContext,
                in tooltips);
        }
        #endregion

        #region Abstract Methods
        protected virtual void OnRenderBegin(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {

        }

        /// <summary>
        /// Call this method during rendering chart.
        /// </summary>
        /// <param name="drawingContext">DrawingContext.</param>
        /// <param name="chartContext">ChartContext</param>
        /// <param name="animationProgress">Animation progress. From 0 to 1.</param>
        protected abstract void OnRendering(
            IDrawingContext drawingContext,
            IChartContext chartContext,
            double animationProgress
        );

        protected virtual void OnRenderCompleted(
            IDrawingContext drawingContext,
            IChartContext chartContext
        )
        {
        }

        protected abstract void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips);
        #endregion

        #endregion
    }

}