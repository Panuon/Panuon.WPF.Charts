using Panuon.WPF.Charts.Implements;
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


        internal readonly Dictionary<double, FormattedText> _formattedTexts =
            new Dictionary<double, FormattedText>();
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
            _formattedTexts.Clear();

            if (YAxis == null)
            {
                return new Size(0, 0);
            }

            var deltaX = (_chartPanel.MaxValue - _chartPanel.MinValue) / 5;

            for(int i = 0; i <= 5; i++)
            {
                var formattedText = new FormattedText((deltaX * i).ToString(),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(YAxis.FontFamily, YAxis.FontStyle, YAxis.FontWeight, YAxis.FontStretch),
                    YAxis.FontSize,
                    YAxis.Foreground
#if NET452 || NET462 || NET472 || NET48
                    );
#else
                    ,VisualTreeHelper.GetDpi(this).PixelsPerDip);
#endif

                _formattedTexts.Add(deltaX * i, formattedText);
            }
            return new Size(_formattedTexts.Values.Max(x => x.Width) + YAxis.Spacing + YAxis.TicksSize + YAxis.StrokeThickness, 0);
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (YAxis == null)
            {
                return new Size(0, 0);
            }

            return new Size(DesiredSize.Width, finalSize.Height);
        }
        #endregion

        #region OnRender
        protected override void OnRender(DrawingContext context)
        {
            if (YAxis == null
                || !_chartPanel.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chartPanel.CreateDrawingContext(context);
            var chartContext = _chartPanel.GetCanvasContext();

            drawingContext.DrawLine(YAxis.Stroke, YAxis.StrokeThickness, ActualWidth, 0, ActualWidth, ActualHeight);

            var deltaY = chartContext.AreaHeight / (_formattedTexts.Count - 1);

            foreach (var valueText in _formattedTexts)
            {
                var value = valueText.Key;
                var text = valueText.Value;

                var offsetY = chartContext.GetOffset(value);
                drawingContext.DrawLine(YAxis.TicksBrush, YAxis.StrokeThickness, ActualWidth - YAxis.StrokeThickness, offsetY, ActualWidth - YAxis.StrokeThickness - YAxis.TicksSize, offsetY);
                drawingContext.DrawText(text, ActualWidth - YAxis.StrokeThickness - YAxis.Spacing - YAxis.TicksSize - text.Width, offsetY - text.Height / 2);
            }
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
