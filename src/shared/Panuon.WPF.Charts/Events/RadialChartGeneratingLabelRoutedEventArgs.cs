using System.Windows;

namespace Panuon.WPF.Charts
{
    public class RadialChartGeneratingLabelRoutedEventArgs
        : RoutedEventArgs
    {
        #region Ctor
        public RadialChartGeneratingLabelRoutedEventArgs(
            RoutedEvent @event,
            string label,
            decimal? value,
            decimal totalValue
        ) : base(@event)
        {
            Value = value;
            Label = label;
            TotalValue = totalValue;
        }
        #endregion

        #region Properties
        public string Label { get; set; }

        public decimal? Value { get; }

        public decimal TotalValue { get; }
        #endregion
    }
}
