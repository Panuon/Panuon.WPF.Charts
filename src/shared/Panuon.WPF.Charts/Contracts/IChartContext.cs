using System.Collections.Generic;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public interface IChartContext
    {
        ChartBase Chart { get; }

        double AreaWidth { get; }

        double AreaHeight { get; }

        IEnumerable<SeriesBase> Series { get; }

        IEnumerable<LayerBase> Layers { get; }

        Point? GetMousePosition(MouseRelativeTarget relativeTo);

        ICoordinate RetrieveCoordinate(Point offset);
    }
}
