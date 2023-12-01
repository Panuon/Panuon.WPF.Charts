using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class XAxisPresenter
        : AxisPresenterBase
    {
        #region Fields
        private ChartPanel _chartPanel;
        #endregion

        #region Ctor
        internal XAxisPresenter(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
            SetBinding(XAxisProperty, new Binding()
            {
                Path = new PropertyPath(ChartPanel.XAxisProperty),
                Source = _chartPanel,
            });
        }
        #endregion

        #region Properties

        #region XAxis
        public XAxis XAxis
        {
            get { return (XAxis)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(XAxis), typeof(XAxisPresenter), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
                OnXAxisChanged));
        #endregion

        #endregion

        #region Overrides

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(0, 50);
        }
        #endregion

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(finalSize.Width, DesiredSize.Height);
        }

        #region OnRender
        protected override void OnRender(DrawingContext drawingContext)
        {
            if(XAxis == null)
            {
                return;
            }

            drawingContext.DrawRectangle(XAxis.Background,
                null, new Rect(0, 0, ActualWidth, ActualHeight));
        }
        #endregion

        #endregion

        #region Event Handlers
        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (XAxisPresenter)d;
            panel.OnXAxisChanged(e.OldValue as XAxis, e.NewValue as XAxis);
        }
        #endregion

        #region Functions
        private void OnXAxisChanged(XAxis oldAxis,
            XAxis newAxis)
        {
            if (oldAxis != null)
            {
                RemoveLogicalChild(oldAxis);
            }
            if (newAxis != null)
            {
                AddLogicalChild(newAxis);
            }
        }
        #endregion
    }
}
