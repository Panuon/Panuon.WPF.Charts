using System.Collections.Generic;

namespace Panuon.WPF.Charts.Implements
{
    internal class CartesianCoordinateImpl
        : ICartesianCoordinate
    {
        #region Fields
        private CartesianChart _chart;
        #endregion

        #region Ctor
        public CartesianCoordinateImpl(CartesianChart chart)
        {
            _chart = chart;
        }
        #endregion

        #region Properties
        public string Label { get; set; }

        public int Index { get; set; }

        public double Offset { get; set; }

        internal Dictionary<IChartArgument, double> Values { get; set; }
        #endregion

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
