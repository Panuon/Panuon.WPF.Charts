using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class SeriesPresenter
        : FrameworkElement
    {
        #region Fields

        #endregion

        #region Ctor
        internal SeriesPresenter(ChartPanel chartPanel)
        {

        }
        #endregion

        #region Overrides

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AA0000FF"))
                , null, new Rect(0, 0, ActualWidth, ActualHeight));
        }
        #endregion
    }
}
