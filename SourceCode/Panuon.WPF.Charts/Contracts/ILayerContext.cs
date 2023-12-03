using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface ILayerContext
    {
        public Point? GetMousePosition();

        public ICoordinate GetCoordinate(double offsetX);

        public IReadOnlyDictionary<SeriesBase, double> GetSeriesValue(int index);

    }
}
