using System;
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
        internal void Render(IDrawingContext drawingContext,
            IChartContext chartContext,
            IEnumerable<ICoordinate> coordinates)
        {
            OnRendering(drawingContext, chartContext, coordinates);
        }

        internal void Highlight(ICoordinate coordinate,
            IDrawingContext drawingContext,
            IChartContext chartContext)
        {
            OnHighlighting(coordinate, drawingContext, chartContext);
        }
        #endregion

        #region Abstract Methods

        protected abstract void OnRendering(IDrawingContext drawingContext,
            IChartContext chartContext,
            IEnumerable<ICoordinate> coordinates);

        protected abstract void OnHighlighting(ICoordinate coordinate,
            IDrawingContext drawingContext,
            IChartContext chartContext);
        #endregion

        #endregion
    }
}