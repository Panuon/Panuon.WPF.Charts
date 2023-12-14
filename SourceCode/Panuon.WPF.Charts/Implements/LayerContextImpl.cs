using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Panuon.WPF.Charts.Implements
{
    internal class LayerContextImpl
        : ILayerContext
    {
        #region Fields
        private ChartPanel _chartPanel;
        
        #endregion

        #region Ctor
        internal LayerContextImpl(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
        }
        #endregion

        #region Methods
        public Point? GetMousePosition()
        {
            if (_chartPanel._layersPanel.IsMouseOver)
            {
                return Mouse.GetPosition(_chartPanel._layersPanel);
            }
            return null;
        }

        public ICoordinate GetCoordinate(double offsetX)
        {
            if (offsetX < 0 ||
                offsetX > _chartPanel.ActualWidth)
            {
                return null;
            }
            var leftCoordinate = _chartPanel.Coordinates.LastOrDefault(x => x.Offset <= offsetX);
            var rightCoordinate = _chartPanel.Coordinates.FirstOrDefault(y => y.Offset >= offsetX);
            if (leftCoordinate == null &&
                rightCoordinate == null)
            {
                return null;
            }
            if (leftCoordinate == null)
            {
                return rightCoordinate;
            }
            if (rightCoordinate == null)
            {
                return leftCoordinate;
            }
            return Math.Abs(leftCoordinate.Offset - offsetX) <= Math.Abs(rightCoordinate.Offset - offsetX)
                ? leftCoordinate
                : rightCoordinate;
        }

        public double GetValue(int index,
            IChartValueProvider seriesOrSegment)
        {
            var coordinate = _chartPanel.Coordinates.FirstOrDefault(x => x.Index == index);
            if (coordinate == null ||
                !coordinate.Values.ContainsKey(seriesOrSegment))
            {
                return _chartPanel.MinValue;
            }
            else
            {
                return coordinate.Values[seriesOrSegment];
            }
        }
        #endregion
    }
}
