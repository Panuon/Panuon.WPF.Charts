namespace Panuon.WPF.Charts
{
    public interface IChartValueProvider
        : IChartArgument
    {
        string Title { get; set; }

        string ValueMemberPath { get; set; }
    }
}
