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
    internal class XAxisPresenter
        : AxisPresenterBase
    {
        #region Fields
        private ChartPanel _chartPanel;

        internal readonly Dictionary<CoordinateImpl, FormattedText> _formattedTexts = 
            new Dictionary<CoordinateImpl, FormattedText>();
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
            _formattedTexts.Clear();

            foreach(var coordinate in _chartPanel.Coordinates)
            {
                var formattedText = new FormattedText(coordinate.Title,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(XAxis.FontFamily, XAxis.FontStyle, XAxis.FontWeight, XAxis.FontStretch),
                    XAxis.FontSize,
                    XAxis.Foreground,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                _formattedTexts.Add(coordinate, formattedText);
            }

            return new Size(0, _formattedTexts.Values.Max(x => x.Height) + XAxis.Spacing + XAxis.TicksSize + XAxis.StrokeThickness);
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
                return new Size(finalSize.Width, DesiredSize.Height);
        }
        #endregion

        #region OnRender
        protected override void OnRender(DrawingContext context)
        {
            if(XAxis == null
                || !_chartPanel.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chartPanel.CreateDrawingContext(context);
            var canvasContext = _chartPanel.GetCanvasContext();

            drawingContext.DrawLine(XAxis.Stroke, XAxis.StrokeThickness, 0, 0, ActualWidth, 0);
            foreach(var coordinateText in _formattedTexts)
            {
                var coordinate = coordinateText.Key;
                var text = coordinateText.Value;

                var offsetX = coordinate.Offset;

                drawingContext.DrawLine(XAxis.TicksBrush, XAxis.StrokeThickness, offsetX, XAxis.StrokeThickness, offsetX, XAxis.StrokeThickness + XAxis.TicksSize);
                drawingContext.DrawText(text, offsetX - text.Width / 2, XAxis.Spacing + XAxis.TicksSize + XAxis.StrokeThickness);
            }
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
