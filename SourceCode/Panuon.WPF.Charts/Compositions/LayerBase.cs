using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class LayerBase
        : DependencyObject
    {
        #region Ctor
        #endregion

        #region Internal Methods
        internal delegate void OnAddChild(UIElement child);

        internal event OnAddChild InternalAddChild;

        internal delegate void OnRemoveChild(UIElement child);

        internal event OnRemoveChild InternalRemoveChild;

        internal delegate void OnInvalidRender();

        internal event OnInvalidRender InternalInvalidRender;
        #endregion

        #region Internal Methods
        internal void Render(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext)
        {
            OnRender(drawingContext, chartContext, layerContext);
        }

        internal void MouseIn(IChartContext chartContext,
            ILayerContext layerContext)
        {
            OnMouseIn(chartContext, layerContext);
        }

        internal void MouseOut(IChartContext chartContext,
            ILayerContext layerContext)
        {
            OnMouseOut(chartContext, layerContext);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnMouseIn(IChartContext chartContext,
            ILayerContext layerContext)
        {

        }

        protected virtual void OnMouseOut(IChartContext chartContext,
            ILayerContext layerContext)
        {

        }

        protected virtual void OnRender(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext) { }

        protected void AddChild(UIElement child)
        {
            InternalAddChild?.Invoke(child);
        }

        protected void RemoveChild(UIElement child)
        {
            InternalRemoveChild?.Invoke(child);
        }

        protected void InvalidRender()
        {
            InternalInvalidRender?.Invoke();
        }
        #endregion
    }
}
