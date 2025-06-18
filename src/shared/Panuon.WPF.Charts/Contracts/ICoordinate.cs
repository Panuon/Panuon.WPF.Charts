namespace Panuon.WPF.Charts
{
    public interface ICoordinate
    {
        string Label { get; }

        int Index { get; }

        decimal? GetValue(IChartArgument seriesOrSegment);

        object GetSource();

        double Offset { get; }
    }
}
