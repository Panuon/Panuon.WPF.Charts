using Panuon.WPF.Resources;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Panuon.WPF.Charts.Controls.Internals
{
    internal class LegendPanel
        : FrameworkElement
    {
        #region Fields
        private ChartBase _chart;

        private UIElementCollection _children;

        private Label _label;

        private ItemsControl _itemsControl;
        #endregion

        #region Ctor
        internal LegendPanel(ChartBase chart)
        {
            _chart = chart;
            _children = new UIElementCollection(this, this);
            _itemsControl = new ItemsControl();
            _itemsControl.SetBinding(ItemsControl.ItemTemplateProperty, new Binding
            {
                Path = new PropertyPath(ChartBase.LegendItemTemplateProperty),
                Source = chart
            });
            _label = new Label
            {
                Content = _itemsControl
            };
            _label.SetBinding(FrameworkElement.StyleProperty, new Binding
            {
                Path = new PropertyPath(ChartBase.LegendLabelStyleProperty),
                Source = chart
            });
            _label.SetBinding(UIElement.VisibilityProperty, new Binding
            {
                Path = new PropertyPath(ChartBase.ShowLegendProperty),
                Converter = Converters.FalseToCollapseConverter,
                Source = chart
            });
            _children.Add(_label);
        }
        #endregion

        #region Overrides
        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size MeasureOverride(Size availableSize)
        {
            _label.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var legendPosition = _chart.LegendPosition;
            var legendOffsetX = _chart.LegendOffsetX;
            var legendOffsetY = _chart.LegendOffsetY;
            switch (legendPosition)
            {
                case LegendPosition.Top:
                    _label.Arrange(new Rect(legendOffsetX + (finalSize.Width - _label.DesiredSize.Width) / 2.0, legendOffsetY, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
                case LegendPosition.TopRight:
                    _label.Arrange(new Rect(finalSize.Width - _label.DesiredSize.Width + legendOffsetX, legendOffsetY, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
                case LegendPosition.Right:
                    _label.Arrange(new Rect(finalSize.Width - _label.DesiredSize.Width + legendOffsetX, legendOffsetY + (finalSize.Height - _label.DesiredSize.Height) / 2.0, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
                case LegendPosition.BottomRight:
                    _label.Arrange(new Rect(finalSize.Width - _label.DesiredSize.Width + legendOffsetX, finalSize.Height - _label.DesiredSize.Height + legendOffsetY, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
                case LegendPosition.Bottom:
                    _label.Arrange(new Rect(legendOffsetX + (finalSize.Width - _label.DesiredSize.Width) / 2.0, finalSize.Height - _label.DesiredSize.Height + legendOffsetY, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
                case LegendPosition.BottomLeft:
                    _label.Arrange(new Rect(legendOffsetX, finalSize.Height - _label.DesiredSize.Height + legendOffsetY, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
                case LegendPosition.Left:
                    _label.Arrange(new Rect(legendOffsetX, legendOffsetY + (finalSize.Height - _label.DesiredSize.Height) / 2.0, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
                case LegendPosition.Center:
                    _label.Arrange(new Rect(legendOffsetX + (finalSize.Width - _label.DesiredSize.Width) / 2.0, legendOffsetY + (finalSize.Height - _label.DesiredSize.Height) / 2.0, _label.DesiredSize.Width, _label.DesiredSize.Height));
                    break;
            }
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        #endregion

        #region Internal Methods
        internal void UpdateEntries()
        {
            var list = new List<SeriesLegendEntry>();
            foreach (var item in _chart.GetSeries())
            {
                var enumerable = item.RetrieveLegendEntries();
                if (enumerable != null)
                {
                    list.AddRange(enumerable);
                }
            }
            _itemsControl.ItemsSource = list;
        }
        #endregion

        #region Functions
        #endregion
    }
}
