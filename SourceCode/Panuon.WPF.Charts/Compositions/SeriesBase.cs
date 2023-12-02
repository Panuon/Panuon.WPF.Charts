using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class SeriesBase
        : DependencyObject
    {
        #region Properties

        #region ValueMemberPath
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SeriesBase));
        #endregion

        #endregion

        #region Methods

        #region Internal Methods
        internal void Render(IDrawingContext drawingContext,
            ICanvasContext canvasContext,
            IEnumerable<ICoordinate> coordinates)
        {
            OnRendering(drawingContext, canvasContext, coordinates);
        }
        #endregion

        #region Abstract Methods

        protected abstract void OnRendering(IDrawingContext drawingContext,
            ICanvasContext canvasContext,
            IEnumerable<ICoordinate> coordinates);


        protected virtual void OnMouseOver(ICanvasContext canvasContext,
            ICoordinate coordinate)
        { }

        protected virtual void OnMouseLeave(ICanvasContext canvasContext) 
        { }
        #endregion

        #endregion
    }
}