using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Panuon.WPF.Charts.Implements
{
    internal class LayerContextImpl
        : ILayerContext
    {
        #region Fields
        private Func<Point?> _getMousePosition;
        private Func<double, ICoordinate> _getCoordinate;
        private Func<int, IReadOnlyDictionary<SeriesBase, double>> _getSeriesValue;
        #endregion

        #region Ctor
        internal LayerContextImpl(Func<Point?> getMousePosition,
            Func<double, ICoordinate> getCoordinate,
            Func<int, IReadOnlyDictionary<SeriesBase, double>> getSeriesValue)
        {
            _getMousePosition = getMousePosition;
            _getCoordinate = getCoordinate;
            _getSeriesValue = getSeriesValue;
        }
        #endregion

        #region Methods
        public Point? GetMousePosition()
        {
            return _getMousePosition.Invoke();
        }

        public ICoordinate GetCoordinate(double offsetX)
        {
            return _getCoordinate.Invoke(offsetX);
        }

        public IReadOnlyDictionary<SeriesBase, double> GetSeriesValue(int index)
        {
            return _getSeriesValue.Invoke(index);
        }
        #endregion
    }
}
