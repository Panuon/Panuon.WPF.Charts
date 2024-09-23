namespace Panuon.WPF.Charts
{
    public interface IChartValueProvider
        : IChartArgument
    {
        string Label { get; set; }

        string ValueMemberPath { get; set; }
    }
}
