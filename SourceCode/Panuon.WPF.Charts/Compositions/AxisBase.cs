using Panuon.WPF.Charts.Controls.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public abstract class AxisBase
        : DependencyObject
    {
        #region RoutedEvent

        #endregion

        #region Properties

        #region LabelStyle
        public Style LabelStyle
        {
            get { return (Style)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register("LabelStyle", typeof(Style), typeof(AxisBase), new PropertyMetadata(null));
        #endregion

        #region Background
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            AxisPresenterBase.BackgroundProperty.AddOwner(typeof(AxisBase), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #endregion

        #region Event Handlers
        #endregion

        #region Functions
        
        #endregion

    }
}
