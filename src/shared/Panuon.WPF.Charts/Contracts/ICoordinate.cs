namespace Panuon.WPF.Charts
{
    public interface ICoordinate
    {
        string Label { get; }

        int Index { get; }

        double GetValue(IChartArgument seriesOrSegment);

        double Offset { get; }
    }
}
