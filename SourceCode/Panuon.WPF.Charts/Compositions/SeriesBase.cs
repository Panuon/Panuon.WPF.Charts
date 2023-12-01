using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            IEnumerable<ICoordinate> coordinates)
        {
            OnRendering(drawingContext, coordinates);
        }
        #endregion

        #region Abstract Methods

        protected abstract void OnRendering(IDrawingContext drawingContext,
            IEnumerable<ICoordinate> coordinates);

        #endregion

        #endregion
    }
}