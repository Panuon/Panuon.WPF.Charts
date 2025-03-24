using System;
using System.Collections.Generic;

namespace Panuon.WPF.Charts.Implements
{
    internal class RadialCoordinateImpl
        : IRadialCoordinate
    {
        #region Fields
        private RadialChart _chart;
        #endregion

        #region Ctor
        public RadialCoordinateImpl(RadialChart chart)
        {
            _chart = chart;
        }
        #endregion

        #region Properties
        public string Label { get; set; }

        public int Index { get; set; }

        public double Offset { get; set; }

        public double StartAngle { get; set; }

        public double Angle { get; set; }
        #endregion

        #region Internal Properties
        internal Dictionary<IChartArgument, (double, double)> Angles { get; set; }

        internal Dictionary<IChartArgument, double> Values { get; set; }
        #endregion

        public (double, double) GetAngle(IChartArgument seriesOrSegment)
        {
            return Angles[seriesOrSegment];
        }

        public double GetValue(IChartArgument seriesOrSegment)
        {
            return Values[seriesOrSegment];
        }

        public object GetSource()
        {
            var itemsSource = _chart.ItemsSource.GetEnumerator();
            itemsSource.MoveNext();
            for (int i = 0; i < Index; i++)
            {
                itemsSource.MoveNext();
            }
            return itemsSource.Current;
        }
    }
}
