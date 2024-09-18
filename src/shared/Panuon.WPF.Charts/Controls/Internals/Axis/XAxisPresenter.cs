using Panuon.WPF.Charts.Implements;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class XAxisPresenter
        : AxisPresenterBase
    {
        #region Fields
        private CartesianChart _chart;

        internal readonly Dictionary<CoordinateImpl, FormattedText> _formattedTexts = 
            new Dictionary<CoordinateImpl, FormattedText>();
        #endregion

        #region Ctor
        internal XAxisPresenter(CartesianChart chart)
        {
            _chart = chart;
            SetBinding(XAxisProperty, new Binding()
            {
                Path = new PropertyPath(CartesianChart.XAxisProperty),
                Source = _chart,
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
            base.MeasureOverride(availableSize);

            _formattedTexts.Clear();

            if (XAxis == null)
            {
                return new Size(0, 0);
            }

            foreach (var coordinate in _chart.Coordinates)
            {
                if(coordinate.Title == null)
                {
                    continue;
                }
                var formattedText = new FormattedText(coordinate.Title,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(XAxis.FontFamily, XAxis.FontStyle, XAxis.FontWeight, XAxis.FontStretch),
                    XAxis.FontSize,
                    XAxis.Foreground
#if NET452 || NET462 || NET472 || NET48
                    );
#else
                    , VisualTreeHelper.GetDpi(this).PixelsPerDip);
#endif

                _formattedTexts.Add(coordinate, formattedText);
            }

            if (!_formattedTexts.Any())
            {
                return new Size(0, 0);
            }
            return new Size(0, _formattedTexts.Values.Max(x => x.Height) + XAxis.Spacing + XAxis.TicksSize + XAxis.StrokeThickness);
        }
#endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (XAxis == null)
            {
                return new Size(0, 0);
            }

            return new Size(finalSize.Width, DesiredSize.Height);
        }
        #endregion

        #region OnRender
        protected override void OnRender(DrawingContext context)
        {
            if(XAxis == null
                || !_chart.IsCanvasReady())
            {
                return;
            }

            var drawingContext = _chart.CreateDrawingContext(context);

            drawingContext.DrawLine(
                XAxis.Stroke, 
                XAxis.StrokeThickness, 
                0, 
                0,
                ActualWidth, 
                0
            );
            foreach(var coordinateText in _formattedTexts)
            {
                var coordinate = coordinateText.Key;
                var text = coordinateText.Value;

                var offsetX = coordinate.Offset;

                drawingContext.DrawLine(
                    XAxis.TicksBrush,
                    XAxis.StrokeThickness,
                    offsetX, 
                    XAxis.StrokeThickness,
                    offsetX,
                    XAxis.StrokeThickness + XAxis.TicksSize
                );
                drawingContext.DrawText(
                    text,
                    offsetX - text.Width / 2,
                    XAxis.Spacing + XAxis.TicksSize + XAxis.StrokeThickness
                );
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
