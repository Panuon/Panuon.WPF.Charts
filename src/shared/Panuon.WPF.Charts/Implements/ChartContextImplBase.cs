using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Panuon.WPF.Charts.Implements
{
    internal abstract class ChartContextImplBase
        : IChartContext
    {
        #region Ctor
        internal ChartContextImplBase(ChartBase chart)
        {
            Chart = chart;
        }
        #endregion

        #region Properties
        public ChartBase Chart { get; }

        public virtual double CanvasWidth => Chart._seriesPanel.RenderSize.Width;

        public virtual double CanvasHeight => Chart._seriesPanel.RenderSize.Height;

        public IEnumerable<SeriesBase> Series => Chart.GetSeries();

        public IEnumerable<LayerBase> Layers => Chart.Layers;

        public Point? GetMousePosition(MouseRelativeTarget relativeTo)
        {
            switch (relativeTo)
            {
                case MouseRelativeTarget.Chart:
                    if (Chart.IsMouseOver)
                    {
                        return Mouse.GetPosition(Chart);
                    }
                    break;
                case MouseRelativeTarget.Layer:
                    if (Chart._layersPanel.IsMouseOver)
                    {
                        return Mouse.GetPosition(Chart._layersPanel);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return null;
        }

        public abstract ICoordinate RetrieveCoordinate(Point offset);
        #endregion
    }
}
