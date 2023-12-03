using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface ILayerContext
    {
        public Point? GetMousePosition();

        public ICoordinate GetCoordinate(double offsetX);

        double GetValue(int index,
            IChartUnit seriesOrSegment);


    }
}
