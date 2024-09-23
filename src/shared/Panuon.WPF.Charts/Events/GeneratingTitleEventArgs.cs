using System;

namespace Panuon.WPF.Charts
{
    public class GeneratingTitleEventArgs
        : EventArgs
    {
        #region Ctor
        public GeneratingTitleEventArgs(
            double value,
            string label
        )
        {
            Value = value;
            Label = label;
        }
        #endregion

        #region Properties
        public double Value { get; }

        public string Label { get; set; }
        #endregion
    }
}
