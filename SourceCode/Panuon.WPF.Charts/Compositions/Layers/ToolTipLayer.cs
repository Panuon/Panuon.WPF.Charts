using Panuon.WPF.Charts.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panuon.WPF.Charts
{
    public class ToolTipLayer
        : LayerBase
    {
        private Label _label;

        public ToolTipLayer()
        {
            _label = new Label()
            {
                ContentTemplate = (DataTemplate)Application.Current.FindResource("ToolTipDataTemplate"),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(25, 10, 25, 10),
                Visibility = Visibility.Collapsed
            };
            AddChild(_label);
        }

        protected override void OnMouseIn(IChartContext chartContext, ILayerContext layerContext)
        {
            _label.Content = null;
            _label.Visibility = Visibility.Visible;
            InvalidRender();
        }

        protected override void OnMouseOut(IChartContext chartContext, ILayerContext layerContext)
        {
            _label.Content = null;
            _label.Visibility = Visibility.Collapsed;
            InvalidRender();
        }

        protected override void OnRender(IDrawingContext drawingContext,
            IChartContext chartContext,
            ILayerContext layerContext)
        {
            if (layerContext.GetMousePosition() is Point position)
            {
                _label.Margin = new Thickness(position.X, position.Y, 0, 0);

                var tooltips = new List<SeriesTooltip>();
                foreach (var series in chartContext.Series)
                {
                    series.Highlight(drawingContext,
                        chartContext,
                        layerContext,
                        tooltips);
                }
                _label.Content = tooltips;
            }
        }
    }
}