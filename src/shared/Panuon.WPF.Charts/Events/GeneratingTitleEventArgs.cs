using System;

namespace Panuon.WPF.Charts
{
    public class GeneratingTitleEventArgs
        : EventArgs
    {
        #region Ctor
        public GeneratingTitleEventArgs(
            double value,
            string title
        )
        {
            Value = value;
            Title = title;
        }
        #endregion

        #region Properties
        public double Value { get; }

        public string Title { get; set; }
        #endregion
    }
}
