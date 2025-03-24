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
            double value,
            double totalValue
        ) : base(@event)
        {
            Value = value;
            Label = label;
            TotalValue = totalValue;
        }
        #endregion

        #region Properties
        public string Label { get; set; }

        public double Value { get; }

        public double TotalValue { get; }
        #endregion
    }
}
