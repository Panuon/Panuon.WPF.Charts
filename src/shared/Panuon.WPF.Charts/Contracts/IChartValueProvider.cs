namespace Panuon.WPF.Charts
{
    public interface IChartValueProvider
    {
        string Title { get; set; }

        string TitleMemberPath { get; set; }

        string ValueMemberPath { get; set; }
    }
}
