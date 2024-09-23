using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface ILayerContext
    {
        Point? GetMousePosition();

        ICoordinate GetCoordinate(double offsetX);

        double GetValue(
            int index,
            IChartValueProvider seriesOrSegment
        );
    }
}
