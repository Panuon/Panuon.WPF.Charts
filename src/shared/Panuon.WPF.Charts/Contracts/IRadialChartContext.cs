namespace Panuon.WPF.Charts
{
    public interface IRadialChartContext
        : IChartContext
    {
        new RadialChart Chart { get; }

        double GetValue();
    }
}
