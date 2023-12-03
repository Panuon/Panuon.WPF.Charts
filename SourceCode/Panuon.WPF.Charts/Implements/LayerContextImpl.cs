using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private Func<int, IChartUnit, double> _getValue;
        #endregion

        #region Ctor
        internal LayerContextImpl(Func<Point?> getMousePosition,
            Func<double, ICoordinate> getCoordinate,
            Func<int, IChartUnit, double> getValue)
        {
            _getMousePosition = getMousePosition;
            _getCoordinate = getCoordinate;
            _getValue = getValue;
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

        public double GetValue(int index,
            IChartUnit seriesOrSegment)
        {
            return _getValue.Invoke(index, seriesOrSegment);
        }
        #endregion
    }
}
