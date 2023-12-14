using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class SegmentBase
        : DependencyObject
    {
        #region Methods
        protected static void OnRenderPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var segments = (SegmentBase)d;
            //segments.InvalidRender();
        }
        #endregion
    }
}
