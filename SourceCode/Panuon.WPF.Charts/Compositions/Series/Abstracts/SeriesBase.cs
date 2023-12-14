using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Media;

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
        internal void Render(IDrawingContext drawingContext,
            IChartContext chartContext)
        {
            OnRendering(drawingContext, chartContext);
        }

        internal void Highlight(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips)
        {
            OnHighlighting(drawingContext,
                chartContext,
                layerContext,
                in tooltips);
        }
        #endregion

        #region Abstract Methods

        protected abstract void OnRendering(IDrawingContext drawingContext,
            IChartContext chartContext);

        protected abstract void OnHighlighting(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext,
            in IList<SeriesTooltip> tooltips);
        #endregion

        #endregion
    }

}