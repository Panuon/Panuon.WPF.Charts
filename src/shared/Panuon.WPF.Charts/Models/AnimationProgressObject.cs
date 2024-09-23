using System;
using System.Windows;

namespace Panuon.WPF.Charts
{
    class AnimationProgressObject
        : UIElement
    {
        public event EventHandler ProgressChanged;

        #region Properties

        #region Progress
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(AnimationProgressObject), new PropertyMetadata(0d, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (AnimationProgressObject)d;
            obj.ProgressChanged?.Invoke(obj, EventArgs.Empty);
        }
        #endregion



        public object Tag { get; set; }
        #endregion
    }
}
