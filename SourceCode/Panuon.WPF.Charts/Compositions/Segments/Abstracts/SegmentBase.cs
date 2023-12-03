using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Panuon.WPF.Charts
{
    public abstract class SegmentBase
        : DependencyObject, IChartUnit
    {
        #region Properties

        #region ValueMemberPath
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SegmentBase));
        #endregion

        #endregion

        #region Methods


        protected static void OnRenderPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var segments = (SegmentBase)d;
            //segments.InvalidRender();
        }
        #endregion
    }
}
