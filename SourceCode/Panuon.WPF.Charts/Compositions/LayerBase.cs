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
            ICanvasContext canvasContext,
            ILayerContext layerContext)
        {
            OnRender(drawingContext, canvasContext, layerContext);
        }

        internal void MouseIn(ICanvasContext canvasContext,
            ILayerContext layerContext)
        {
            OnMouseIn(canvasContext, layerContext);
        }

        internal void MouseOut(ICanvasContext canvasContext,
            ILayerContext layerContext)
        {
            OnMouseOut(canvasContext, layerContext);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnMouseIn(ICanvasContext canvasContext,
            ILayerContext layerContext)
        {

        }

        protected virtual void OnMouseOut(ICanvasContext canvasContext,
            ILayerContext layerContext)
        {

        }

        protected virtual void OnRender(IDrawingContext drawingContext,
            ICanvasContext canvasContext,
            ILayerContext layerContext) { }

        protected void AddChild(UIElement child)
        {
            InternalAddChild.Invoke(child);
        }

        protected void RemoveChild(UIElement child)
        {
            InternalRemoveChild.Invoke(child);
        }

        protected void InvalidRender()
        {
            InternalInvalidRender.Invoke();
        }
        #endregion
    }
}
