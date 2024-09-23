using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class CartesianSegmentsSeriesBase
        : CartesianSeriesBase
    {
        #region Methods
        internal CartesianSegmentsSeriesBase() { }
        #endregion

        #region Properties

        #region ValuesMemberPath
        public string ValuesMemberPath
        {
            get { return (string)GetValue(ValuesMemberPathProperty); }
            set { SetValue(ValuesMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValuesMemberPathProperty =
            DependencyProperty.Register("ValuesMemberPath", typeof(string), typeof(CartesianSegmentsSeriesBase));
        #endregion

        #endregion

        #region Abstract Methods
        public abstract IEnumerable<SegmentBase> GetSegments();
        #endregion

        #region Overrides
        protected override ICoordinate OnRetrieveCoordinate(
           IChartContext chartContext,
           ILayerContext layerContext,
           Point position)
        {
            return layerContext.GetCoordinate(position.X);
        }
        #endregion
    }
}