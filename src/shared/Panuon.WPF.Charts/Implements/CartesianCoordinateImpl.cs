using System.Collections;
using System.Collections.Generic;
using System.Data;

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

        internal Dictionary<IChartArgument, decimal?> Values { get; set; }
        #endregion

        public decimal? GetValue(IChartArgument seriesOrSegment)
        {
            return Values[seriesOrSegment];
        }

        public object GetSource()
        {
            if (_chart.ItemsSource is IEnumerable itemsSource)
            {
                var enumerator = itemsSource.GetEnumerator();
                enumerator.MoveNext();
                for (int i = 0; i < Index; i++)
                {
                    enumerator.MoveNext();
                }
                return enumerator.Current;
            }
            else if (_chart.ItemsSource is DataTable dataTable)
            {
                if (Index < dataTable.Rows.Count)
                {
                    return dataTable.Rows[Index];
                }
                return null;
            }
            return null;
        }

    }
}
