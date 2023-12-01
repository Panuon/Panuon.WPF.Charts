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
    internal class YAxisPresenter
        : AxisPresenterBase
    {
        #region Fields
        private ChartPanel _chartPanel;
        #endregion

        #region Ctor
        internal YAxisPresenter(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
            SetBinding(YAxisProperty, new Binding()
            {
                Path = new PropertyPath(ChartPanel.YAxisProperty),
                Source = _chartPanel,
            });
        }
        #endregion

        #region Properties

        #region YAxis
        public YAxis YAxis
        {
            get { return (YAxis)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(YAxis), typeof(YAxisPresenter), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
                OnYAxisChanged));
        #endregion

        #endregion

        #region Overrides

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(50, 0);
        }
        #endregion

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(DesiredSize.Width, finalSize.Height);
        }

        #region OnRender
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (YAxis == null)
            {
                return;
            }

            drawingContext.DrawRectangle(YAxis.Background,
                null, new Rect(0, 0, ActualWidth, ActualHeight));
        }
        #endregion

        #endregion

        #region Event Handlers
        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (YAxisPresenter)d;
            panel.OnYAxisChanged(e.OldValue as YAxis, e.NewValue as YAxis);
        }
        #endregion

        #region Functions
        private void OnYAxisChanged(YAxis oldAxis,
            YAxis newAxis)
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
